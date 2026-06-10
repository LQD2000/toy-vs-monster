# ObjectPool 对象池配置指南

> **创建步骤**: Step 2 - 核心框架搭建（初始创建）
> **更新步骤**: Step 6 - 战斗系统（新增 Defender 池）
> **规格文档**: `openspec/specs/combat-system/spec.md`
> **归档位置**: `openspec/changes/archive/2026-06-10-toy-tower-defense-step6/`

## 概述

ObjectPool 是单例组件，管理游戏中频繁创建/销毁的对象（弹丸、怪物等）。需要在 Unity Editor 中配置对象池列表。

## 文件位置

ObjectPool 组件挂载在场景中的持久化 GameObject 上（通常命名为 `ObjectPool`）。

## 配置步骤

### 1. 创建 ObjectPool GameObject

1. 在 Hierarchy 窗口右键 → `Create Empty`
2. 命名为 `ObjectPool`
3. 添加 `ObjectPool (Script)` 组件

### 2. 配置对象池列表

在 Inspector 中找到 `Pool Configs` 列表，添加以下 4 个配置:

#### MarbleProjectile（弹丸池）
| 字段 | 值 | 说明 |
|------|-----|------|
| Pool Name | `MarbleProjectile` | 必须与代码中的字符串完全一致 |
| Prefab | `Marble.prefab` | 从 Project 窗口拖拽 |
| Initial Size | 20 | 预创建数量 |
| Auto Expand | ✓ | 池空时自动扩展 |

#### Enemy（敌人池）
| 字段 | 值 | 说明 |
|------|-----|------|
| Pool Name | `Enemy` | 必须与代码中的字符串完全一致 |
| Prefab | `DustSprite.prefab` | 从 Project 窗口拖拽 |
| Initial Size | 10 | 预创建数量 |
| Auto Expand | ✓ | 池空时自动扩展 |

#### Attacker（进攻方池）
| 字段 | 值 | 说明 |
|------|-----|------|
| Pool Name | `Attacker` | 必须与代码中的字符串完全一致 |
| Prefab | `DustSprite.prefab` | 从 Project 窗口拖拽 |
| Initial Size | 10 | 预创建数量 |
| Auto Expand | ✓ | 池空时自动扩展 |

#### Defender（防守方池）
| 字段 | 值 | 说明 |
|------|-----|------|
| Pool Name | `Defender` | 必须与代码中的字符串完全一致 |
| Prefab | `MarbleShooter.prefab` | 从 Project 窗口拖拽 |
| Initial Size | 5 | 预创建数量 |
| Auto Expand | ✓ | 池空时自动扩展 |

### 3. 确保 DontDestroyOnLoad

ObjectPool 的 `Awake()` 方法会自动调用 `DontDestroyOnLoad(gameObject)`，确保跨场景持久化。

## 代码中的对象池使用

### 获取对象
```csharp
GameObject obj = ObjectPool.Instance.Get("MarbleProjectile", position, rotation);
```

### 归还对象
```csharp
ObjectPool.Instance.Return("MarbleProjectile", gameObject);
```

## 对象池名称对照表

| 池名称 | 使用场景 | Get 调用位置 | Return 调用位置 |
|--------|---------|-------------|----------------|
| `MarbleProjectile` | 弹丸发射 | AttackComponent.cs:75 | Projectile.cs:66 |
| `Enemy` | 敌人生成 | (外部) | Enemy.cs:49 |
| `Attacker` | 怪物生成 | AttackerSpawner.cs:119 | Attacker.cs:51, AttackerMovement.cs:76 |
| `Defender` | 防御者回收 | (外部) | Defender.cs:57 |

## 注意事项

- **Pool Name 必须与代码完全一致**，包括大小写
- Prefab 必须在 Unity Editor 中创建后才能拖拽到配置中
- Initial Size 影响游戏启动时的内存占用
- Auto Expand = true 时，池空会自动创建新对象（有性能开销）
- ObjectPool 必须在场景中存在且 Awake 被调用后才能使用
