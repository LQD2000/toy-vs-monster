## ADDED Requirements

### Requirement: 怪物数据定义
每个进攻方怪物 SHALL 有完整的属性数据定义。

#### Scenario: 怪物属性
- **WHEN** 查看怪物信息
- **THEN** 显示：名称、血量、攻击力、移速、特殊能力

#### Scenario: ScriptableObject 配置
- **WHEN** 开发者配置新怪物
- **THEN** 通过 AttackerData ScriptableObject 定义怪物属性

### Requirement: 怪物移动
怪物 SHALL 沿跑道从右向左移动。

#### Scenario: 怪物出生
- **WHEN** 怪物生成
- **THEN** 怪物从屏幕右侧外出现，缓缓走入格子10，然后沿跑道向左移动

#### Scenario: 持续移动
- **WHEN** 怪物存活
- **THEN** 怪物以配置的速度沿跑道向左移动

#### Scenario: 到达终点
- **WHEN** 怪物超过格子1左侧边界
- **THEN** 触发防线突破判定（调用 LaneManager.OnMonsterReachedEnd）

### Requirement: 怪物生成器
游戏 SHALL 提供怪物生成器，按配置生成怪物。

#### Scenario: 按时间轴生成
- **WHEN** 关卡进行中
- **THEN** AttackerSpawner 按配置的时间间隔生成怪物

#### Scenario: 生成位置
- **WHEN** 怪物生成
- **THEN** 怪物出现在指定跑道的屏幕右侧外

### Requirement: 基础怪物 - 灰尘精灵
游戏 SHALL 包含基础怪物灰尘精灵。

#### Scenario: 灰尘精灵属性
- **WHEN** 查看灰尘精灵
- **THEN** 为基础怪物，血量低、移速慢、无特殊能力

#### Scenario: 灰尘精灵 Prefab
- **WHEN** 开发者创建灰尘精灵
- **THEN** 使用 DustSprite Prefab，挂载 AttackerMovement 组件
