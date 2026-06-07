## Purpose

EventSystem 提供事件发布/订阅机制，实现模块间解耦通信。支持无参数和带参数事件、异常隔离、事件清理等功能。

## Requirements

### Requirement: 事件订阅
EventSystem SHALL 支持无参数和带参数的事件订阅机制，实现模块间解耦通信。

#### Scenario: 订阅无参数事件
- **WHEN** 调用 On 方法订阅某个事件名
- **THEN** 回调被注册到该事件名对应的列表中

#### Scenario: 订阅带参数事件
- **WHEN** 调用 On<T> 方法订阅某个事件名并提供泛型回调
- **THEN** 回调被注册到该事件名对应的列表中

### Requirement: 事件触发
EventSystem SHALL 支持触发已订阅的事件，并传递可选参数。

#### Scenario: 触发无参数事件
- **WHEN** 调用 Emit 方法触发已订阅的事件名
- **THEN** 所有注册到该事件名的无参数回调依次执行

#### Scenario: 触发带参数事件
- **WHEN** 调用 Emit<T> 方法触发已订阅的事件名并传入参数
- **THEN** 所有注册到该事件名的泛型回调依次执行，且收到正确的参数值

#### Scenario: 触发未订阅的事件
- **WHEN** 调用 Emit 触发一个没有任何订阅者的事件名
- **THEN** 不抛出异常，静默无操作

### Requirement: 取消订阅
EventSystem SHALL 支持取消已订阅的事件回调。

#### Scenario: 取消无参数事件订阅
- **WHEN** 调用 Off 方法取消之前订阅的回调
- **THEN** 该回调从事件表中移除，不再被触发

#### Scenario: 取消订阅后事件表清理
- **WHEN** 某个事件名的最后一个订阅者被取消
- **THEN** 该事件名从事件字典中完全移除

#### Scenario: 取消未订阅的回调
- **WHEN** 调用 Off 取消一个从未订阅过的回调
- **THEN** 不抛出异常

### Requirement: 异常隔离
EventSystem SHALL 确保单个回调的异常不影响其他回调的执行。

#### Scenario: 回调异常不影响其他订阅者
- **WHEN** 事件触发时某个回调抛出异常
- **THEN** 其他回调继续正常执行，异常被捕获并记录错误日志

### Requirement: 事件清理
EventSystem SHALL 支持清除指定事件或所有事件的订阅。

#### Scenario: 清除指定事件
- **WHEN** 调用 Clear 方法清除某个事件名
- **THEN** 该事件的所有订阅被移除

#### Scenario: 清除所有事件
- **WHEN** 调用 ClearAll 方法
- **THEN** 所有事件的所有订阅被移除，事件表清空

### Requirement: 单例模式
EventSystem SHALL 保证全局唯一实例。

#### Scenario: 重复创建时销毁新实例
- **WHEN** 场景中已存在 EventSystem 实例时再次创建
- **THEN** 新实例被销毁，原实例保留

### Requirement: 跨场景持久化
EventSystem SHALL 在场景切换时保持存活。

#### Scenario: 场景加载不销毁 EventSystem
- **WHEN** 加载新场景
- **THEN** EventSystem 实例仍然存在
