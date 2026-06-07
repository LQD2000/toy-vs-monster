## ADDED Requirements

### Requirement: 场景加载
游戏 SHALL 支持异步场景加载和场景切换。

#### Scenario: 异步加载
- **WHEN** 需要切换场景
- **THEN** SceneLoader 异步加载目标场景，显示加载进度

#### Scenario: 场景过渡
- **WHEN** 场景切换
- **THEN** 播放过渡动画（如淡入淡出）

### Requirement: 主菜单
游戏 SHALL 提供主菜单界面。

#### Scenario: 主菜单选项
- **WHEN** 玩家进入主菜单
- **THEN** 显示：开始游戏、设置、退出

#### Scenario: 进入关卡选择
- **WHEN** 玩家点击开始游戏
- **THEN** 进入关卡选择界面

### Requirement: 关卡选择界面
游戏 SHALL 提供关卡选择界面，展示各章节和关卡。

#### Scenario: 章节显示
- **WHEN** 玩家进入关卡选择
- **THEN** 显示 5 个章节，每章 10 关

#### Scenario: 关卡解锁
- **WHEN** 玩家通关某关
- **THEN** 解锁下一关，若为章节最后一关则解锁下一章节第 1 关

#### Scenario: 已通关标记
- **WHEN** 玩家查看关卡列表
- **THEN** 已通关关卡显示完成标记

### Requirement: 关卡解锁逻辑
游戏 SHALL 持久化存储关卡解锁状态。

#### Scenario: 解锁状态存储
- **WHEN** 玩家通关
- **THEN** 解锁状态保存到 JSON 文件

#### Scenario: 解锁状态读取
- **WHEN** 玩家进入关卡选择
- **THEN** 从 JSON 文件读取解锁状态，显示可玩关卡
