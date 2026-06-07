## Requirements

### Requirement: 网格系统
游戏 SHALL 支持基于网格的跑道布局，每条跑道由 10 个格子组成，用于管理单位放置和位置计算。

#### Scenario: 网格初始化
- **WHEN** 关卡开始
- **THEN** 初始化 5 条跑道 × 10 列格子的网格数据，每个格子有唯一的世界坐标

#### Scenario: 格子占用检测
- **WHEN** 尝试在某个格子上放置单位
- **THEN** 系统返回该格子是否已被占用，防止重复放置

#### Scenario: 坐标转换
- **WHEN** 需要将世界坐标转换为网格坐标（或反向转换）
- **THEN** 系统正确计算对应的格子位置或世界位置

### Requirement: 跑道小车
每条跑道 SHALL 关联一辆小车，作为一次性防御机制，触发后清除整行。

#### Scenario: 小车注册
- **WHEN** 小车组件初始化
- **THEN** 小车自动注册到所属跑道的 LaneManager

#### Scenario: 小车触发
- **WHEN** 调用小车触发接口（怪物到达跑道末端时由外部调用）
- **THEN** 小车启动，清除该跑道上所有格子的占用，随后小车销毁

#### Scenario: 小车不可恢复
- **WHEN** 小车被触发后
- **THEN** 本关卡内该跑道小车不再存在，再次触发返回失败

### Requirement: 跑道可视化
游戏 SHALL 提供跑道网格的可视化显示，用于编辑器和运行时调试。

#### Scenario: 网格线绘制
- **WHEN** LaneRenderer 组件初始化
- **THEN** 使用 LineRenderer 绘制跑道网格线

#### Scenario: 小车位置标记
- **WHEN** 场景中有 LaneRenderer 和 LaneManager
- **THEN** 在编辑器中显示小车位置标记和跑道起点/终点
