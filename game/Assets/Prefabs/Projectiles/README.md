# 弹丸 Prefab 创建指南

## 文件位置
`game/Assets/Prefabs/Projectiles/Marble.prefab`

## 创建步骤

### 1. 在 Unity Editor 中创建

1. 在 Hierarchy 窗口右键 → `Create Empty`
2. 命名为 `Marble`
3. 设置 Transform:
   - Position: (0, 0, 0)
   - Rotation: (0, 0, 0)
   - Scale: (0.3, 0.3, 1) (弹丸较小)

### 2. 添加组件

#### SpriteRenderer
- Sprite: 选择弹珠图片（圆形）
- Color: White
- Sorting Layer: Default
- Order in Layer: 15

#### Projectile (Script)
- Speed: 10 (飞行速度)
- Damage: 20 (伤害值，运行时由 MarbleShooter 设置)

#### CircleCollider2D
- Radius: 0.15
- Is Trigger: ✓ (勾选)

#### Rigidbody2D
- Body Type: Kinematic
- 用于触发碰撞检测

### 3. 设置 Tag

1. 在 Inspector 顶部 Tag 下拉框
2. 选择 `Projectile` (如果没有，点击 Add Tag 创建)

### 4. 保存为 Prefab

1. 将 `Marble` GameObject 拖到 Project 窗口的 `Assets/Prefabs/Projectiles/`
2. 删除 Hierarchy 中的实例

## 属性参考

| 属性 | 值 | 说明 |
|------|-----|------|
| Speed | 10 | 飞行速度 |
| Damage | 20 | 伤害值（运行时设置） |
| Collider | CircleCollider2D | 碰撞检测 |
| Is Trigger | true | 触发器模式 |

## 对象池配置

在 ObjectPool 的 Inspector 中添加配置:
- Pool Name: `"MarbleProjectile"`
- Prefab: `Marble.prefab`
- Initial Size: 20
- Auto Expand: true

## 注意事项

- Prefab 必须在 Unity Editor 中创建，无法通过代码生成
- 确保 Sprite 图片已导入到 `game/Assets/Art/Sprites/Projectiles/`
- 弹丸需要 Rigidbody2D (Kinematic) 才能触发 OnTriggerEnter2D
- Tag 必须设置为 `Projectile` 才能被碰撞检测识别
