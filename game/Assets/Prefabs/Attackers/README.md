# DustSprite Prefab 创建指南

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

#### Attacker (Script)
- 自动添加，无需配置

#### AttackerMovement (Script)
- 自动添加，无需配置

#### Collider2D
- 类型: BoxCollider2D 或 CircleCollider2D
- 用于碰撞检测

### 3. 保存为 Prefab

1. 将 `DustSprite` GameObject 拖到 Project 窗口的 `Assets/Prefabs/Attackers/` 文件夹
2. 删除 Hierarchy 中的实例

### 4. 配置 AttackerData

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

## 注意事项

- Prefab 必须在 Unity Editor 中创建，无法通过代码生成
- 确保 Sprite 图片已导入到 `game/Assets/Art/Sprites/Attackers/`
- 配置完成后，在 AttackerSpawner 中引用 DustSpriteData
