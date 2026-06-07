using UnityEngine;

/// <summary>
/// 格子数据 - 表示跑道上单个格子的状态
/// </summary>
[System.Serializable]
public struct GridCell
{
    /// <summary>行索引（跑道编号，0-based）</summary>
    public int Row;

    /// <summary>列索引（0-based，0=最左，9=最右）</summary>
    public int Col;

    /// <summary>格子中心世界坐标</summary>
    public Vector3 WorldPosition;

    /// <summary>格子是否被占用</summary>
    public bool IsOccupied;

    /// <summary>占用该格子的游戏对象</summary>
    public GameObject Occupant;

    public GridCell(int row, int col, Vector3 worldPosition)
    {
        Row = row;
        Col = col;
        WorldPosition = worldPosition;
        IsOccupied = false;
        Occupant = null;
    }
}
