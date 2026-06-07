# 跑道与网格系统实现文档

## 模块概述

实现玩具塔防游戏的第3步：跑道与网格系统基础设施。包含四个核心组件：

- **GridSystem** - 网格管理：格子占用检测、坐标转换
- **LaneManager** - 跑道管理：跑道-小车关联、防线突破判定
- **LaneCar** - 跑道小车：一次性防御，触发后清除整行
- **LaneRenderer** - 可视化渲染：程序化绘制网格线、小车标记

**注意**：本步骤仅实现网格和跑道基础设施。怪物生成/移动由进攻方系统（步骤5）实现，单位放置由防守方系统（步骤4）实现。

## 类图/结构图

```
GridCell (struct)
├── Row: int                  # 行索引（跑道编号，0-based）
├── Col: int                  # 列索引（0=最左，9=最右）
├── WorldPosition: Vector3    # 格子中心世界坐标
├── IsOccupied: bool          # 是否被占用
└── Occupant: GameObject      # 占用者引用

GridSystem (Singleton)
├── _rows: int                # 行数（跑道数），默认 5
├── _cols: int                # 列数，默认 10
├── _cellWidth: float         # 格子宽度，默认 1.5
├── _cellHeight: float        # 格子高度，默认 2.0
├── _gridOrigin: Vector3      # 网格原点（左上角），默认 (-6, -4, 0)
├── _cells: GridCell[,]       # 网格数据
├── GridToWorld(row, col) → Vector3
├── WorldToGrid(Vector3) → (int,int)?
├── GetCell(row, col) → GridCell?
├── IsCellOccupied(row, col) → bool
├── OccupyCell(row, col, occupant) → bool
├── ReleaseCell(row, col)
├── ReleaseRow(row)            # 清除整行
└── GetOccupiedCols(row) → List<int>

LaneManager (Singleton)
├── _laneCount: int           # 跑道数量（1-1 关卡为 1）
├── _laneCars: Dictionary<int, LaneCar>
├── OnLaneCarTriggered: event  # 小车触发事件
├── OnAllCarsTriggered: event  # 全部小车消耗事件
├── RegisterCar(laneIndex, car)
├── UnregisterCar(laneIndex)
├── IsCarAvailable(laneIndex) → bool
├── GetAvailableCars() → List<int>
├── ClearLane(laneIndex)
├── OnMonsterReachedEnd(laneIndex) → bool  # 防线突破处理
├── TriggerCar(laneIndex)
├── GetCarPosition(laneIndex) → Vector3
└── GetLaneBounds(laneIndex) → (minX, maxX, centerY)

LaneCar (MonoBehaviour)
├── _laneIndex: int           # 所属跑道
├── IsTriggered: bool         # 是否已触发
├── OnCarTriggered: event     # 触发事件
├── _spriteRenderer: SpriteRenderer
├── _activeColor: Color       # 就绪颜色（绿）
├── _triggeredColor: Color    # 已触发颜色（红）
├── Trigger()                 # 触发小车
└── TriggerAnimation()        # 销毁动画

LaneRenderer (MonoBehaviour)
├── 使用 LineRenderer 绘制网格线
├── Gizmos 绘制格子标记、小车位置
├── DrawGrid()                # 绘制网格线
├── ClearGrid()               # 清除网格线
└── OnDrawGizmos()            # 编辑器可视化
```

## 架构关系

```
                    ┌─────────────┐
                    │  GameManager │ (Step 2)
                    └──────┬──────┘
                           │ 状态变更事件
         ┌─────────────────┼─────────────────┐
         ▼                 ▼                  ▼
  ┌─────────────┐   ┌─────────────┐   ┌─────────────┐
  │  GridSystem  │◄──│ LaneManager │──►│   LaneCar   │
  │   (Singleton)│   │ (Singleton) │   │(MonoBehaviour)
  └──────┬──────┘   └─────────────┘   └─────────────┘
         │                                     │
         │ 格子数据                         Trigger()
         ▼                                     ▼
  ┌──────────────┐                    ┌─────────────┐
  │ LaneRenderer │                    │ GridSystem.  │
  │(MonoBehaviour)│                   │ ReleaseRow() │
  └──────────────┘                    └─────────────┘
```

**数据流：**
1. `LaneManager.OnMonsterReachedEnd(laneIndex)` → 检查小车可用性
2. 小车可用 → `LaneCar.Trigger()` → `LaneManager.ClearLane(laneIndex)` → `GridSystem.ReleaseRow(row)`
3. 小车已消耗 → 返回 `false`，由调用方决定是否调用 `GameManager.LoseGame()`
4. 全部小车消耗 → `LaneManager.OnAllCarsTriggered`

## 关键实现

### GridSystem - 网格管理

**坐标系统：**
- 原点 `_gridOrigin` 位于网格区域左上角
- X 轴向右递增，Y 轴向下递减（Unity 屏幕坐标）
- 格子中心偏移：`cellWidth * 0.5` 和 `cellHeight * 0.5`

```
原点 (-6, -4)
   ┌─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┬─────┐
   │(0,0)│(0,1)│(0,2)│(0,3)│(0,4)│(0,5)│(0,6)│(0,7)│(0,8)│(0,9)│  跑道 0
   ├─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┼─────┤
   │(1,0)│(1,1)│ ...                                          │  跑道 1
   ├─────┼─────┼                                             ─┼─────┤
   │ ...                                                       │  跑道 2
   └───────────────────────────────────────────────────────────┘
  ← 左侧（防线）                                          右侧（入口）→
  ← 怪物前进方向                                           怪物出生 →
```

**占用检测：**
- 越界访问自动返回 `true`（视为已占用），防止在无效位置放置单位
- `OccupyCell` 自动将占用者移动到格子中心

### LaneManager - 跑道管理

**防线突破处理 (`OnMonsterReachedEnd`)：**
```
怪物到达格1左侧
    │
    ├── IsCarAvailable(laneIndex) == true
    │       → TriggerCar(laneIndex)
    │       → 小车清除整行怪物
    │       → 返回 true（防线守住）
    │
    └── IsCarAvailable(laneIndex) == false
            → OnAllCarsTriggered (如全部消耗)
            → 返回 false（防线突破）
            → GameManager.LoseGame()
```

### LaneCar - 触发动画

触发流程：
1. 标记 `IsTriggered = true`
2. 通知 `LaneManager.ClearLane(laneIndex)` 清除所有格子
3. 执行抖动 + 颜色渐变动画（0.5 秒）
4. 动画结束后 `Destroy(gameObject)`

### LaneRenderer - 可视化

**运行时渲染：**
- 使用 `LineRenderer` 动态绘制水平线和垂直线
- 线段数 = `(Rows+1) + (Cols+1)`（所有边界线）
- 每条线段 2 个顶点，因此总顶点数 = `2 * (Rows+1 + Cols+1)`

**编辑器 Gizmos：**
- `OnDrawGizmos`: 绘制格子方块、小车位置标记、跑道起点/终点
- `OnDrawGizmosSelected`: 绘制格子坐标标签 `(row, col)`

## 配置说明

### GridSystem Inspector 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Rows | int | 5 | 跑道数量 |
| Cols | int | 10 | 每跑道格子数 |
| Cell Width | float | 1.5 | 格子宽度（世界单位） |
| Cell Height | float | 2.0 | 格子高度（世界单位） |
| Grid Origin | Vector3 | (-6, -4, 0) | 网格左上角世界坐标 |

### LaneManager Inspector 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Lane Count | int | 1 | 跑道数量（1-1 关卡为 1） |

### LaneCar Inspector 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Lane Index | int | 0 | 所属跑道索引 |
| Sprite Renderer | SpriteRenderer | null | 小车精灵渲染器 |
| Active Color | Color | Green | 就绪状态颜色 |
| Triggered Color | Color | Red | 已触发状态颜色 |
| Trigger Animation Duration | float | 0.5 | 触发动画时长（秒） |

### LaneRenderer Inspector 参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Grid Line Color | Color | Gray (50% alpha) | 网格线颜色 |
| Grid Line Width | float | 0.03 | 网格线宽度 |
| Grid Line Material | Material | null | 网格线材质 |
| Cell Marker Color | Color | Dark (20% alpha) | 格子标记颜色 |
| Show Cell Labels | bool | true | 是否显示格子标签 |
| Car Marker Color | Color | Green (50% alpha) | 小车标记颜色 |
| Car Triggered Color | Color | Red (50% alpha) | 小车已触发颜色 |
| Start Marker Color | Color | Blue (50% alpha) | 跑道起点标记 |
| End Marker Color | Color | Orange (50% alpha) | 跑道终点标记 |

## 使用示例

### 在 Unity 场景中设置

1. 创建空 GameObject，命名为 `GridManager`，挂载 `GridSystem` 组件
2. 创建空 GameObject，命名为 `LaneManager`，挂载 `LaneManager` 组件
3. 创建空 GameObject，命名为 `LaneRenderer`，挂载 `LaneRenderer` 组件（自动添加 `LineRenderer`）
4. 创建小车 GameObject，挂载 `SpriteRenderer`（添加精灵）和 `LaneCar` 组件

### 代码调用示例

```csharp
// 放置单位到格子
var gridCell = GridSystem.Instance.WorldToGrid(clickPosition);
if (gridCell.HasValue)
{
    bool placed = GridSystem.Instance.OccupyCell(
        gridCell.Value.row,
        gridCell.Value.col,
        defenderPrefab
    );
}

// 怪物到达终点
bool defended = LaneManager.Instance.OnMonsterReachedEnd(laneIndex);
if (!defended)
{
    GameManager.Instance.LoseGame();
}

// 监听小车触发
LaneManager.Instance.OnLaneCarTriggered += (laneIndex, hasRemaining) => {
    Debug.Log($"跑道 {laneIndex} 小车触发，剩余小车: {hasRemaining}");
};

// 监听全部小车消耗
LaneManager.Instance.OnAllCarsTriggered += () => {
    Debug.Log("所有小车已消耗！");
};
```

## 注意事项

1. **初始化顺序**：`GridSystem` 需在 `LaneRenderer` 之前初始化，确保网格数据已就绪
2. **1-1 关卡适配**：`LaneManager._laneCount` 默认值为 1，当需要更多跑道时改为 5-6
3. **坐标系**：网格原点 `_gridOrigin` 使用左上角参考，Y 轴向下为正向
4. **对象销毁**：`ReleaseRow()` 仅 `SetActive(false)` 不直接 `Destroy`，实际销毁由对象池或调用方处理
5. **编辑器可视化**：`OnDrawGizmos` 仅在编辑器模式下可见，运行时使用 `LaneRenderer` 的 `LineRenderer`
6. **越界处理**：`IsCellOccupied` 在越界时返回 `true`（已占用），防止单位放置到无效位置

## 后续优化方向

1. 支持动态调整网格尺寸（运行时修改 rows/cols）
2. 添加单位拖拽移动功能
3. 格子高亮效果（悬停、选中、攻击范围）
4. 支持网格缩放和旋转
5. LineRenderer 池化以减少 GC
