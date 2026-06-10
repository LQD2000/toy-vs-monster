# 弹珠射手 Prefab 创建指南

> **创建步骤**: Step 4 - 防守方系统（初始创建）
> **更新步骤**: Step 6 - 战斗系统（新增 Health + AttackComponent 组件）
> **规格文档**: `openspec/specs/combat-system/spec.md`
> **归档位置**: `openspec/changes/archive/2026-06-10-toy-tower-defense-step6/`

## 文件位置
`game/Assets/Prefabs/Defenders/MarbleShooter.prefab`

## 创建步骤

### 1. 在 Unity Editor 中创建

1. 在 Hierarchy 窗口右键 → `Create Empty`
2. 命名为 `MarbleShooter`
3. 设置 Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (1, 1, 1)

### 2. 添加组件

#### SpriteRenderer
- Sprite: 选择弹珠射手图片
- Color: White
- Sorting Layer: Default
- Order in Layer: 5

#### Health (Script)
- Max Health: 100
- 自动添加（通过 RequireComponent）

#### AttackComponent (Script)
- Projectile Prefab: 拖拽 `Marble.prefab` 到此字段
- Fire Point: 创建子 GameObject 作为发射点
- Projectile Pool Name: `MarbleProjectile`
- 自动添加（通过 RequireComponent）

#### MarbleShooter (Script)
- 继承自 Defender 基类
- 自动添加 Health 和 AttackComponent

#### Collider2D
- 类型: BoxCollider2D
- 用于点击选择检测

### 3. 创建发射点

1. 在 MarbleShooter 下创建空子 GameObject
2. 命名为 `FirePoint`
3. 设置位置（射手右侧）

### 4. 保存为 Prefab

1. 将 `MarbleShooter` GameObject 拖到 Project 窗口的 `Assets/Prefabs/Defenders/`
2. 删除 Hierarchy 中的实例

### 5. 配置 DefenderData

1. 在 Project 窗口右键 → `Create → Game → Defender Data`
2. 命名为 `MarbleShooterData`
3. 配置属性:
   - DefenderName: "弹珠射手"
   - AttackPower: 20
   - MaxHealth: 100
   - AttackSpeed: 1
   - Range: -1 (整条跑道)
   - AttackType: Projectile
   - PowerCost: 50
   - Prefab: 选择 `MarbleShooter.prefab`

## 属性参考

根据 `data/defenders/basic/marble-shooter.md`:

| 属性 | 值 | 说明 |
|------|-----|------|
| 攻击力 | 20 | 初始攻击力 |
| 血量 | 100 | 初始血量 |
| 攻速 | 1次/秒 | 攻击频率 |
| 射程 | -1 | 整条跑道 |
| 电力消耗 | 50 | 放置消耗 |
| 最大等级 | 12 | 最大等级 |

## 组件依赖关系

```
MarbleShooter (Defender)
├── [RequireComponent] Health
│   ├── Max Health: 100
│   └── OnDead 事件 → 触发死亡逻辑
├── [RequireComponent] AttackComponent
│   ├── Projectile Prefab: Marble.prefab
│   ├── Fire Point: FirePoint 子对象
│   └── Projectile Pool Name: MarbleProjectile
└── Defender 基类
    ├── Data: DefenderData
    └── Initialize() → 初始化所有组件
```

## 注意事项

- Prefab 必须在 Unity Editor 中创建，无法通过代码生成
- 确保 Sprite 图片已导入到 `game/Assets/Art/Sprites/Defenders/`
- 需要先创建 `Marble.prefab` 弹丸 Prefab
- Health 和 AttackComponent 会通过 RequireComponent 自动添加
- 配置完成后，在 DefenderFactory 中引用 MarbleShooterData
