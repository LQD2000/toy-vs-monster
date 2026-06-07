using UnityEngine;

/// <summary>
/// 跑道渲染器 - 程序化绘制跑道网格线、格子标记和小车位置
/// 用于在编辑器和运行时可视化网格系统
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LaneRenderer : MonoBehaviour
{
    [Header("网格线配置")]
    [SerializeField] private Color _gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private float _gridLineWidth = 0.03f;
    [SerializeField] private Material _gridLineMaterial;

    [Header("格子标记")]
    [SerializeField] private Color _cellMarkerColor = new Color(0.3f, 0.3f, 0.3f, 0.2f);
    [SerializeField] private bool _showCellLabels = true;
    [SerializeField] private int _cellLabelFontSize = 12;

    [Header("小车位置标记")]
    [SerializeField] private Color _carMarkerColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color _carTriggeredColor = new Color(1f, 0f, 0f, 0.5f);
    [SerializeField] private float _carMarkerSize = 0.4f;

    [Header("跑道起点/终点标记")]
    [SerializeField] private Color _startMarkerColor = new Color(0f, 0f, 1f, 0.5f); // 左端=防线
    [SerializeField] private Color _endMarkerColor = new Color(1f, 0.5f, 0f, 0.5f);  // 右端=怪物入口

    private GridSystem _gridSystem;
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _gridSystem = GridSystem.Instance;
    }

    private void Start()
    {
        if (_gridSystem == null)
        {
            _gridSystem = FindFirstObjectByType<GridSystem>();
        }

        if (_gridSystem != null)
        {
            DrawGrid();
        }
        else
        {
            Debug.LogWarning("[LaneRenderer] 找不到 GridSystem，无法绘制网格");
        }
    }

    /// <summary>
    /// 绘制整个网格（使用 LineRenderer）
    /// </summary>
    [ContextMenu("Draw Grid")]
    public void DrawGrid()
    {
        if (_gridSystem == null)
        {
            Debug.LogError("[LaneRenderer] GridSystem 为空");
            return;
        }

        // 配置 LineRenderer
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.startWidth = _gridLineWidth;
        _lineRenderer.endWidth = _gridLineWidth;
        _lineRenderer.startColor = _gridLineColor;
        _lineRenderer.endColor = _gridLineColor;

        if (_gridLineMaterial != null)
        {
            _lineRenderer.material = _gridLineMaterial;
        }
        else
        {
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // 计算需要的线段数量：
        // 水平线: (rows + 1) 条，每条需要 2 个顶点
        // 垂直线: (cols + 1) 条，每条需要 2 个顶点
        int rows = _gridSystem.Rows;
        int cols = _gridSystem.Cols;
        int lineCount = (rows + 1) + (cols + 1);
        int vertexCount = lineCount * 2;

        _lineRenderer.positionCount = vertexCount;

        int vertexIndex = 0;

        // 绘制水平线（每行上下边界）
        for (int row = 0; row <= rows; row++)
        {
            float y;
            if (row == 0)
            {
                // 顶部边界
                y = _gridSystem.GridOrigin.y;
            }
            else if (row == rows)
            {
                // 底部边界
                y = _gridSystem.GridOrigin.y - rows * _gridSystem.CellHeight;
            }
            else
            {
                // 中间分隔线 = 格子边界
                y = _gridSystem.GridOrigin.y - row * _gridSystem.CellHeight;
            }

            float startX = _gridSystem.GridOrigin.x;
            float endX = _gridSystem.GridOrigin.x + cols * _gridSystem.CellWidth;

            _lineRenderer.SetPosition(vertexIndex++, new Vector3(startX, y, 0f));
            _lineRenderer.SetPosition(vertexIndex++, new Vector3(endX, y, 0f));
        }

        // 绘制垂直线（每列左右边界）
        for (int col = 0; col <= cols; col++)
        {
            float x;
            if (col == 0)
            {
                x = _gridSystem.GridOrigin.x;
            }
            else if (col == cols)
            {
                x = _gridSystem.GridOrigin.x + cols * _gridSystem.CellWidth;
            }
            else
            {
                x = _gridSystem.GridOrigin.x + col * _gridSystem.CellWidth;
            }

            float startY = _gridSystem.GridOrigin.y;
            float endY = _gridSystem.GridOrigin.y - rows * _gridSystem.CellHeight;

            _lineRenderer.SetPosition(vertexIndex++, new Vector3(x, startY, 0f));
            _lineRenderer.SetPosition(vertexIndex++, new Vector3(x, endY, 0f));
        }

        Debug.Log($"[LaneRenderer] 网格绘制完成: {rows}x{cols}, {lineCount} 条线");
    }

    /// <summary>
    /// 清除网格线
    /// </summary>
    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (_gridSystem == null)
        {
            _gridSystem = FindFirstObjectByType<GridSystem>();
        }
        if (_gridSystem == null) return;

        int rows = _gridSystem.Rows;
        int cols = _gridSystem.Cols;

        // 绘制格子标记（半透明方块）
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector3 center = _gridSystem.GridToWorld(row, col);
                Vector3 size = new Vector3(_gridSystem.CellWidth * 0.9f, _gridSystem.CellHeight * 0.9f, 0.01f);

                Gizmos.color = _cellMarkerColor;
                Gizmos.DrawCube(center, size);
            }
        }

        // 绘制小车位置标记
        if (LaneManager.Instance != null)
        {
            for (int lane = 0; lane < LaneManager.Instance.LaneCount; lane++)
            {
                Vector3 carPos = LaneManager.Instance.GetCarPosition(lane);
                bool isAvailable = LaneManager.Instance.IsCarAvailable(lane);

                Gizmos.color = isAvailable ? _carMarkerColor : _carTriggeredColor;
                Gizmos.DrawWireCube(carPos, Vector3.one * _carMarkerSize * 2f);

                // 小车到第一格的连线
                Vector3 cell0Pos = _gridSystem.GridToWorld(lane, 0);
                Gizmos.color = isAvailable ? Color.green : Color.red;
                Gizmos.DrawLine(carPos, cell0Pos);
            }
        }

        // 绘制跑道起点（左侧防线）和终点（右侧怪物入口）标记
        if (LaneManager.Instance != null)
        {
            for (int lane = 0; lane < LaneManager.Instance.LaneCount; lane++)
            {
                var bounds = LaneManager.Instance.GetLaneBounds(lane);

                // 起点标记（左侧 = 防线位置）
                Vector3 startPos = new Vector3(bounds.minX, bounds.centerY, 0f);
                Gizmos.color = _startMarkerColor;
                Gizmos.DrawWireSphere(startPos, 0.2f);

                // 终点标记（右侧 = 怪物入口）
                Vector3 endPos = new Vector3(bounds.maxX, bounds.centerY, 0f);
                Gizmos.color = _endMarkerColor;
                Gizmos.DrawWireSphere(endPos, 0.2f);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 在编辑器中绘制格子标签
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_gridSystem == null || !_showCellLabels) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = _cellLabelFontSize;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        for (int row = 0; row < _gridSystem.Rows; row++)
        {
            for (int col = 0; col < _gridSystem.Cols; col++)
            {
                Vector3 center = _gridSystem.GridToWorld(row, col);
                UnityEditor.Handles.Label(
                    center,
                    $"({row},{col})",
                    style
                );
            }
        }
    }
#endif
}
