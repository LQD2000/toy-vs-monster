# PROJECT KNOWLEDGE BASE

**Generated:** 2026-06-05
**Tech Stack:** Unity 6.3 LTS (6000.3.x) + C#
**Workflow:** OpenSpec (spec-driven)

## OVERVIEW
AI-game — greenfield project, spec-driven development via OpenSpec. Chinese-language docs, Unity + C# implementation. No code written yet.

## TECH STACK

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

## STRUCTURE
```
./
├── openspec/                    # Spec-driven workflow (specs, changes, config)
│   ├── config.yaml              # Project config: Unity + C#, Chinese, proposal/task rules
│   ├── specs/                   # Main specs (synced from changes)
│   └── changes/                 # Delta changes
│       ├── archive/             # Archived changes (YYYY-MM-DD-name)
│       ├── toy-tower-defense/   # Original change (reference)
│       └── toy-tower-defense-step{2-22}/  # Split changes (21 steps)
├── data/                        # Game data (ScriptableObject definitions)
│   ├── defenders/               # Defender stats and configs
│   ├── attackers/               # Attacker stats and configs
│   └── levels/                  # Level configurations
├── docs/                        # Implementation documentation
├── .opencode/                   # OpenCode AI tooling (commands, skills, plugin)
├── game/                        # Unity project
└── AGENTS.md                    # This file
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Project config | `openspec/config.yaml` | Language, tech stack, artifact rules |
| Game design | `openspec/changes/toy-tower-defense/proposal.md` | What & why |
| Technical design | `openspec/changes/toy-tower-defense/design.md` | How |
| Task list | `openspec/changes/toy-tower-defense/tasks.md` | Implementation checklist |
| Specs | `openspec/changes/toy-tower-defense/specs/` | Requirements per capability |
| Unit data | `data/defenders/` | Defender stats |
| Attacker data | `data/attackers/` | Attacker stats |
| Level data | `data/levels/` | Level configs |
| Implementation docs | `docs/` | Module implementation details |
| Propose a change | `/opsx-propose` | Creates proposal + design + tasks |
| Implement changes | `/opsx-apply` | Reads artifacts, implements tasks |
| Explore/think | `/opsx-explore` | No code writing in this mode |
| Verify | `/opsx-verify` | Check implementation vs artifacts |
| Archive | `/opsx-archive` | Date-prefixed: `YYYY-MM-DD-name` |

## CONVENTIONS
- **Language**: Write docs/outputs in Chinese. Keep technical terms (API, REST, GraphQL) and code/paths in English
- **Proposals**: Under 1500 words, must include Non-goals section
- **Tasks**: Max 2 hours per chunk
- **Spec requirements**: Use SHALL keyword
- **Change naming**: kebab-case (e.g., `add-user-auth`)
- **Delta specs**: Section headers ADDED / MODIFIED / REMOVED / RENAMED
- **Implementation docs**: All development MUST be accompanied by detailed implementation documentation

## IMPLEMENTATION DOCUMENTATION RULE

**所有开发必须同步新增详细实现文档。**

每当实现一个功能或模块时，必须在 `docs/` 目录下创建或更新对应的实现文档，内容包括：

1. **模块概述**：功能描述、职责范围
2. **类图/结构图**：关键类和它们的关系
3. **关键实现**：核心算法、设计模式、Unity 组件用法
4. **配置说明**：Inspector 参数、ScriptableObject 字段
5. **使用示例**：如何调用、如何配置
6. **注意事项**：已知限制、性能考量、后续优化方向

文档命名规范：`<模块名>-implementation.md`（如 `lane-system-implementation.md`）

## UNITY EDITOR 手动步骤文档规则

**无法通过代码创建的 Unity 资源，必须通过 README 文档说明创建步骤。**

Unity 中有些资源只能在 Editor 中手动创建（如 Prefab、Scene、Animation 等），AI 无法直接生成这些二进制资产。对此类资源，必须：

1. **创建目录结构**：在对应位置创建文件夹（如 `game/Assets/Prefabs/Attackers/`）
2. **编写 README.md**：详细说明手动创建步骤，包括：
   - 文件位置和命名
   - 分步操作指南（带编号）
   - 需要添加的组件及其配置
   - 属性参考值（来自 `data/` 目录的设计文档）
   - 注意事项和常见问题

3. **在实现文档中引用**：在 `<模块名>-implementation.md` 中引用 README 的创建步骤

**适用场景**：
- Prefab 创建（需要拖拽组件、配置 Inspector）
- Scene 创建（需要设置 Lighting、Build Settings）
- Animation/Animator Controller（需要在 Animator 窗口编辑）
- Tilemap Palette（需要在 Tile Palette 窗口创建）
- Shader Graph（需要可视化编辑器）

**示例结构**：
```
game/Assets/Prefabs/
├── Attackers/
│   ├── README.md          # DustSprite Prefab 创建指南
│   └── DustSprite.prefab  # 手动创建（按 README 步骤）
├── Defenders/
│   ├── README.md          # 防御者 Prefab 创建指南
│   └── MarbleShooter.prefab
└── Projectiles/
    ├── README.md          # 弹丸 Prefab 创建指南
    └── Marble.prefab
```

**文档模板**：
```markdown
# [资源名称] 创建指南

## 文件位置
`game/Assets/Prefabs/[路径]/[名称].prefab`

## 创建步骤

### 1. 在 Unity Editor 中创建
1. [第一步操作]
2. [第二步操作]
...

### 2. 添加组件
- [组件1]: [配置说明]
- [组件2]: [配置说明]

### 3. 保存为 Prefab
1. [保存步骤]

## 属性参考
| 属性 | 值 | 说明 |
|------|-----|------|
| ... | ... | ... |

## 注意事项
- [注意事项1]
- [注意事项2]
```

## CODE STYLE

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

## NOTES
- Git repo will be initialized at project root
- Unity project will be in `./game/` directory
- `.opencode/` is tooling only, not application code
- All implementation must have corresponding documentation in `docs/` folder
- Original change `toy-tower-defense` is split into 21 steps (`toy-tower-defense-step{2-22}`)
- Step 1 (project init) is archived in `openspec/changes/archive/2026-06-07-toy-tower-defense-step1/`
- Use `/opsx-apply` to implement individual steps, `/opsx-archive` to archive completed steps

## WORKFLOW: SPEC SYNC RULE
**归档步骤后必须同步规格文档到主规格目录。**

执行顺序：
1. `/opsx-archive` 归档变更
2. 检查 `openspec/changes/archive/<date>-<name>/specs/` 中的 delta specs
3. 将 delta specs 同步到 `openspec/specs/<capability>/spec.md`
4. 提交并推送

示例：
```bash
# 归档后同步
cp openspec/changes/archive/2026-06-07-toy-tower-defense-step3/specs/core-gameplay/spec.md openspec/specs/core-gameplay/spec.md
git add openspec/specs/ && git commit -m "docs: 同步规格文档" && git push
```
