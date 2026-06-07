## 设计概述

搭建玩具塔防游戏的基础开发环境，为后续游戏开发做好准备。

## 技术选型

- **引擎**: Unity 6.3 LTS (6000.3.x)
- **语言**: C#
- **渲染**: 2D（2D 精灵 + Sorting Layer）
- **架构**: MonoBehaviour（组件化设计）
- **输入**: Unity New Input System
- **联网**: Mirror Networking
- **数据**: ScriptableObject + JSON
- **UI**: UI Toolkit

## 目录结构

```
Assets/
├── Scripts/          # C# 脚本
├── Prefabs/          # 预制体
├── Scenes/           # 场景文件
├── Art/              # 美术资源
├── Audio/            # 音频资源
└── Resources/        # 运行时加载资源
```

## Package 依赖

- Mirror Networking: 网络同步
- TextMeshPro: 文本渲染
- DOTween: 动画缓动
- New Input System: 输入处理

## 版本控制

- Git 仓库初始化
- .gitignore 配置（忽略 Unity 临时文件、Library、Temp 等）

## 实现步骤

1. 安装 Unity 6.3 LTS via Unity Hub
2. 创建新 Unity 项目（2D 模板）
3. 配置项目设置（分辨率、平台）
4. 创建基础目录结构
5. 创建实现文档目录
6. 安装必要 Package
7. 初始化 Git 仓库
8. 配置 New Input System