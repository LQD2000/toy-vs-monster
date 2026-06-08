## Why

实现玩具塔防游戏的第5步：进攻方系统（1-1 关卡必需）。

## What Changes

**进攻方系统（1-1 关卡必需）：**
- 创建 AttackerData ScriptableObject（怪物数据定义：血量、攻击、移速）
- 创建 DustSprite Prefab（灰尘精灵：基础怪物）
- 实现 AttackerMovement（怪物移动：从屏幕外走入，沿跑道向左）
- 实现 AttackerSpawner（怪物生成器，按时间轴生成）

## Non-goals

- 不涉及其他步骤的实现
- 不涉及整体游戏设计变更
