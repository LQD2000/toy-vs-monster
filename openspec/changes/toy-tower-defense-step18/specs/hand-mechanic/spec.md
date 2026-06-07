## ADDED Requirements

### Requirement: 小手功能
玩家 SHALL 能够使用小手在关卡内移动已放置的玩具。

#### Scenario: 移动单位
- **WHEN** 玩家使用小手点击已放置的玩具，再点击目标格子
- **THEN** 玩具移动到目标格子

#### Scenario: 移动限制
- **WHEN** 目标格子已被占用
- **THEN** 移动失败，提示位置已占用

### Requirement: 小手冷却
小手 SHALL 有冷却时间机制。

#### Scenario: 冷却触发
- **WHEN** 玩家使用小手移动单位
- **THEN** 小手进入冷却状态，冷却期间不可使用

#### Scenario: 冷却结束
- **WHEN** 冷却时间结束
- **THEN** 小手恢复可用状态

### Requirement: 小手策略价值
小手 SHALL 为游戏提供策略深度。

#### Scenario: 调整阵型
- **WHEN** 怪物路线变化
- **THEN** 玩家可用小手调整玩具位置应对新威胁

#### Scenario: 救援残血单位
- **WHEN** 某个玩具血量较低
- **THEN** 玩家可用小手将其移动到安全位置
