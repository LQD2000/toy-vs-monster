# 核心框架实现文档

## 模块概述

实现玩具塔防游戏的核心框架，包含三个基础组件：
- **GameManager** - 游戏状态管理
- **EventSystem** - 事件发布/订阅系统
- **ObjectPool** - 对象池管理

## 类图/结构图

```
GameManager (Singleton)
├── GameState enum {Preparation, InProgress, Victory, Defeat}
├── CurrentState (readonly)
├── OnGameStateChanged event
├── SetState(GameState)
├── StartGame()
├── WinGame()
├── LoseGame()
└── ResetGame()

EventSystem (Singleton)
├── _eventTable: Dictionary<string, List<Delegate>>
├── On(string, Action)
├── On<T>(string, Action<T>)
├── Off(string, Action)
├── Off<T>(string, Action<T>)
├── Emit(string)
├── Emit<T>(string, T)
├── Clear(string)
└── ClearAll()

ObjectPool (Singleton)
├── PoolConfig class
│   ├── poolName
│   ├── prefab
│   ├── initialSize
│   └── autoExpand
├── _pools: Dictionary<string, Queue<GameObject>>
├── CreatePool(PoolConfig)
├── Get(string, Vector3, Quaternion)
├── Return(string, GameObject)
├── ClearPool(string)
└── ClearAllPools()
```

## 关键实现

### GameManager
- 使用单例模式确保全局唯一
- DontDestroyOnLoad 跨场景持久化
- 状态变更时触发事件通知其他系统
- 状态转换有严格验证（如只能从 InProgress 到 Victory/Defeat）

### EventSystem
- 基于字典的事件表实现 O(1) 查找
- 支持泛型参数的事件订阅
- 使用 ToArray() 遍历避免遍历时修改异常
- 异常处理防止单个回调影响整个事件链

### ObjectPool
- 支持多池配置（弹丸池、怪物池、电力球池等）
- 自动扩展机制（可配置）
- 预创建策略减少运行时分配
- 父子关系管理（归还时重新设置 parent）

## 配置说明

### ObjectPool Inspector 参数

| 参数 | 类型 | 说明 |
|------|------|------|
| poolName | string | 对象池唯一标识 |
| prefab | GameObject | 要池化的预制体 |
| initialSize | int | 初始创建数量 |
| autoExpand | bool | 池满时是否自动扩展 |

## 使用示例

### GameManager
```csharp
// 获取当前状态
GameState state = GameManager.Instance.CurrentState;

// 开始游戏
GameManager.Instance.StartGame();

// 监听状态变化
GameManager.Instance.OnGameStateChanged += (oldState, newState) => {
    Debug.Log($"状态从 {oldState} 变为 {newState}");
};
```

### EventSystem
```csharp
// 订阅事件
EventSystem.Instance.On("EnemyDied", OnEnemyDied);

// 带参数订阅
EventSystem.Instance.On<int>("ScoreChanged", OnScoreChanged);

// 触发事件
EventSystem.Instance.Emit("EnemyDied");
EventSystem.Instance.Emit("ScoreChanged", 100);

// 取消订阅
EventSystem.Instance.Off("EnemyDied", OnEnemyDied);
```

### ObjectPool
```csharp
// 从池中获取
GameObject bullet = ObjectPool.Instance.Get("Bullet", spawnPos, Quaternion.identity);

// 归还到池
ObjectPool.Instance.Return("Bullet", bullet);

// 动态创建池
var config = new ObjectPool.PoolConfig {
    poolName = "PowerOrb",
    prefab = powerOrbPrefab,
    initialSize = 20,
    autoExpand = true
};
ObjectPool.Instance.CreatePool(config);
```

## 注意事项

1. **初始化顺序**: 三个管理器都使用 DontDestroyOnLoad，需注意 Awake 执行顺序
2. **线程安全**: 当前实现非线程安全，仅限主线程使用
3. **内存管理**: 对象池不会自动释放，需手动调用 ClearPool/ClearAllPools
4. **事件清理**: 场景切换时建议调用 EventSystem.ClearAll() 防止引用泄漏

## 后续优化方向

1. 支持异步加载预制体（Addressables）
2. 对象池自动收缩策略
3. 事件系统支持优先级
4. 添加调试面板显示池状态
