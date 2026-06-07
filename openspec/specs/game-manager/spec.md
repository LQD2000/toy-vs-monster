## Purpose

GameManager 负责管理游戏全局状态，提供状态转换机制和事件通知，确保游戏在不同阶段（准备、进行中、胜利、失败）之间正确流转。

## Requirements

### Requirement: 游戏状态管理
GameManager SHALL 管理游戏全局状态，支持四种状态（准备、进行中、胜利、失败），并提供状态转换和事件通知。

#### Scenario: 游戏从准备阶段开始
- **WHEN** GameManager 初始化
- **THEN** 当前状态为 Preparation

#### Scenario: 开始游戏
- **WHEN** 玩家触发开始游戏且当前状态为 Preparation
- **THEN** 状态切换为 InProgress，触发 OnGameStateChanged 事件

#### Scenario: 游戏胜利
- **WHEN** 达成胜利条件且当前状态为 InProgress
- **THEN** 状态切换为 Victory，触发 OnGameStateChanged 事件

#### Scenario: 游戏失败
- **WHEN** 达成失败条件且当前状态为 InProgress
- **THEN** 状态切换为 Defeat，触发 OnGameStateChanged 事件

#### Scenario: 无效的状态转换 - 非准备阶段开始游戏
- **WHEN** 当前状态不是 Preparation 时调用 StartGame
- **THEN** 状态不变，输出警告日志

#### Scenario: 无效的状态转换 - 非进行中阶段胜利
- **WHEN** 当前状态不是 InProgress 时调用 WinGame
- **THEN** 状态不变，输出警告日志

#### Scenario: 无效的状态转换 - 非进行中阶段失败
- **WHEN** 当前状态不是 InProgress 时调用 LoseGame
- **THEN** 状态不变，输出警告日志

#### Scenario: 重置游戏
- **WHEN** 调用 ResetGame
- **THEN** 状态切换到 Preparation

#### Scenario: 相同状态不触发事件
- **WHEN** 设置与当前相同的状态
- **THEN** OnGameStateChanged 事件不被触发，状态不变

### Requirement: 跨场景持久化
GameManager SHALL 在场景切换时保持存活，不被销毁。

#### Scenario: 场景加载不销毁 GameManager
- **WHEN** 加载新场景
- **THEN** GameManager 实例仍然存在

### Requirement: 单例模式
GameManager SHALL 保证全局唯一实例。

#### Scenario: 重复创建时销毁新实例
- **WHEN** 场景中已存在 GameManager 实例时再次创建
- **THEN** 新实例被销毁，原实例保留
