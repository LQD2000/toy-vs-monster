# 弹珠射手 Prefab 创建指南

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

#### MarbleShooter (Script)
- 继承自 Defender 基类
- Projectile Prefab: 拖拽 `Marble.prefab` 到此字段
- Fire Point: 创建子 GameObject 作为发射点

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

## 注意事项

- Prefab 必须在 Unity Editor 中创建，无法通过代码生成
- 确保 Sprite 图片已导入到 `game/Assets/Art/Sprites/Defenders/`
- 需要先创建 `Marble.prefab` 弹丸 Prefab
- 配置完成后，在 DefenderFactory 中引用 MarbleShooterData
