## ADDED Requirements

### Requirement: 电力资源
关卡内 SHALL 使用电力作为放置单位的资源。

#### Scenario: 电力初始值
- **WHEN** 关卡开始
- **THEN** 玩家拥有初始电力值 50

#### Scenario: 电力消耗
- **WHEN** 玩家放置单位
- **THEN** 消耗对应电力值

#### Scenario: 电力不足
- **WHEN** 玩家电力不足以放置单位
- **THEN** 无法放置，显示资源不足提示

### Requirement: 场景固定产电
部分关卡 SHALL 有场景固定的产电单位，不可被玩家控制。

#### Scenario: 场景固定产电单位
- **WHEN** 关卡开始
- **THEN** 场景边缘有固定的产电单位（如小太阳宝宝）

#### Scenario: 产电单位移动限制
- **WHEN** 场景固定产电单位存在
- **THEN** 该单位在场景边缘随机漂移，永远不进入跑道格子内

#### Scenario: 电力产出
- **WHEN** 场景固定产电单位产出电力
- **THEN** 在当前位置生成电力球，玩家需点击收集

#### Scenario: 电力球存在时间
- **WHEN** 电力球生成
- **THEN** 存在10秒，未收集则消失

#### Scenario: 电力球收集
- **WHEN** 玩家点击电力球
- **THEN** 获得对应电力值
