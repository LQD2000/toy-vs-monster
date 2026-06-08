## Why

实现玩具塔防游戏的第4步：防守方系统（1-1 关卡必需）。

## What Changes

**防守方系统（1-1 关卡必需）：**
- 创建 DefenderData ScriptableObject（单位数据定义：攻击、血量、费用）
- 创建 MarbleShooter Prefab（弹珠射手：弹射类攻击）
- 实现 DefenderFactory（防守方工厂，根据数据创建实例）
- 实现 UnitPlacement（点击放置逻辑、电力消耗、格子占用）

## Non-goals

- 不涉及其他步骤的实现
- 不涉及整体游戏设计变更
