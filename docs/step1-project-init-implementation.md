# 项目初始化 - 实现文档

## 模块概述
项目初始化模块负责搭建 Unity 项目的基础环境，包括项目创建、目录结构、包管理和版本控制配置。

## 项目结构
```
toy-vs-monster/
├── .gitignore                # Git 忽略规则
├── AGENTS.md                 # 项目知识库
├── openspec/                 # OpenSpec 工作流文档
│   └── changes/toy-tower-defense/
│       ├── proposal.md       # 提案
│       ├── design.md         # 设计文档
│       ├── tasks.md          # 任务清单
│       ├── specs/            # 规格说明
│       ├── data/             # 单位/关卡数据
│       └── docs/             # 实现文档
└── game/                     # Unity 项目
    ├── Assets/
    │   ├── Scripts/          # C# 脚本
    │   ├── Prefabs/          # 预制体
    │   ├── Scenes/           # 场景文件
    │   ├── Art/              # 美术资源
    │   ├── Audio/            # 音频资源
    │   ├── Resources/        # 动态加载资源
    │   └── Mirror/           # Mirror Networking（直接导入）
    ├── Packages/
    │   ├── manifest.json     # 包依赖配置
    │   └── packages-lock.json
    └── ProjectSettings/
        ├── ProjectVersion.txt
        ├── ProjectSettings.asset
        └── ... (其他 Unity 设置文件)
```

## 关键配置

### Unity 版本
- **实际版本**: Unity 6000.3.17f1 (Unity 6.3 LTS)
- **安装方式**: Unity Hub 手动安装
- **License**: Unity Personal

### 包依赖 (Packages/manifest.json)
| 包名 | 版本 | 用途 | 安装方式 |
|------|------|------|----------|
| com.unity.inputsystem | 1.19.0 | New Input System | Package Manager |
| com.unity.textmeshpro | 3.2.0-pre.12 | 文本渲染 | Package Manager |
| com.unity.2d.sprite | 1.0.0 | 2D 精灵 | Package Manager (内置) |
| com.unity.2d.tilemap | 1.0.0 | 瓦片地图 | Package Manager (内置) |
| com.unity.2d.tilemap.extras | 6.0.2 | 扩展瓦片功能 | Package Manager |
| com.unity.ugui | 2.0.0 | UI 系统 | Package Manager (内置) |

### Assets 目录直接导入
| 组件 | 路径 | 说明 |
|------|------|------|
| Mirror Networking | Assets/Mirror/ | 直接导入，非 Package Manager |

### 待安装
| 组件 | 安装方式 | 说明 |
|------|----------|------|
| DOTween | Asset Store / OpenUPM | 需要时再安装 |

### 项目设置 (ProjectSettings/)
- **分辨率**: 1920x1080 (默认)
- **色彩空间**: Gamma
- **公司名**: DefaultCompany
- **产品名**: toy-vs-monster
- **应用标识**: com.DefaultCompany.toy-vs-monster

### Input System 配置
- **版本**: com.unity.inputsystem 1.19.0
- **Input Actions 资源**: `Assets/Settings/PlayerControls.inputactions`
- **验证脚本**: `Assets/Scripts/InputTest.cs`（可删除）
- **已配置 Action Map**: `Gameplay`
  - `Tap` (Button) — 点击放置/收集
  - `Drag` (Value, Vector2) — 小手拖拽
  - `TapPosition` (Value, Vector2) — 点击位置

### .gitignore 配置
Unity 项目在 `game/` 子目录中，路径已添加 `game/` 前缀：

**忽略的目录：**
- `game/[Ll]ibrary/` - Unity 缓存（自动生成）
- `game/[Tt]emp/` - 临时文件
- `game/[Ll]ogs/` - 日志
- `game/[Uu]ser[Ss]ettings/` - 用户本地设置

**跟踪的目录：**
- `game/Assets/` - 脚本、预制体、场景、资源
- `game/Packages/` - 包配置
- `game/ProjectSettings/` - 项目设置

**全局忽略：**
- `.DS_Store`, `Thumbs.db` (OS 文件)
- `.vs/`, `.vscode/`, `.idea/` (IDE 配置)
- `*.csproj`, `*.sln` (Unity 自动生成)
- `*.apk`, `*.aab`, `*.unitypackage` (构建产物)

## 注意事项
1. **Mirror 安装方式**: 由于 Package Manager 无法正常安装 Git URL，Mirror 采用直接导入 Assets 目录的方式
2. **Unity 首次打开**: 用 Unity 打开 `game/` 目录时会自动导入所有包并生成 Library 缓存
3. **版本控制**: `.gitignore` 已配置正确，忽略 Unity 生成的缓存文件

## 验证清单
- [x] Unity Hub 已安装
- [x] Unity 6000.3.17f1 已安装
- [x] Unity License 已激活
- [x] 项目目录结构已创建
- [x] Packages/manifest.json 已配置
- [x] Mirror 已导入 (Assets/Mirror/)
- [x] Git 仓库已初始化
- [x] .gitignore 已配置（game/ 前缀）
- [x] 项目已被 Unity 成功打开并导入
