## ADDED Requirements

### Requirement: 单位数据定义
每个防守方单位 SHALL 有完整的属性数据定义。

#### Scenario: 单位属性
- **WHEN** 查看单位信息
- **THEN** 显示：名称、攻击力、血量、射程、攻速、电力消耗、攻击类型

#### Scenario: ScriptableObject 配置
- **WHEN** 开发者配置新单位
- **THEN** 通过 DefenderData ScriptableObject 定义单位属性
