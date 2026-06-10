# Step 6 - 战斗系统实现文档

> **对应步骤**: toy-tower-defense-step6
> **归档位置**: `openspec/changes/archive/2026-06-10-toy-tower-defense-step6/`
> **规格文档**: `openspec/specs/combat-system/spec.md`

## 模块概述

实现玩具塔防游戏的战斗系统，包括弹丸发射、伤害计算、血量管理和攻击组件。

## 类图/结构图

```
┌─────────────────────────────────────────────────────────────┐
│                      战斗系统架构                            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────┐      ┌──────────────┐      ┌──────────┐ │
│  │   Projectile │─────▶│DamageCalculator│    │  Health  │ │
│  │   (弹丸)     │      │  (伤害计算)    │    │ (血量)   │ │
│  └──────────────┘      └──────────────┘      └──────────┘ │
│         │                                          │       │
│         │ OnHit                                    │ OnDead│
│         ▼                                          ▼       │
│  ┌──────────────┐                          ┌──────────┐   │
│  │    Enemy     │◀──── TakeDamage ────────│ ObjectPool│   │
│  │   (敌人)     │                          │  (对象池) │   │
│  └──────────────┘                          └──────────┘   │
│                                                             │
│  ┌──────────────┐      ┌──────────────┐                    │
│  │  Attacker    │      │   Defender   │                    │
│  │  (进攻方)    │      │   (防守方)   │                    │
│  └──────────────┘      └──────────────┘                    │
│         │                      │                           │
│         └──────┬───────────────┘                           │
│                │                                           │
│                ▼                                           │
│       ┌──────────────┐                                    │
│       │AttackComponent│                                    │
│       │  (攻击组件)   │                                    │
│       └──────────────┘                                    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## 关键实现

### 1. Projectile（弹珠弹丸）

**文件**: `game/Assets/Scripts/Gameplay/Projectile.cs`

- 沿跑道直线飞行（`Vector3.right * speed * Time.deltaTime`）
- 命中敌人后调用 `DamageCalculator.CalculateDamage()` 计算伤害
- 使用对象池回收（`ObjectPool.Return("MarbleProjectile")`）

```csharp
// 伤害计算集成
DamageCalculator.DamageResult result = DamageCalculator.CalculateDamage(_damage);
enemy.TakeDamage(result.finalDamage);
```

### 2. DamageCalculator（伤害计算器）

**文件**: `game/Assets/Scripts/Gameplay/DamageCalculator.cs`

静态工具类，提供统一的伤害计算逻辑：

- **基础伤害**: `baseAttack × typeMultiplier × critMultiplier - defense`
- **暴击判定**: 10% 概率触发，2.0x 倍率
- **爆炸伤害**: 距离衰减公式

```csharp
public static DamageResult CalculateDamage(
    int baseAttack,
    int defense = 0,
    float typeMultiplier = 1.0f,
    float critChance = -1f)
```

### 3. Health（血量组件）

**文件**: `game/Assets/Scripts/Gameplay/Health.cs`

可复用的血量管理组件，通过 `RequireComponent` 自动添加到 Enemy/Attacker/Defender：

- `TakeDamage(int damage)` - 扣血
- `Heal(int amount)` - 回血
- `OnHealthChanged` - 血量变化事件
- `OnDead` - 死亡事件

### 4. AttackComponent（攻击组件）

**文件**: `game/Assets/Scripts/Gameplay/AttackComponent.cs`

通用攻击逻辑组件，可挂载到任意防御者：

- `HasTargetInRange()` - 检测范围内敌人
- `TryAttack()` - 发射弹丸
- 冷却计时器（`1f / attackSpeed`）

## 组件依赖关系

### Enemy（敌人）

```
Enemy
├── [RequireComponent] Health
│   └── OnDead → ObjectPool.Return("Enemy")
└── TakeDamage() → Health.TakeDamage()
```

### Attacker（进攻方）

```
Attacker
├── [RequireComponent] Health
│   └── OnDead → ObjectPool.Return("Attacker")
├── AttackerMovement
└── TakeDamage() → Health.TakeDamage()
```

### Defender（防守方）

```
Defender (abstract)
├── [RequireComponent] Health
│   └── OnDead → ObjectPool.Return("Defender")
└── TakeDamage() → Health.TakeDamage()
```

### MarbleShooter（弹珠射手）

```
MarbleShooter : Defender
├── [RequireComponent] AttackComponent
│   ├── HasTargetInRange()
│   └── TryAttack() → ObjectPool.Get("MarbleProjectile")
└── Initialize() → AttackComponent.Initialize()
```

## 配置说明

### ObjectPool 配置

| 池名称 | Prefab | 初始大小 | 自动扩展 |
|--------|--------|---------|---------|
| MarbleProjectile | Marble.prefab | 20 | ✓ |
| Enemy | DustSprite.prefab | 10 | ✓ |
| Attacker | DustSprite.prefab | 10 | ✓ |
| Defender | MarbleShooter.prefab | 5 | ✓ |

### Unity Tags

- `"Enemy"` - Projectile 和 AttackComponent 通过此 Tag 检测敌人

## Unity Editor 手动步骤

### 需要创建的 Prefab

| Prefab | 创建步骤 | 更新步骤 | README |
|--------|---------|---------|--------|
| Marble.prefab | Step 6 | - | `game/Assets/Prefabs/Projectiles/README.md` |
| DustSprite.prefab | Step 5 | Step 6 | `game/Assets/Prefabs/Attackers/README.md` |
| MarbleShooter.prefab | Step 4 | Step 6 | `game/Assets/Prefabs/Defenders/README.md` |

### 需要配置的组件

| 组件 | 配置步骤 | README |
|------|---------|--------|
| ObjectPool | Step 2, 更新 Step 6 | `game/Assets/Prefabs/ObjectPool-README.md` |

## 使用示例

### 发射弹丸

```csharp
// AttackComponent.TryAttack()
GameObject projectileObj = ObjectPool.Instance.Get(
    _projectilePoolName,
    spawnPosition,
    Quaternion.identity
);

Projectile projectile = projectileObj.GetComponent<Projectile>();
projectile.Initialize(_attackPower, _currentRow);
```

### 造成伤害

```csharp
// Projectile.OnTriggerEnter2D()
DamageCalculator.DamageResult result = DamageCalculator.CalculateDamage(_damage);
enemy.TakeDamage(result.finalDamage);
```

### 单位死亡

```csharp
// Health.Die()
_isDead = true;
OnDead?.Invoke();

// Enemy/Attacker/Defender 订阅 OnDead 事件
ObjectPool.Instance.Return("Enemy", gameObject);
```

## 注意事项

- `RequireComponent` 在 Edit Mode 测试中可能不会自动添加组件，需要手动添加
- `Initialize()` 方法使用懒加载模式，不依赖 `Awake()` 先被调用
- 对象池名称必须与代码中的字符串完全一致
- 弹丸需要 Rigidbody2D (Kinematic) 才能触发 OnTriggerEnter2D

## 后续优化方向

- 实现类型克制系统（DamageCalculator.GetTypeMultiplier）
- 实现死亡动画播放
- 优化敌人检测（避免 FindGameObjectsWithTag）
- 添加伤害数字显示
