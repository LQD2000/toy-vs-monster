## ADDED Requirements

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
