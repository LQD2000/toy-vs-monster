# AI-game - 塔防游戏项目

基于 Unity 6.3 LTS 的 2.5D 塔防游戏，采用 OpenSpec 规范驱动工作流开发。

## 技术栈

| 组件 | 选择 | 说明 |
|------|------|------|
| 引擎 | Unity 6.3 LTS (6000.3.x) | 当前最新 LTS，支持到 2028 年 |
| 语言 | C# | Unity 原生支持 |
| 渲染 | 2.5D | 2D 精灵 + Sorting Layer，类似 PvZ 视角 |
| 架构 | MonoBehaviour | 组件化设计，非 ECS/DOTS |
| 输入 | Unity New Input System | 支持键盘、鼠标、手柄、触屏 |
| 联网 | Mirror Networking | 开源框架，支持 Client-Server |
| 数据 | ScriptableObject + JSON | 配置数据 + 游戏数据 |
| 存档 | JSON 文件 | Application.persistentDataPath |
| 场景 | 多场景架构 | MainMenu / Game / Loading |
| UI | UI Toolkit | 基于 USS/CSS，响应式布局 |
| 优化 | 对象池 | 管理弹丸、怪物、电力球等 |
| 版本控制 | Git | 配置 .gitignore + Git LFS |
| 代码规范 | Unity 官方 C# 规范 | PascalCase / camelCase / _camelCase |
| 平台 | 多平台 | Windows/Mac/Linux/iOS/Android/Web |

## 项目结构

```
AI-game/
├── openspec/                    # OpenSpec 工作流文档
│   ├── config.yaml              # 项目配置
│   ├── specs/                   # 主规范（从变更同步）
│   └── changes/                 # 变更记录
│       ├── archive/             # 已归档的变更
│       └── toy-tower-defense/   # 当前变更
├── data/                        # 游戏数据
│   ├── defenders/               # 防御者数据
│   ├── attackers/               # 攻击者数据
│   └── levels/                  # 关卡配置
├── docs/                        # 实现文档
├── .opencode/                   # OpenCode AI 工具配置
├── game/                        # Unity 项目
│   ├── Assets/
│   │   ├── Scripts/             # C# 脚本
│   │   ├── Prefabs/             # 预制体
│   │   ├── Scenes/              # 场景文件
│   │   ├── Art/                 # 美术资源
│   │   ├── Audio/               # 音频资源
│   │   ├── Resources/           # 动态加载资源
│   │   └── Mirror/              # Mirror Networking
│   ├── Packages/                # 包依赖配置
│   └── ProjectSettings/         # 项目设置
└── AGENTS.md                    # 项目知识库
```

## 开发环境要求

- **Unity Hub**: 最新版本
- **Unity 6.3 LTS**: 版本 6000.3.x
- **Git**: 版本控制
- **IDE**: Visual Studio、Rider 或 VS Code（推荐）

## 快速开始

### 1. 克隆仓库
```bash
git clone <仓库地址>
cd AI-game
```

### 2. 打开 Unity 项目
1. 启动 Unity Hub
2. 点击 "Open" → 选择 `game/` 目录
3. 等待 Unity 导入所有包和资源

### 3. 开始开发
- 游戏脚本位于 `game/Assets/Scripts/`
- 预制体位于 `game/Assets/Prefabs/`
- 场景文件位于 `game/Assets/Scenes/`

## 开发工作流

本项目采用 **OpenSpec 规范驱动工作流**：

### 提议变更
```bash
# 使用 OpenSpec 提议新功能
/opsx-propose
```

### 实现变更
```bash
# 根据规范实现功能
/opsx-apply
```

### 验证实现
```bash
# 验证实现是否符合规范
/opsx-verify
```

### 归档变更
```bash
# 完成后归档变更
/opsx-archive
```

## 代码规范

### 命名约定
```csharp
// 类名：PascalCase
public class GameManager : MonoBehaviour

// 方法名：PascalCase
public void SpawnEnemy()

// 变量名：camelCase
private int currentHealth;

// 私有字段：_camelCase
[SerializeField] private float _moveSpeed;

// 常量：UPPER_SNAKE_CASE
private const int MAX_HEALTH = 100;

// 接口：I + PascalCase
public interface IDamageable
```

### 文件组织
- 每个类一个文件
- 文件名与类名一致
- 按功能模块组织目录

## 数据配置

### 防御者数据
位于 `data/defenders/`，包含：
- 基础属性（生命值、攻击力、射程）
- 升级路径
- 特殊能力

### 攻击者数据
位于 `data/attackers/`，包含：
- 敌人类型和属性
- 行为模式
- 掉落物

### 关卡配置
位于 `data/levels/`，包含：
- 地图布局
- 敌人波次
- 胜利条件

## 文档

- **项目知识库**: `AGENTS.md`
- **实现文档**: `docs/` 目录
- **OpenSpec 规范**: `openspec/specs/`
- **变更记录**: `openspec/changes/`

## 贡献指南

1. ** Fork 仓库 **
2. ** 创建功能分支 **: `git checkout -b feature/新功能`
3. ** 提交变更 **: `git commit -m 'feat: 添加新功能'`
4. ** 推送分支 **: `git push origin feature/新功能`
5. ** 创建 Pull Request **

### 提交信息规范
使用 Conventional Commits：
- `feat:` 新功能
- `fix:` 修复 bug
- `docs:` 文档更新
- `style:` 代码格式调整
- `refactor:` 重构
- `test:` 测试相关
- `chore:` 构建/工具链更新

## 许可证

本项目采用 [MIT 许可证](LICENSE)。

## 联系方式

- **项目维护者**: [维护者姓名]
- **邮箱**: [邮箱地址]
- **GitHub**: [GitHub 地址]

## 致谢

- Unity Technologies 提供强大的游戏引擎
- Mirror Networking 提供网络解决方案
- OpenSpec 提供规范驱动工作流