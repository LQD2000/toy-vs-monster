# 防守方系统实现文档

## 模块概述

实现玩具塔防游戏的第4步：防守方系统（1-1 关卡必需）。包含六个核心组件：

- **DefenderData** - ScriptableObject 数据定义：攻击力、血量、电力消耗等属性
- **Defender** - 防守方基类：管理单位生命周期、受伤、死亡
- **MarbleShooter** - 弹珠射手：弹射类攻击，发射弹丸沿跑道飞行
- **Projectile** - 弹丸：沿跑道直线飞行，命中同跑道敌人造成伤害
- **DefenderFactory** - 防守方工厂：根据 DefenderData 创建 GameObject 实例
- **UnitPlacement** - 单位放置系统：点击放置逻辑、电力消耗验证、格子占用管理
- **ResourceManager** - 资源管理器：电力资源管理
- **Enemy** - 敌人基类：生命值管理、受伤、死亡（供弹丸命中交互）

**注意**：敌人生成和波次系统由进攻方系统（步骤5）实现，单元升级系统由后续步骤实现。

## 类图/结构图

```
DefenderData (ScriptableObject)
├── _defenderName: string        # 单位名称
├── _description: string         # 单位描述
├── _icon: Sprite                # 单位图标
├── _prefab: GameObject          # 单位预制体
├── _attackPower: int            # 攻击力（默认 10）
├── _maxHealth: int              # 初始血量（默认 100）
├── _attackSpeed: float          # 攻击速度/秒（默认 1）
├── _range: int                  # 射程格子数（-1=整条跑道）
├── _attackType: AttackType      # 攻击类型
├── _powerCost: int              # 电力消耗（默认 50）
└── _maxLevel: int               # 最大等级（默认 12）

AttackType (enum)
├── Projectile    # 弹射类（弹珠射手）
├── Block         # 阻挡类（积木墙）
└── Generate      # 产电类（小太阳宝宝）

Defender (Abstract Mono)
├── _data: DefenderData          # 单位数据引用
├── _currentHealth: int          # 当前血量
├── _currentRow: int             # 所在行
├── _currentCol: int             # 所在列
├── _isDead: bool                # 是否死亡
├── OnDefenderDead: event        # 死亡事件
├── Initialize(data, row, col)   # 初始化
├── TakeDamage(damage)           # 受伤
└── Die()                        # 死亡（释放格子 + 销毁）

MarbleShooter : Defender
├── _projectilePrefab: GameObject# 弹丸预制体
├── _firePoint: Transform        # 发射点
├── _attackTimer: float          # 攻击计时器
├── HasTargetInRange() → bool    # 射程检查
└── TryAttack()                  # 尝试发射弹丸

Projectile (MonoBehaviour)
├── _speed: float                # 飞行速度（默认 10）
├── _damage: int                 # 伤害值
├── _laneRow: int                # 所属跑道（限制伤害同跑道敌人）
├── _initialized: bool           # 是否已初始化
├── Initialize(damage, laneRow)  # 初始化
├── GetRightBoundary() → float   # 计算网格右边界
└── ReturnToPool()               # 归还对象池

DefenderFactory (Singleton)
├── _availableDefenders: DefenderData[]  # 可用单位列表
├── CreateDefender(data, row, col) → Defender
└── GetAvailableDefenders() → DefenderData[]

UnitPlacement (Singleton)
├── _controls: PlayerControls    # 新 Input System 引用
├── _selectedDefender: DefenderData  # 当前选中的单位
├── OnDefenderSelected: event    # 单位选中事件
├── OnDefenderPlaced: event      # 放置成功事件
├── OnPlacementFailed: event     # 放置失败事件
├── SelectDefender(data)         # 选择单位
├── ClearSelection()             # 清除选择
├── OnTap(InputAction.CallbackContext)  # 点击处理
└── TryPlaceDefender()           # 尝试放置

ResourceManager (Singleton, Core/)
├── _maxPower: int               # 最大电力（默认 200）
├── _currentPower: int           # 当前电力
├── OnPowerChanged: event        # 电力变化事件
├── HasEnoughPower(cost) → bool  # 电力检查
├── ConsumePower(cost) → bool    # 消耗电力
├── AddPower(amount)             # 增加电力
└── ResetPower()                 # 重置电力

Enemy (MonoBehaviour)
├── _maxHealth: int              # 最大血量
├── _currentHealth: int          # 当前血量
├── _currentRow: int             # 所在跑道
├── IsDead: bool                 # 是否死亡
├── OnEnemyDead: event           # 死亡事件
├── Initialize(health, row)      # 初始化
├── TakeDamage(damage)           # 受伤
└── Die()                        # 死亡（归还对象池）
```

## 架构关系

```
                              ┌─────────────┐
                              │ GameManager │ (Step 2: 状态控制)
                              └──────┬──────┘
                                     │ Preparation 状态门控
         ┌───────────────────────────┼──────────────────────────┐
         │                           │                          │
  ┌──────▼──────┐            ┌──────▼──────┐           ┌───────▼──────┐
  │UnitPlacement│            │ResrcManager │           │   GridSystem │ (Step 3)
  │ (放置逻辑)  │──电力检查──│  (电力管理) │           │ (格子占用)   │
  └──────┬──────┘            └─────────────┘           └───────▲──────┘
         │ 调用工厂                                              │
  ┌──────▼────────┐                                     ┌──────┴──────┐
  │DefenderFactory│                                     │  Defender   │
  │  (实例创建)   │─────────实例化───────────────────────│  .Die()     │
  └───────────────┘                                     │  释放格子   │
                                                        └──────┬──────┘
                                                               │ 子类
                                                ┌──────────────┴──────────┐
                                                │                         │
                                        ┌───────▼───────┐         ┌──────▼──────┐
                                        │ MarbleShooter  │         │  (未来单位)  │
                                        │  (弹珠射手)    │         │             │
                                        └───────┬───────┘         └─────────────┘
                                                │ 发射
                                        ┌───────▼───────┐
                                        │   Projectile  │──命中──▶ Enemy
                                        │  (弹丸飞行)   │ (同跑道检查)
                                        └───────┬───────┘
                                                │ 归还/销毁
                                        ┌───────▼───────┐
                                        │  ObjectPool   │ (Step 2: 对象池)
                                        └───────────────┘
```

## 数据流：单位放置流程

```
玩家点击 (New Input System)
  │
  ▼
UnitPlacement.OnTap()
  ├── 检查 GameState == Preparation？（否 → 忽略）
  ├── 检查 _selectedDefender != null？（否 → 忽略）
  │
  ▼
UnitPlacement.TryPlaceDefender()
  ├── 1. 获取 TapPosition（屏幕坐标 → 世界坐标）
  ├── 2. GridSystem.WorldToGrid()（世界坐标 → 行列坐标）
  │     └── 超出范围？→ OnPlacementFailed("超出网格范围")
  ├── 3. GridSystem.IsCellOccupied(row, col)？
  │     └── 已占用？→ OnPlacementFailed("位置已占用")
  ├── 4. ResourceManager.HasEnoughPower(cost)？
  │     └── 不足？→ OnPlacementFailed("电力不足")
  ├── 5. DefenderFactory.CreateDefender(data, row, col)
  │     ├── GridSystem.GridToWorld(row, col) → 世界坐标
  │     ├── Instantiate(data.Prefab, worldPos)
  │     ├── defender.Initialize(data, row, col)
  │     └── 返回 Defender 实例
  ├── 6. ResourceManager.ConsumePower(cost)
  ├── 7. GridSystem.OccupyCell(row, col, defender.gameObject)
  ├── 8. OnDefenderPlaced(defender)
  └── 9. ClearSelection()
```

## 数据流：弹丸命中流程

```
MarbleShooter.Update()
  ├── _attackTimer 累积
  ├── 冷却完毕？
  │     └── HasTargetInRange()？
  │           ├── Data.Range == -1 → true（整条跑道）
  │           ├── Data.Range > 0 → Physics2D.OverlapBoxAll()
  │           │     沿跑道方向检测 enemies
  │           └── true → TryAttack()
  │
  ▼
MarbleShooter.TryAttack()
  ├── ObjectPool.Get("MarbleProjectile", firePoint, rotation)
  ├── projectile.Initialize(attackPower, currentRow)
  │
  ▼
Projectile.Update()
  ├── transform.position += Vector3.right * _speed * dt
  └── 超出右边界（GridSystem 计算）？→ ReturnToPool()
  │
  ▼
Projectile.OnTriggerEnter2D()
  ├── tag == "Enemy"？
  ├── enemy.CurrentRow == _laneRow？（同跑道检查）
  │     └── 是 → enemy.TakeDamage(_damage) + ReturnToPool()
  └── 否 → 忽略（穿过）
```

## 关键实现

### 1. DefenderData ScriptableObject

使用 `[CreateAssetMenu]` 特性，在 Unity 编辑器中通过右键菜单创建。

```csharp
[CreateAssetMenu(fileName = "NewDefenderData", menuName = "Game/Defender Data")]
public class DefenderData : ScriptableObject
```

所有字段使用 `[Header]` 分组 + `[SerializeField] private _fieldName` 模式，与项目规范一致。

### 2. Prefab 创建

> 📖 详细步骤参见：
> - [`game/Assets/Prefabs/Defenders/README.md`](../game/Assets/Prefabs/Defenders/README.md) - 弹珠射手 Prefab
> - [`game/Assets/Prefabs/Projectiles/README.md`](../game/Assets/Prefabs/Projectiles/README.md) - 弹丸 Prefab

**MarbleShooter Prefab 组件：**
- SpriteRenderer（精灵图）
- MarbleShooter（攻击脚本）
- Collider2D（点击检测）

**Marble Prefab 组件：**
- SpriteRenderer（弹珠图）
- Projectile（弹丸脚本）
- CircleCollider2D（触发器）
- Rigidbody2D（Kinematic）

### 3. 单位放置系统（UnitPlacement）

- 使用 **Unity New Input System** 的 `PlayerControls` 处理点击输入
- 通过 `_controls.Gameplay.Tap.performed` 事件订阅点击
- 仅在 `GameState.Preparation` 状态下允许放置
- 三层验证：网格范围 → 格子占用 → 电力充足
- 验证顺序经过优化：先做轻量检查（坐标转换、占用检测），最后做带副作用的操作（消耗电力、创建实例）

### 3. 弹丸伤害（Projectile）

- 沿 `Vector3.right` 方向飞行，使用 `_speed * dt` 控制速度
- 通过 `OnTriggerEnter2D` 检测碰撞
- **同跑道伤害检查**：`enemy.CurrentRow == _laneRow` 确保只伤害同跑道敌人
- **动态边界计算**：`GetRightBoundary()` 从 GridSystem 获取网格右边界，替代硬编码 10f
- 超出边界或命中后归还 ObjectPool（回退到 Destroy）

### 4. 射程检查（MarbleShooter）

- `HasTargetInRange()` 在每次攻击冷却完毕后调用
- Range == -1：无条件攻击（全跑道）
- Range > 0：使用 `Physics2D.OverlapBoxAll()` 沿跑道方向检测 Enemy
- 盒子检测范围：`(startX + rangeWorld/2, y)` 中心，`(rangeWorld, cellHeight/2)` 尺寸

### 5. ObjectPool 集成

| 对象 | 池名称 | 归还位置 |
|------|--------|----------|
| 弹丸 | `"MarbleProjectile"` | `Projectile.ReturnToPool()` / 超出边界 |
| 敌人 | `"Enemy"` | `Enemy.Die()` |

均采用 fallback 模式：对象池不可用时回退到 `Destroy()`。

## 事件系统

| 事件 | 发送者 | 用途 |
|------|--------|------|
| `OnDefenderSelected(DefenderData)` | UnitPlacement | UI 更新选中单位信息 |
| `OnDefenderPlaced(Defender)` | UnitPlacement | UI 确认放置成功 |
| `OnPlacementFailed(string)` | UnitPlacement | UI 显示错误提示 |
| `OnDefenderDead(Defender)` | Defender | 统计、UI 更新 |
| `OnPowerChanged(int, int)` | ResourceManager | UI 更新电力显示 |
| `OnEnemyDead(Enemy)` | Enemy | 统计、奖励计算 |

## 配置说明

### DefenderData 配置示例（弹珠射手）

| 参数 | 值 | 说明 |
|------|-----|------|
| Defender Name | 弹珠射手 | 单位名称 |
| Attack Power | 20 | 初始攻击力 |
| Max Health | 100 | 初始血量 |
| Attack Speed | 1 | 1 次/秒 |
| Range | -1 | 整条跑道 |
| Attack Type | Projectile | 弹射类 |
| Power Cost | 50 | 放置消耗 |
| Max Level | 12 | 最大等级 |

### ResourceManager 配置

| 参数 | 默认值 | 说明 |
|------|--------|------|
| Max Power | 200 | 最大电力上限 |
| Current Power | 200 | 初始电力（等于最大值） |

## 注意事项

### 已知限制

1. **射手无条件发射（Range=-1 时）**：不检测范围内是否有敌人，PvZ 风格设计，确保视觉上有持续弹丸发射效果。Range > 0 时有正常射程限制。

2. **弹丸仅检测触发碰撞**：需要为敌人和弹丸设置 2D Collider + 对应 Tag（"Enemy"），才能正常触发命中检测。

3. **UI 集成待后续步骤**：`SelectDefender()` 方法已提供，但卡牌选择 UI 由后续步骤实现。

4. **敌人对象池需预配置**：`Enemy.Die()` 归还到 `"Enemy"` 对象池，需要在 ObjectPool 中配置对应的 PoolConfig。

### 性能考量

- `MarbleShooter.HasTargetInRange()` 每帧最多调用 `AttackSpeed` 次（1次/秒），`Physics2D.OverlapBoxAll` 开销可控
- 弹丸使用对象池避免 GC 分配
- 敌人推荐使用对象池减少 Instantiate/Destroy 开销

### 后续优化方向

- 添加射程可视化指示器（放置时显示范围预览）
- 支持拖拽放置（当前仅支持点击）
- 敌人波次系统整合（步骤5）
- 单位升级系统（后续步骤）
