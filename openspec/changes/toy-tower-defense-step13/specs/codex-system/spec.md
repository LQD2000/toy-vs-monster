## ADDED Requirements

### Requirement: 图鉴管理
游戏 SHALL 提供图鉴系统，记录已遇到的进攻方和已获得的防守方。

#### Scenario: 图鉴数据存储
- **WHEN** 玩家解锁新图鉴条目
- **THEN** 解锁状态保存到 JSON 文件

#### Scenario: 图鉴数据读取
- **WHEN** 玩家进入图鉴界面
- **THEN** 从 JSON 文件读取解锁状态

### Requirement: 进攻方图鉴
游戏 SHALL 记录玩家遇到的进攻方怪物。

#### Scenario: 解锁进攻方图鉴
- **WHEN** 玩家在关卡中首次遇到某种进攻方怪物
- **THEN** 解锁该怪物的图鉴条目

#### Scenario: 进攻方图鉴内容
- **WHEN** 查看已解锁的进攻方图鉴
- **THEN** 显示：怪物名称、图片、描述、属性

### Requirement: 防守方图鉴
游戏 SHALL 记录玩家获得的防守方单位。

#### Scenario: 解锁防守方图鉴
- **WHEN** 玩家首次获得某种防守方单位
- **THEN** 解锁该单位的图鉴条目

#### Scenario: 默认解锁
- **WHEN** 玩家开始新游戏
- **THEN** 默认解锁弹珠射手图鉴

#### Scenario: 防守方图鉴内容
- **WHEN** 查看已解锁的防守方图鉴
- **THEN** 显示：单位名称、图片、描述、属性

### Requirement: 局内图鉴提示
关卡内 SHALL 在首次遇到新进攻方时弹出提示，但不暂停游戏。

#### Scenario: 新怪物提示
- **WHEN** 关卡中首次出现新的进攻方类型
- **THEN** 左侧弹出该怪物的基本描述提示框

#### Scenario: 查看详情
- **WHEN** 玩家点击提示框
- **THEN** 显示该怪物的详细信息，但游戏不暂停

#### Scenario: 不暂停游戏
- **WHEN** 图鉴提示弹出或查看详情
- **THEN** 游戏继续进行，不暂停

### Requirement: 图鉴界面
游戏 SHALL 提供图鉴查看界面。

#### Scenario: 图鉴分类
- **WHEN** 玩家进入图鉴界面
- **THEN** 分为进攻方图鉴和防守方图鉴两个标签页

#### Scenario: 未解锁显示
- **WHEN** 查看未解锁的图鉴条目
- **THEN** 显示为灰色剪影，不显示详细信息
