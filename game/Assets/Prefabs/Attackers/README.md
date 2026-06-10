# DustSprite Prefab 创建指南

> **创建步骤**: Step 5 - 进攻方系统（初始创建）
> **更新步骤**: Step 6 - 战斗系统（新增 Health 组件）
> **规格文档**: `openspec/specs/combat-system/spec.md`
> **归档位置**: `openspec/changes/archive/2026-06-10-toy-tower-defense-step6/`

## 文件位置
`game/Assets/Prefabs/Attackers/DustSprite.prefab`

## 创建步骤

### 1. 在 Unity Editor 中创建

1. 在 Hierarchy 窗口右键 → `Create Empty`
2. 命名为 `DustSprite`
3. 设置 Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

### 2. 添加组件

#### SpriteRenderer
- Sprite: 选择灰尘精灵图片（或创建占位 Sprite）
- Color: White
- Sorting Layer: Default
- Order in Layer: 10

#### Health (Script)
- Max Health: 100
- 自动添加（通过 RequireComponent）

#### Attacker (Script)
- 自动添加，无需配置

#### AttackerMovement (Script)
- 自动添加，无需配置

#### Collider2D
- 类型: BoxCollider2D 或 CircleCollider2D
- 用于碰撞检测

### 3. 设置 Tag

1. 在 Inspector 顶部 Tag 下拉框
2. 选择 `Enemy` (如果没有，点击 Add Tag 创建)
3. **重要**: Projectile 通过 `CompareTag("Enemy")` 识别敌人

### 4. 保存为 Prefab

1. 将 `DustSprite` GameObject 拖到 Project 窗口的 `Assets/Prefabs/Attackers/` 文件夹
2. 删除 Hierarchy 中的实例

### 5. 配置 AttackerData

1. 在 Project 窗口右键 → `Create → Game → Attacker Data`
2. 命名为 `DustSpriteData`
3. 配置属性:
   - AttackerName: "灰尘精灵"
   - MaxHealth: 100
   - AttackPower: 10
   - MoveSpeed: 0.5
   - Prefab: 选择 `DustSprite.prefab`

## 属性参考

根据 `data/attackers/dust-sprite.md`:

| 属性 | 值 | 说明 |
|------|-----|------|
| 血量 | 100 | 基础血量 |
| 攻击力 | 10 | 基础攻击 |
| 移速 | 0.5 | 慢速（每2秒1格） |
| 攻速 | 1次/秒 | 攻击频率 |

## 组件依赖关系

```
DustSprite (Attacker)
├── [RequireComponent] Health
│   ├── Max Health: 100
│   └── OnDead 事件 → 回收到对象池
├── Attacker 基类
│   ├── TakeDamage() → 委托给 Health
│   └── HandleDeath() → ObjectPool.Return("Attacker")
├── AttackerMovement
│   ├── Move() → 左移
│   └── OnReachedEnd() → 触发突破
└── Tag: "Enemy"
```

## 对象池配置

在 ObjectPool 的 Inspector 中添加配置:
- Pool Name: `"Attacker"`
- Prefab: `DustSprite.prefab`
- Initial Size: 10
- Auto Expand: true

## 注意事项

- Prefab 必须在 Unity Editor 中 创建，无法通过代码生成
- 确保 Sprite 图片已导入到 `game/Assets/Art/Sprites/Attackers/`
- **必须设置 Tag 为 "Enemy"**，否则弹丸无法检测到敌人
- Health 组件会通过 RequireComponent 自动添加
- 配置完成后，在 AttackerSpawner 中引用 DustSpriteData
