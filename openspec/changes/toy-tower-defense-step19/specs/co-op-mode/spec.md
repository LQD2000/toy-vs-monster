## ADDED Requirements

### Requirement: 合作模式入口
游戏 SHALL 提供双人合作模式入口。

#### Scenario: 好友邀请
- **WHEN** 玩家选择好友邀请
- **THEN** 生成邀请码或直接邀请在线好友

#### Scenario: 随机匹配
- **WHEN** 玩家选择随机匹配
- **THEN** 系统匹配其他玩家，进入合作关卡

### Requirement: 资源共享
合作模式 SHALL 共享资源池。

#### Scenario: 共享电力
- **WHEN** 两名玩家进入合作关卡
- **THEN** 共享同一电力池，双方操作都消耗同一资源

#### Scenario: 共享单位
- **WHEN** 合作模式中
- **THEN** 双方可放置和移除对方的单位

### Requirement: 胜负判定
合作模式 SHALL 共享胜负条件。

#### Scenario: 合作胜利
- **WHEN** 最后一波怪物被消灭
- **THEN** 双方都胜利，都获得奖励

#### Scenario: 合作失败
- **WHEN** 任一跑道被突破
- **THEN** 双方都失败

### Requirement: 信任机制
随机匹配模式 SHALL 采用信任机制防止滥用。

#### Scenario: 好友模式
- **WHEN** 好友组队
- **THEN** 完全共享，无需确认

#### Scenario: 随机匹配模式
- **WHEN** 随机组队
- **THEN** 默认需要对方确认才能操作对方单位（可在设置中配置）

### Requirement: 合作奖励
合作模式 SHALL 提供额外奖励鼓励联机。

#### Scenario: 掉落加成
- **WHEN** 合作模式通关
- **THEN** 金币和材料掉落概率或比例提高
