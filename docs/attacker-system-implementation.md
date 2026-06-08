# 进攻方系统实现文档

## 模块概述

进攻方系统负责管理游戏中的怪物单位，包括数据定义、移动控制、生成管理等功能。该系统是 1-1 关卡必需的核心模块。

## 类图/结构图

```
┌─────────────────────────────────────────────────────────────┐
│                      AttackerData                           │
│                      (ScriptableObject)                      │
├─────────────────────────────────────────────────────────────┤
│ - _attackerName: string                                     │
│ - _maxHealth: int                                           │
│ - _attackPower: int                                         │
│ - _moveSpeed: float                                         │
│ - _prefab: GameObject                                       │
├─────────────────────────────────────────────────────────────┤
│ + AttackerName: string                                      │
│ + MaxHealth: int                                            │
│ + AttackPower: int                                          │
│ + MoveSpeed: float                                          │
│ + Prefab: GameObject                                        │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ 引用
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                         Attacker                            │
│                       (MonoBehaviour)                        │
├─────────────────────────────────────────────────────────────┤
│ - _data: AttackerData                                       │
│ - _currentHealth: int                                       │
│ - _currentLane: int                                         │
│ - _isDead: bool                                             │
├─────────────────────────────────────────────────────────────┤
│ + Initialize(data, lane): void                              │
│ + TakeDamage(damage): void                                  │
│ - Die(): void                                               │
│ + OnAttackerDead: event                                     │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ 组合
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                     AttackerMovement                         │
│                       (MonoBehaviour)                        │
├─────────────────────────────────────────────────────────────┤
│ - _moveSpeed: float                                         │
│ - _currentLane: int                                         │
│ - _isMoving: bool                                           │
│ - _targetX: float                                           │
├─────────────────────────────────────────────────────────────┤
│ + Initialize(lane, speed): void                             │
│ + Stop(): void                                              │
│ - Update(): void                                            │
│ - OnReachedEnd(): void                                      │
│ - ReturnToPool(): void                                      │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                     AttackerSpawner                          │
│                       (MonoBehaviour)                        │
│                     (Singleton Pattern)                      │
├─────────────────────────────────────────────────────────────┤
│ - _spawnTimeline: List<SpawnEntry>                          │
│ - _gameTimer: float                                         │
│ - _nextSpawnIndex: int                                      │
│ - _isSpawning: bool                                         │
├─────────────────────────────────────────────────────────────┤
│ + StartSpawning(): void                                     │
│ + StopSpawning(): void                                      │
│ + SetTimeline(timeline): void                               │
│ + AddSpawnEntry(data, lane, time): void                     │
│ + OnAttackerSpawned: event                                  │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ 依赖
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      ObjectPool                              │
│                     (Singleton Pattern)                      │
├─────────────────────────────────────────────────────────────┤
│ + Get(poolName, position, rotation): GameObject             │
│ + Return(poolName, obj): void                               │
└─────────────────────────────────────────────────────────────┘
```

## 关键实现

### 1. AttackerData ScriptableObject

使用 `[CreateAssetMenu]` 特性创建可配置的怪物数据资产：

```csharp
[CreateAssetMenu(fileName = "NewAttackerData", menuName = "Game/Attacker Data")]
public class AttackerData : ScriptableObject
{
    [SerializeField] private string _attackerName = "New Attacker";
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _attackPower = 10;
    [SerializeField] private float _moveSpeed = 0.5f;
    // ...
}
```

**创建步骤：**
1. 在 Unity Editor 中，右键点击 Project 窗口
2. 选择 `Create → Game → Attacker Data`
3. 命名资产（如 `DustSpriteData`）
4. 在 Inspector 中配置属性

### 2. Attacker 基类

管理怪物的生命周期，包括受伤、死亡、对象池回收：

```csharp
public class Attacker : MonoBehaviour
{
    public void Initialize(AttackerData data, int lane)
    {
        _data = data;
        _currentHealth = data.MaxHealth;
        _currentLane = lane;
        _isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;
        _currentHealth -= damage;
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        _isDead = true;
        OnAttackerDead?.Invoke(this);
        ObjectPool.Instance.Return("Attacker", gameObject);
    }
}
```

### 3. AttackerMovement 移动控制

实现怪物沿跑道从右向左移动，到达终点时触发防线突破判定：

```csharp
public class AttackerMovement : MonoBehaviour
{
    private void Update()
    {
        if (!_isMoving) return;

        transform.position += Vector3.left * _moveSpeed * Time.deltaTime;

        if (transform.position.x <= _targetX)
        {
            _isMoving = false;
            OnReachedEnd();
        }
    }

    private void OnReachedEnd()
    {
        if (LaneManager.Instance != null)
        {
            bool defended = LaneManager.Instance.OnMonsterReachedEnd(_currentLane);
            if (!defended)
            {
                Debug.Log($"[AttackerMovement] 跑道 {_currentLane} 防线被突破！");
            }
        }
        ReturnToPool();
    }
}
```

**关键点：**
- 使用 `Vector3.left * _moveSpeed * Time.deltaTime` 实现帧率无关的移动
- 到达终点时调用 `LaneManager.OnMonsterReachedEnd()` 触发防线突破判定
- 支持对象池回收

### 4. AttackerSpawner 生成器

按配置的时间轴生成怪物，支持多条跑道和多种怪物类型：

```csharp
public class AttackerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public AttackerData attackerData;
        public int laneIndex;
        public float spawnTime;
    }

    private void Update()
    {
        if (!_isSpawning) return;
        if (_nextSpawnIndex >= _spawnTimeline.Count) return;

        _gameTimer += Time.deltaTime;

        while (_nextSpawnIndex < _spawnTimeline.Count)
        {
            SpawnEntry entry = _spawnTimeline[_nextSpawnIndex];
            if (_gameTimer >= entry.spawnTime)
            {
                SpawnAttacker(entry);
                _nextSpawnIndex++;
            }
            else break;
        }
    }
}
```

**关键点：**
- 使用 `GameManager.OnGameStateChanged` 事件监听游戏状态
- 时间轴按时间排序，支持动态添加生成条目
- 生成位置在屏幕右侧外（`GridSystem.Cols - 1` + 偏移）

## 配置说明

### AttackerData Inspector 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| AttackerName | string | "New Attacker" | 怪物名称 |
| Description | string | "" | 怪物描述 |
| Icon | Sprite | null | 怪物图标 |
| Prefab | GameObject | null | 怪物预制体 |
| MaxHealth | int | 100 | 最大血量 |
| AttackPower | int | 10 | 攻击力 |
| AttackSpeed | float | 1f | 攻击速度（次/秒） |
| MoveSpeed | float | 0.5f | 移动速度（格/秒） |
| HasSpecialAbility | bool | false | 是否有特殊能力 |
| SpecialAbilityDescription | string | "" | 特殊能力描述 |

### AttackerSpawner Inspector 参数

| 参数 | 类型 | 说明 |
|------|------|------|
| SpawnTimeline | List<SpawnEntry> | 生成时间轴配置 |
| SpawnOffsetX | float | 生成位置X轴偏移 |

## 使用示例

### 1. 创建怪物数据资产

```csharp
// 在 Unity Editor 中创建
// 右键 → Create → Game → Attacker Data
// 命名: DustSpriteData
// 配置: MaxHealth=100, AttackPower=10, MoveSpeed=0.5f
```

### 2. 创建怪物预制体

> 📖 详细步骤参见：[`game/Assets/Prefabs/Attackers/README.md`](../game/Assets/Prefabs/Attackers/README.md)

**快速步骤：**
1. 创建空 GameObject，命名为 "DustSprite"
2. 添加 SpriteRenderer、Attacker、AttackerMovement、Collider2D 组件
3. 保存为 Prefab 到 `game/Assets/Prefabs/Attackers/`

### 3. 配置对象池

**方法一：Inspector 配置（推荐）**

1. 在 Hierarchy 中找到 `ObjectPool` GameObject
2. 在 Inspector 中找到 `_poolConfigs` 列表
3. 点击 `+` 添加新配置
4. 填写以下字段：
   - **Pool Name**: `"Attacker"`（必须与代码中使用的名称一致）
   - **Prefab**: 拖拽 `DustSprite.prefab` 到此字段
   - **Initial Size**: `10`（初始实例数量）
   - **Auto Expand**: `true`（自动扩展，池空时创建新实例）

**方法二：代码自动注册**

如果需要在运行时自动注册对象池，可以在 `GameManager.Start()` 或场景初始化时添加：

```csharp
if (ObjectPool.Instance != null)
{
    var attackerPrefab = Resources.Load<GameObject>("Prefabs/Attackers/DustSprite");
    if (attackerPrefab != null)
    {
        ObjectPool.Instance.CreatePool(new ObjectPool.PoolConfig
        {
            poolName = "Attacker",
            prefab = attackerPrefab,
            initialSize = 10,
            autoExpand = true
        });
    }
}
```

**注意事项**：
- 池名称 `"Attacker"` 必须与 `AttackerSpawner.cs:119` 和 `AttackerMovement.cs:69` 中使用的名称完全一致
- 如果 Prefab 未配置，怪物生成时会使用 `Instantiate` 作为 fallback（性能较差）
- 建议在游戏开始前确保对象池已正确配置

### 4. 配置生成时间轴

```csharp
// 在 AttackerSpawner 的 Inspector 中添加生成条目:
// [0] AttackerData: DustSpriteData, Lane: 0, Time: 2.0
// [1] AttackerData: DustSpriteData, Lane: 0, Time: 5.0
// [2] AttackerData: DustSpriteData, Lane: 1, Time: 8.0
```

### 5. 运行时动态添加生成条目

```csharp
if (AttackerSpawner.Instance != null)
{
    AttackerSpawner.Instance.AddSpawnEntry(dustSpriteData, 0, 10.0f);
}
```

## 注意事项

### 已知限制

1. **Prefab 创建**：Prefab 必须在 Unity Editor 中手动创建，无法通过代码生成
2. **对象池配置**：需要在 ObjectPool 的 Inspector 中预先配置 "Attacker" 池
3. **碰撞检测**：当前实现未包含怪物与防御者的碰撞/攻击逻辑

### 性能考量

1. **对象池**：使用 ObjectPool 管理怪物实例，避免频繁创建/销毁
2. **时间轴排序**：生成时间轴在设置时排序，运行时按序检查，避免遍历
3. **事件驱动**：使用 C# 事件而非轮询，减少不必要的计算

### 后续优化方向

1. **攻击逻辑**：实现怪物遇到防御者时停下攻击的逻辑
2. **动画系统**：添加怪物移动、攻击、死亡动画
3. **音效集成**：添加怪物生成、移动、攻击、死亡音效
4. **特效系统**：添加死亡特效、攻击特效
5. **路径finding**：支持更复杂的移动路径（非直线）

## 与其他系统的集成

### GridSystem
- 使用 `GridSystem.GridToWorld()` 获取格子世界坐标
- 使用 `GridSystem.Cols` 确定跑道长度

### LaneManager
- 调用 `LaneManager.OnMonsterReachedEnd()` 触发防线突破判定
- 监听 `LaneManager.OnAllCarsTriggered` 事件（可选）

### ObjectPool
- 使用 `ObjectPool.Get()` 获取怪物实例
- 使用 `ObjectPool.Return()` 回收怪物实例

### GameManager
- 监听 `GameManager.OnGameStateChanged` 事件控制生成状态
- 游戏状态为 `InProgress` 时开始生成
