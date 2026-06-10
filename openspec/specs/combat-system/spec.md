## ADDED Requirements

### Requirement: 攻击类型 - 弹射类
弹射类玩具 SHALL 发射直线飞行的弹丸攻击单个敌人。

#### Scenario: 弹射攻击
- **WHEN** 弹射类玩具检测到范围内敌人
- **THEN** 发射弹丸，直线飞行，命中造成单体伤害

#### Scenario: 弹丸飞行
- **WHEN** 弹丸被发射
- **THEN** 弹丸沿直线飞行，命中敌人后销毁

### Requirement: 伤害计算
游戏 SHALL 有完整的伤害计算公式。

#### Scenario: 伤害公式
- **WHEN** 攻击命中敌人
- **THEN** 最终伤害 = 基础攻击力 × 类型克制系数 - 敌人防御力

#### Scenario: 暴击判定
- **WHEN** 攻击命中
- **THEN** 有概率触发暴击，造成额外伤害

### Requirement: 血量系统
游戏 SHALL 有血量管理系统。

#### Scenario: 受伤
- **WHEN** 单位受到伤害
- **THEN** 扣除对应血量，血量归零时单位死亡

#### Scenario: 死亡
- **WHEN** 单位血量归零
- **THEN** 播放死亡动画，回收对象到对象池
