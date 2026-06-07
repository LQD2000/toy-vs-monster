## Purpose

ObjectPool 提供对象池管理机制，用于管理弹丸、怪物、电力球等频繁创建/销毁的对象，减少 GC 压力，提升运行时性能。

## Requirements

### Requirement: 对象池创建
ObjectPool SHALL 支持在 Inspector 中配置多个对象池，并在初始化时预创建指定数量的对象。

#### Scenario: 初始化时预创建对象
- **WHEN** ObjectPool 初始化且配置中 initialSize = N
- **THEN** 池中预创建 N 个非激活状态的对象

#### Scenario: 运行时动态创建池
- **WHEN** 运行时调用 CreatePool 方法传入配置
- **THEN** 新池被创建，预创建指定数量的对象

#### Scenario: 重复创建同名池
- **WHEN** 调用 CreatePool 创建已存在的池名
- **THEN** 输出警告日志，不重复创建

### Requirement: 对象获取
ObjectPool SHALL 支持从池中获取对象，自动设置位置和旋转，并在池空时支持自动扩展。

#### Scenario: 从池中获取对象
- **WHEN** 调用 Get 方法传入存在的池名、位置和旋转
- **THEN** 返回一个激活状态的对象，位于指定位置和旋转

#### Scenario: 池空时自动扩展
- **WHEN** 池已空且 autoExpand = true 时调用 Get
- **THEN** 自动创建新对象并返回，输出扩展日志

#### Scenario: 池空且不自动扩展
- **WHEN** 池已空且 autoExpand = false 时调用 Get
- **THEN** 返回 null，输出警告日志

#### Scenario: 从不存在的池获取
- **WHEN** 调用 Get 传入不存在的池名
- **THEN** 返回 null，输出错误日志

### Requirement: 对象归还
ObjectPool SHALL 支持将对象归还到池中以便复用。

#### Scenario: 归还对象到池
- **WHEN** 调用 Return 方法传入正确的池名和对象
- **THEN** 对象变为非激活状态，重新设为 ObjectPool 的子对象，加入池队列

#### Scenario: 归还不存在的池
- **WHEN** 调用 Return 传入不存在的池名
- **THEN** 直接销毁对象，输出警告日志

### Requirement: 池清理
ObjectPool SHALL 支持清空指定池或所有池。

#### Scenario: 清空指定池
- **WHEN** 调用 ClearPool 传入存在的池名
- **THEN** 该池中所有对象被销毁，池队列清空

#### Scenario: 清空所有池
- **WHEN** 调用 ClearAllPools
- **THEN** 所有池的对象被销毁，池字典和配置字典被清空

#### Scenario: 获取池中对象数量
- **WHEN** 调用 GetCount 传入存在的池名
- **THEN** 返回池队列中的对象数量

### Requirement: 单例模式
ObjectPool SHALL 保证全局唯一实例。

#### Scenario: 重复创建时销毁新实例
- **WHEN** 场景中已存在 ObjectPool 实例时再次创建
- **THEN** 新实例被销毁，原实例保留

### Requirement: 跨场景持久化
ObjectPool SHALL 在场景切换时保持存活。

#### Scenario: 场景加载不销毁 ObjectPool
- **WHEN** 加载新场景
- **THEN** ObjectPool 实例仍然存在
