## ADDED Requirements

### Requirement: 章节结构
游戏 SHALL 包含 5 个章节，每章 10 关。

#### Scenario: 章节主题
- **WHEN** 玩家进入不同章节
- **THEN** 场景主题分别为：卧室、客厅、阁楼、玩具店、游乐园

#### Scenario: 关卡解锁
- **WHEN** 玩家通关某关
- **THEN** 解锁下一关，章节最后一关通关后解锁下一章节

### Requirement: 难度曲线
关卡难度 SHALL 随章节和关卡递进。

#### Scenario: 每章入门关
- **WHEN** 玩家进入每章第 1 关
- **THEN** 难度简单，用于学习新场景机制

#### Scenario: 难度递增
- **WHEN** 玩家推进关卡
- **THEN** 怪物数量、种类、强度逐步增加

### Requirement: 怪物类型
不同章节 SHALL 引入不同类型的怪物。

#### Scenario: 普通怪物
- **WHEN** 关卡波次开始
- **THEN** 生成普通怪物（灰尘精灵、床底爬虫、蜘蛛网怪等）

#### Scenario: 精英怪物
- **WHEN** 特定波次或高难度关卡
- **THEN** 生成精英怪物（跳跳球怪、阴影幽灵、霉菌藤蔓等）

### Requirement: Boss 战
每章第 10 关 SHALL 为 Boss 关，包含黑暗玩具 Boss。

#### Scenario: Boss 出现
- **WHEN** 玩家进入每章第 10 关
- **THEN** 最后一波出现黑暗玩具 Boss，拥有独特技能和高血量

#### Scenario: Boss 击败
- **WHEN** 玩家击败 Boss
- **THEN** 获得额外奖励，解锁下一章节

### Requirement: 关卡结算
关卡结束 SHALL 显示结算界面。

#### Scenario: 胜利结算
- **WHEN** 关卡胜利
- **THEN** 显示获得的金币、材料、新解锁单位

#### Scenario: 失败结算
- **WHEN** 关卡失败
- **THEN** 显示获得的少量奖励，提供重试按钮
