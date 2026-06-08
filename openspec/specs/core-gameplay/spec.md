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

### Requirement: 单位放置
玩家 SHALL 能够在跑道的格子上放置玩具单位。

#### Scenario: 成功放置
- **WHEN** 玩家选择一个单位并点击跑道上的空格子
- **THEN** 单位被放置在该格子上，消耗对应电力

#### Scenario: 放置失败 - 资源不足
- **WHEN** 玩家电力不足以放置所选单位
- **THEN** 显示资源不足提示，单位不被放置

#### Scenario: 放置失败 - 位置占用
- **WHEN** 玩家尝试在已有单位的格子上放置
- **THEN** 显示位置已占用提示，单位不被放置

### Requirement: 单位数据定义
每个防守方单位 SHALL 有完整的属性数据定义。

#### Scenario: 单位属性
- **WHEN** 查看单位信息
- **THEN** 显示：名称、攻击力、血量、射程、攻速、电力消耗、攻击类型

#### Scenario: ScriptableObject 配置
- **WHEN** 开发者配置新单位
- **THEN** 通过 DefenderData ScriptableObject 定义单位属性

### Requirement: 防守方工厂
游戏 SHALL 提供工厂模式创建防守方实例。

#### Scenario: 根据数据创建实例
- **WHEN** 玩家放置单位
- **THEN** DefenderFactory 根据 DefenderData 创建对应的 GameObject 实例
