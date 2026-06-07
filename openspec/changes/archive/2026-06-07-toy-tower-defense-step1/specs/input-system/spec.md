## ADDED Requirements

### Requirement: 输入系统配置
项目 SHALL 使用 Unity New Input System 替代旧版 Input Manager，并创建基本的输入动作映射。

#### Scenario: 旧版 Input Manager 已禁用
- **WHEN** 查看 Project Settings → Player → Active Input Handling
- **THEN** 设置为 "Input System Package (New)" 或 "Both"

### Requirement: PlayerControls 输入动作资源
项目 SHALL 创建 PlayerControls.inputactions 资源，定义基本输入动作。

#### Scenario: PlayerControls 资源存在
- **WHEN** 查看 Assets/Input/ 目录
- **THEN** 存在 PlayerControls.inputactions 文件

#### Scenario: Gameplay Action Map 存在
- **WHEN** 在 Input Actions 编辑器中打开 PlayerControls
- **THEN** 存在名为 Gameplay 的 Action Map

### Requirement: Tap 触控动作
PlayerControls SHALL 定义 Tap 触控动作，支持点击/触摸输入。

#### Scenario: Tap 动作定义
- **WHEN** 查看 Gameplay Action Map
- **THEN** 存在 Tap 动作，绑定到 Pointer → Press 和 Touchscreen → Primary Touch → Press

#### Scenario: Tap 动作可用代码访问
- **WHEN** 在 C# 脚本中引用 PlayerControls
- **THEN** 可通过 `_controls.Gameplay.Tap` 访问 Tap 动作

### Requirement: TapPosition 触控位置
PlayerControls SHALL 定义 TapPosition 动作，获取点击/触摸的屏幕坐标。

#### Scenario: TapPosition 动作定义
- **WHEN** 查看 Gameplay Action Map
- **THEN** 存在 TapPosition 动作，绑定到 Pointer → Position 和 Touchscreen → Primary Touch → Position

#### Scenario: TapPosition 返回 Vector2 坐标
- **WHEN** 在 Tap 事件回调中调用 ReadValue<T>()
- **THEN** 返回的 Vector2 值为屏幕坐标

### Requirement: 输入动作 C# 类生成
PlayerControls.inputactions SHALL 启用 "Generate C# Class" 以便脚本访问。

#### Scenario: PlayerControls.cs 已生成
- **WHEN** 查看 Assets/Input/ 目录
- **THEN** 存在 PlayerControls.cs 文件

#### Scenario: 脚本可实例化 PlayerControls
- **WHEN** 在 C# MonoBehaviour 中 new PlayerControls()
- **THEN** 可成功创建实例并启用/禁用输入动作

### Requirement: 输入动作生命周期
使用 PlayerControls 的 MonoBehaviour SHALL 正确管理输入动作的启用和禁用。

#### Scenario: OnEnable 中启用输入
- **WHEN** MonoBehaviour 被启用
- **THEN** PlayerControls 实例被 Enable 并订阅动作回调

#### Scenario: OnDisable 中禁用输入
- **WHEN** MonoBehaviour 被禁用
- **THEN** 动作回调被取消订阅，PlayerControls 实例被 Disable
