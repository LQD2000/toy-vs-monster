## Context

全新项目，目标是开发一款玩具题材的 2D 跑道式塔防游戏。技术栈从 Python 切换为 Unity + C#，以获得更好的跨平台支持、联网能力和开发效率。

当前状态：项目从零开始，无已有代码。游戏设计已完成初步讨论，确定了核心玩法框架。

约束条件：
- 2D 游戏，5-6 条固定跑道
- 单人闯关 + 双人合作模式
- 局外养成系统（升级/升阶/分支进化）
- 不需要主角参与战斗
- 需要跨平台支持（PC、移动端）

## Goals / Non-Goals

**Goals:**
- 搭建可运行的塔防游戏原型
- 实现完整的单位放置、战斗、波次系统
- 实现局外升级升阶养成系统
- 实现双人合作联机模式
- 代码结构清晰，便于后续扩展
- 支持多平台发布

**Non-Goals:**
- 不实现 3D 渲染（纯 2D 游戏）
- 不实现对战模式（PvP）
- 不实现主角系统
- 不实现黑市商人
- 不实现黑暗玩具净化系统
- 不追求精美美术（先用占位资源）

## Decisions

### 1. 游戏引擎选择

**选择：Unity 6.3 LTS (6000.3.x)**

理由：
- 当前最新 LTS 版本（2026年6月）
- 支持周期长（至少到2028年）
- 性能最优化（多线程、内存、渲染改进）
- 2D 工具最完善（Sprite、Tilemap、2D Physics）
- 一键打包多平台（Windows/Mac/Linux/iOS/Android/Web）
- 成熟的联网框架（Mirror）
- 内置物理引擎（Box2D）、动画系统（Animator）、UI 系统（UI Toolkit）
- 强大的编辑器和调试工具
- 庞大的社区和 Asset Store 资源

备选方案：
- Unity 2022 LTS：即将结束支持，不推荐新项目
- Godot：开源免费，但社区较小，跨平台支持不如 Unity
- Unreal Engine：功能强大，但对 2D 游戏来说过重

### 2. 编程语言

**选择：C#**

理由：
- Unity 原生支持
- 强类型语言，适合大型项目
- 丰富的库和框架支持

### 3. 联网方案

**选择：Mirror Networking**

理由：
- Unity 最流行的开源联网框架
- 社区活跃，文档完善
- 支持 Client-Server 和 Host 模式
- 内置同步机制（SyncVar、RPC）

备选方案：
- Netcode for GameObjects：Unity 官方方案，但较新，社区资源少
- Photon：商业方案，需要付费

### 4. 数据存储

**选择：ScriptableObject + JSON + PlayerPrefs/SQLite**

理由：
- 单位/关卡配置用 ScriptableObject，编辑器内可视化编辑
- 游戏数据用 JSON，便于版本控制
- 玩家存档用 PlayerPrefs（简单）或 SQLite（复杂查询）

### 5. UI 方案

**选择：UI Toolkit（Unity 新 UI 系统）**

理由：
- 基于 USS/CSS，样式分离
- 支持响应式布局
- 性能优于 UGUI
- 适合复杂 UI 界面

备选方案：
- UGUI：成熟稳定，但代码和样式耦合

### 6. 组件架构

**选择：MonoBehaviour + 组件化设计**

理由：
- 游戏实体规模适中（150-200个），MonoBehaviour 性能足够
- 开发效率高，代码量少，调试简单
- 与 Unity 内置系统（UI、Physics、Animation）原生集成
- 学习成本低，社区资源丰富
- 后续可用对象池、Job System 局部优化

备选方案：
- ECS/DOTS：适合万级实体的大规模游戏，对本项目过重

### 7. 对象池策略

**选择：对象池（Object Pool）管理频繁创建销毁的对象**

适用对象：
- 弹丸（Projectile）：频繁发射和销毁
- 怪物（Attacker）：波次生成和击杀
- 电力球（PowerOrb）：定期生成和收集
- 特效（VFX）：爆炸、增益等

理由：
- 减少 GC 开销，避免频繁 Instantiate/Destroy
- 提升性能，稳定帧率
- Unity 2021+ 内置 ObjectPool<T> 类，实现简单

### 8. 项目架构

**选择：组件化架构 + ScriptableObject 数据驱动**

理由：
- 利用 Unity 的 Component 系统
- ScriptableObject 作为配置数据，便于编辑和复解耦
- Manager 类管理全局状态（GameManager、LevelManager）

### 9. 渲染方式

**选择：2.5D（类似 PvZ 视角）**

理由：
- 玩家熟悉这种视角，更接近 PvZ 的游戏体验
- 视觉效果更好，有层次感和深度
- Unity 的 Sorting Layer 可以轻松实现

实现方式：
- 相机：正交相机（Orthographic）或轻微透视角度
- 角色：2D 精灵图（Sprite），等距视角绘制
- 排序：Sorting Layer 控制前后顺序
  - Background（最远）
  - Lane（跑道）
  - Defender（防守方）
  - Attacker（进攻方）
  - Foreground（最近）
  - UI（最上层）
- 深度：Order in Layer，Y 值越大排序越前
- 阴影：添加阴影精灵营造立体感
- 背景：视差滚动增加层次

备选方案：
- 纯 2D：实现简单，但视觉效果较平淡

### 10. 输入系统

**选择：Unity New Input System**

理由：
- 更灵活，支持多设备（键盘、鼠标、手柄、触屏）
- 事件驱动，性能更好
- 可配置性强，易于扩展
- Unity 官方推荐的新方案

备选方案：
- 旧版 Input Manager：简单，但不支持多设备切换

### 11. 存档系统

**选择：JSON 文件存档**

理由：
- 简单、可读、易调试
- 跨平台兼容性好
- 便于版本控制和迁移
- 不需要复杂查询，SQLite 过重

实现方式：
- 存档路径：Application.persistentDataPath
- 格式：JSON（Unity 内置 JsonUtility）
- 内容：玩家金币、材料、单位等级、关卡进度

### 12. 场景管理

**选择：多场景架构**

理由：
- 菜单和游戏分离，便于管理
- 减少单场景内存占用
- 支持异步加载，避免卡顿

场景划分：
- MainMenu：主菜单、关卡选择、单位管理
- Game：游戏战斗场景
- Loading：加载过渡场景

### 13. 版本控制

**选择：Git**

理由：
- 代码备份和版本管理
- 支持分支开发
- 行业标准

配置：
- 初始化 .gitignore（忽略 Library、Temp、Build 等）
- 使用 Git LFS 管理大文件（图片、音频）

### 14. 代码规范

**选择：Unity 官方 C# 编码规范**

命名规范：
- 类名：PascalCase（如 GameManager）
- 方法名：PascalCase（如 SpawnEnemy）
- 变量名：camelCase（如 currentHealth）
- 私有字段：_camelCase（如 _currentHealth）
- 常量：UPPER_SNAKE_CASE（如 MAX_HEALTH）
- 接口：I + PascalCase（如 IDamageable）

注释规范：
- 公共 API 必须有 XML 文档注释
- 复杂逻辑添加行内注释
- 类头部添加功能说明

核心模块划分：
```
Assets/
├── Scripts/
│   ├── Core/              # 核心模块
│   │   ├── GameManager.cs
│   │   ├── LevelManager.cs
│   │   └── EventSystem.cs
│   ├── Gameplay/          # 游戏逻辑
│   │   ├── Lane/
│   │   ├── Defender/
│   │   ├── Attacker/
│   │   └── Power/
│   ├── Data/              # 数据模型
│   │   ├── ScriptableObjects/
│   │   └── SaveSystem/
│   ├── Network/           # 联网模块
│   │   ├── MirrorManager.cs
│   │   └── SyncComponents/
│   └── UI/                # UI 系统
│       ├── MainMenu/
│       ├── LevelSelect/
│       └── InGameHUD/
├── Prefabs/               # 预制体
├── Scenes/                # 场景文件
├── ScriptableObjects/     # 数据配置
├── Art/                   # 美术资源
├── Audio/                 # 音频资源
└── Resources/             # 动态加载资源
```

## Risks / Trade-offs

| 风险 | 影响 | 缓解措施 |
|------|------|----------|
| Unity 学习曲线 | 开发初期效率低 | 先学习基础，循序渐进 |
| C# 语法不熟悉 | 代码质量不佳 | 参考 Unity 最佳实践 |
| Mirror 联网复杂 | 合作模式开发困难 | 先实现单人，后加联网 |
| ScriptableObject 滥用 | 数据管理混乱 | 合理规划数据结构 |
| 跨平台测试不足 | 平台兼容性问题 | 优先测试主流平台 |
| 对象池管理不当 | 内存泄漏或对象残留 | 规范对象回收流程 |

## Open Questions

1. ~~Unity 版本选择：2022 LTS 还是 Unity 6？~~ → 已确定：Unity 6.3 LTS
2. ~~是否需要支持手柄/触屏操作？~~ → 已确定：支持（New Input System）
3. 存档是否需要云同步？（暂不实现，后续考虑）
4. 是否需要接入广告/内购系统？（暂不实现，后续考虑）
