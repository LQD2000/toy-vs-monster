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

### Requirement: 产电玩具
玩家 SHALL 能够放置产电玩具来持续获取电力。

#### Scenario: 产电玩具放置
- **WHEN** 玩家放置产电玩具
- **THEN** 消耗电力，产电玩具开始持续产出电力

#### Scenario: 产电周期
- **WHEN** 产电玩具存活
- **THEN** 每隔固定时间产出一定量电力

### Requirement: 太阳能类产电
太阳能类产电玩具 SHALL 在特定场景有不同表现。

#### Scenario: 卧室场景太阳能
- **WHEN** 在卧室场景放置窗台太阳能板
- **THEN** 稳定产出电力，产出效率中等

#### Scenario: 客厅场景太阳能
- **WHEN** 在客厅场景放置落地窗太阳能
- **THEN** 产出效率高，但只能放在特定位置

### Requirement: 风力类产电
风力类产电玩具 SHALL 具有特殊机制。

#### Scenario: 风力产电效果
- **WHEN** 放置风力产电玩具
- **THEN** 产出电力的同时可能附带击退小怪效果

### Requirement: 热能类产电
热能类产电玩具 SHALL 具有特殊机制。

#### Scenario: 热能产电效果
- **WHEN** 放置热能产电玩具
- **THEN** 产出电力，可能附带范围增益效果

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

### Requirement: 场景差异化
不同场景 SHALL 有不同的产电玩具选择和机制。

#### Scenario: 场景产电玩具切换
- **WHEN** 玩家进入不同章节的关卡
- **THEN** 可用的产电玩具类型和效果不同
