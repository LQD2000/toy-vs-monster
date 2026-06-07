## ADDED Requirements

### Requirement: 波次管理
游戏 SHALL 按波次生成怪物，每关有多波怪物。

#### Scenario: 波次配置
- **WHEN** 开发者配置关卡
- **THEN** 通过 WaveConfig ScriptableObject 定义每波的怪物类型、数量、生成间隔

#### Scenario: 波次推进
- **WHEN** 当前波次的所有怪物被消灭或到达终点
- **THEN** 开始下一波次，生成新一批怪物

#### Scenario: 波次间隔
- **WHEN** 一波怪物结束
- **THEN** 等待配置的间隔时间后开始下一波

### Requirement: 胜负判定
游戏 SHALL 在特定条件下判定胜负。

#### Scenario: 胜利条件
- **WHEN** 最后一波所有怪物被消灭且无怪物在场上
- **THEN** 玩家胜利，触发 GameManager.WinGame()

#### Scenario: 失败条件
- **WHEN** 怪物到达跑道最左侧且该跑道小车已被消耗
- **THEN** 玩家失败，触发 GameManager.LoseGame()

### Requirement: 波次配置数据
每关 SHALL 有独立的波次配置。

#### Scenario: WaveConfig 字段
- **WHEN** 查看波次配置
- **THEN** 包含：波次列表、每波怪物类型、每波怪物数量、生成间隔、波次间间隔

#### Scenario: 1-1 关卡波次
- **WHEN** 配置 1-1 关卡
- **THEN** 简单波次配置：少量灰尘精灵，较长间隔
