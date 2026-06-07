using UnityEngine;

/// <summary>
/// 网格系统 - 管理游戏场地格子（占用检测、坐标转换）
/// 单例模式，与 LaneManager 配合使用
/// </summary>
public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("网格配置")]
    [SerializeField] private int _rows = 5;
    [SerializeField] private int _cols = 10;
    [SerializeField] private float _cellWidth = 1.5f;
    [SerializeField] private float _cellHeight = 2.0f;
    [SerializeField] private Vector3 _gridOrigin = new Vector3(-6f, -4f, 0f);

    /// <summary>行数（跑道数）</summary>
    public int Rows => _rows;

    /// <summary>列数（每跑道格子数）</summary>
    public int Cols => _cols;

    /// <summary>格子宽度</summary>
    public float CellWidth => _cellWidth;

    /// <summary>格子高度</summary>
    public float CellHeight => _cellHeight;

    /// <summary>网格原点（左上角位置）</summary>
    public Vector3 GridOrigin => _gridOrigin;

    /// <summary>网格数据</summary>
    private GridCell[,] _cells;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeGrid();
    }

    /// <summary>
    /// 初始化网格数据
    /// </summary>
    private void InitializeGrid()
    {
        _cells = new GridCell[_rows, _cols];
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                Vector3 worldPos = GridToWorld(row, col);
                _cells[row, col] = new GridCell(row, col, worldPos);
            }
        }
        Debug.Log($"[GridSystem] 网格初始化完成: {_rows} 行 x {_cols} 列, 原点: {_gridOrigin}");
    }

    /// <summary>
    /// 将网格坐标转换为世界坐标（格子中心）
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    /// <param name="col">列索引（0-based）</param>
    /// <returns>格子中心的世界坐标</returns>
    public Vector3 GridToWorld(int row, int col)
    {
        float x = _gridOrigin.x + col * _cellWidth + _cellWidth * 0.5f;
        float y = _gridOrigin.y - row * _cellHeight - _cellHeight * 0.5f;
        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// 将世界坐标转换为网格坐标
    /// </summary>
    /// <param name="worldPosition">世界坐标</param>
    /// <returns>网格坐标 (row, col)，如果超出范围返回 null</returns>
    public (int row, int col)? WorldToGrid(Vector3 worldPosition)
    {
        float relX = worldPosition.x - _gridOrigin.x;
        float relY = _gridOrigin.y - worldPosition.y;

        int col = Mathf.FloorToInt(relX / _cellWidth);
        int row = Mathf.FloorToInt(relY / _cellHeight);

        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
        {
            return null;
        }

        return (row, col);
    }

    /// <summary>
    /// 获取指定格子数据
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    /// <param name="col">列索引（0-based）</param>
    /// <returns>格子数据，超出范围返回 null</returns>
    public GridCell? GetCell(int row, int col)
    {
        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
        {
            Debug.LogWarning($"[GridSystem] 获取格子超出范围: ({row}, {col})");
            return null;
        }
        return _cells[row, col];
    }

    /// <summary>
    /// 检查格子是否被占用
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    /// <param name="col">列索引（0-based）</param>
    /// <returns>是否被占用</returns>
    public bool IsCellOccupied(int row, int col)
    {
        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
        {
            Debug.LogWarning($"[GridSystem] 检查占用超出范围: ({row}, {col})");
            return true; // 越界视为已占用
        }
        return _cells[row, col].IsOccupied;
    }

    /// <summary>
    /// 占用指定格子
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    /// <param name="col">列索引（0-based）</param>
    /// <param name="occupant">占用者游戏对象</param>
    /// <returns>是否占用成功</returns>
    public bool OccupyCell(int row, int col, GameObject occupant)
    {
        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
        {
            Debug.LogWarning($"[GridSystem] 占用格子超出范围: ({row}, {col})");
            return false;
        }

        if (_cells[row, col].IsOccupied)
        {
            Debug.Log($"[GridSystem] 格子 ({row}, {col}) 已被占用");
            return false;
        }

        _cells[row, col].IsOccupied = true;
        _cells[row, col].Occupant = occupant;

        // 将占用者移动到格子中心
        if (occupant != null)
        {
            occupant.transform.position = _cells[row, col].WorldPosition;
        }

        Debug.Log($"[GridSystem] 格子 ({row}, {col}) 被占用: {occupant?.name}");
        return true;
    }

    /// <summary>
    /// 释放指定格子
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    /// <param name="col">列索引（0-based）</param>
    public void ReleaseCell(int row, int col)
    {
        if (row < 0 || row >= _rows || col < 0 || col >= _cols)
        {
            Debug.LogWarning($"[GridSystem] 释放格子超出范围: ({row}, {col})");
            return;
        }

        if (_cells[row, col].IsOccupied)
        {
            Debug.Log($"[GridSystem] 释放格子 ({row}, {col})");
        }

        _cells[row, col].IsOccupied = false;
        _cells[row, col].Occupant = null;
    }

    /// <summary>
    /// 释放指定行（跑道）的所有格子
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    public void ReleaseRow(int row)
    {
        if (row < 0 || row >= _rows)
        {
            Debug.LogWarning($"[GridSystem] 释放行超出范围: {row}");
            return;
        }

        for (int col = 0; col < _cols; col++)
        {
            if (_cells[row, col].IsOccupied)
            {
                if (_cells[row, col].Occupant != null)
                {
                    Destroy(_cells[row, col].Occupant);
                }
                _cells[row, col].IsOccupied = false;
                _cells[row, col].Occupant = null;
            }
        }

        Debug.Log($"[GridSystem] 已释放第 {row} 行所有格子");
    }

    /// <summary>
    /// 获取指定行中所有被占用的格子列索引列表
    /// </summary>
    /// <param name="row">行索引（0-based）</param>
    /// <returns>被占用的格子列索引列表</returns>
    public System.Collections.Generic.List<int> GetOccupiedCols(int row)
    {
        var occupied = new System.Collections.Generic.List<int>();
        if (row < 0 || row >= _rows) return occupied;

        for (int col = 0; col < _cols; col++)
        {
            if (_cells[row, col].IsOccupied)
            {
                occupied.Add(col);
            }
        }
        return occupied;
    }

    private void OnDrawGizmosSelected()
    {
        if (_cells == null) return;

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                Vector3 center = GridToWorld(row, col);
                Vector3 size = new Vector3(_cellWidth * 0.9f, _cellHeight * 0.9f, 0f);

                // 占用状态用不同颜色
                Gizmos.color = _cells[row, col].IsOccupied
                    ? new Color(1f, 0f, 0f, 0.3f)
                    : new Color(0f, 1f, 0f, 0.15f);

                Gizmos.DrawCube(center, size);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
