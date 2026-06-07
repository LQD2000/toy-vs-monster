using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 跑道管理器 - 管理所有跑道及其小车
/// 单例模式，与 GridSystem 和 LaneCar 配合使用
/// </summary>
public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance { get; private set; }

    [Header("跑道配置")]
    [SerializeField] private int _laneCount = 1; // 1-1 关卡只有 1 条跑道

    /// <summary>跑道数量</summary>
    public int LaneCount => _laneCount;

    /// <summary>跑道小车字典 - key: 跑道索引, value: 跑道小车引用</summary>
    private readonly Dictionary<int, LaneCar> _laneCars = new Dictionary<int, LaneCar>();

    /// <summary>
    /// 小车触发事件 - 参数1: 跑道索引, 参数2: 是否还有剩余小车
    /// </summary>
    public event Action<int, bool> OnLaneCarTriggered;

    /// <summary>
    /// 所有小车消耗事件 - 所有跑道小车均被触发时触发
    /// </summary>
    public event Action OnAllCarsTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log($"[LaneManager] 跑道管理器初始化, 跑道数量: {_laneCount}");
    }

    private void Start()
    {
        // 确保 GridSystem 存在
        if (GridSystem.Instance == null)
        {
            Debug.LogError("[LaneManager] GridSystem.Instance 为空！");
        }
    }

    /// <summary>
    /// 注册跑道小车
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <param name="car">跑道小车</param>
    public void RegisterCar(int laneIndex, LaneCar car)
    {
        if (_laneCars.ContainsKey(laneIndex))
        {
            Debug.LogWarning($"[LaneManager] 跑道 {laneIndex} 已有小车注册，将被替换");
        }
        _laneCars[laneIndex] = car;
        Debug.Log($"[LaneManager] 跑道 {laneIndex} 小车已注册");
    }

    /// <summary>
    /// 注销跑道小车
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    public void UnregisterCar(int laneIndex)
    {
        if (_laneCars.ContainsKey(laneIndex))
        {
            _laneCars.Remove(laneIndex);
            Debug.Log($"[LaneManager] 跑道 {laneIndex} 小车已注销");
        }
    }

    /// <summary>
    /// 获取指定跑道的格子起始列索引
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <returns>起始列索引（始终为 0）</returns>
    public int GetLaneStartCol(int laneIndex)
    {
        // 在网格系统中，每个跑道从第0列开始
        return 0;
    }

    /// <summary>
    /// 获取指定跑道的格子结束列索引
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <returns>结束列索引</returns>
    public int GetLaneEndCol(int laneIndex)
    {
        if (GridSystem.Instance != null)
        {
            return GridSystem.Instance.Cols - 1;
        }
        return 9; // 默认 10 列
    }

    /// <summary>
    /// 获取跑道小车是否可用
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <returns>true 表示小车可用（未被触发）</returns>
    public bool IsCarAvailable(int laneIndex)
    {
        return _laneCars.ContainsKey(laneIndex) && !_laneCars[laneIndex].IsTriggered;
    }

    /// <summary>
    /// 获取所有可用小车的跑道索引列表
    /// </summary>
    /// <returns>可用小车的跑道索引列表</returns>
    public List<int> GetAvailableCars()
    {
        var available = new List<int>();
        foreach (var kvp in _laneCars)
        {
            if (!kvp.Value.IsTriggered)
            {
                available.Add(kvp.Key);
            }
        }
        return available;
    }

    /// <summary>
    /// 清除指定跑道上的所有单位
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    public void ClearLane(int laneIndex)
    {
        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ReleaseRow(laneIndex);
        }
        Debug.Log($"[LaneManager] 跑道 {laneIndex} 已清除");
    }

    /// <summary>
    /// 怪物突破防线处理
    /// 当怪物到达跑道最左侧且小车未被触发时，触发小车；否则判定失败
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <returns>true 表示防线被守住（小车触发成功），false 表示防线被突破</returns>
    public bool OnMonsterReachedEnd(int laneIndex)
    {
        if (IsCarAvailable(laneIndex))
        {
            // 小车可用，触发小车
            Debug.Log($"[LaneManager] 跑道 {laneIndex} 怪物突破，触发小车！");
            TriggerCar(laneIndex);
            return true;
        }
        else
        {
            // 小车已消耗，防线被突破
            Debug.Log($"[LaneManager] 跑道 {laneIndex} 怪物突破，小车已消耗，防线失守！");
            CheckAllCarsTriggered();
            return false;
        }
    }

    /// <summary>
    /// 触发指定跑道的小车
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    public void TriggerCar(int laneIndex)
    {
        if (_laneCars.ContainsKey(laneIndex) && !_laneCars[laneIndex].IsTriggered)
        {
            _laneCars[laneIndex].Trigger();
            OnLaneCarTriggered?.Invoke(laneIndex, GetAvailableCars().Count > 0);

            CheckAllCarsTriggered();
        }
        else
        {
            Debug.LogWarning($"[LaneManager] 跑道 {laneIndex} 小车不可用或已被触发");
        }
    }

    /// <summary>
    /// 检查是否所有小车都已触发
    /// </summary>
    private void CheckAllCarsTriggered()
    {
        if (GetAvailableCars().Count == 0)
        {
            Debug.Log("[LaneManager] 所有小车已消耗！");
            OnAllCarsTriggered?.Invoke();
        }
    }

    /// <summary>
    /// 获取小车在指定跑道的最左侧世界位置（用于放置车道 Prefab）
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <returns>小车应在的世界坐标</returns>
    public Vector3 GetCarPosition(int laneIndex)
    {
        if (GridSystem.Instance != null)
        {
            // 小车放在第一个格子的左侧
            Vector3 cell0Pos = GridSystem.Instance.GridToWorld(laneIndex, 0);
            return new Vector3(
                cell0Pos.x - GridSystem.Instance.CellWidth * 0.75f,
                cell0Pos.y,
                0f
            );
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 获取整个跑道的世界坐标范围
    /// </summary>
    /// <param name="laneIndex">跑道索引</param>
    /// <returns>(minX, maxX, centerY) 跑道范围</returns>
    public (float minX, float maxX, float centerY) GetLaneBounds(int laneIndex)
    {
        if (GridSystem.Instance != null)
        {
            Vector3 startPos = GridSystem.Instance.GridToWorld(laneIndex, 0);
            Vector3 endPos = GridSystem.Instance.GridToWorld(laneIndex, GridSystem.Instance.Cols - 1);
            float halfWidth = GridSystem.Instance.CellWidth * 0.5f;

            return (
                startPos.x - halfWidth,
                endPos.x + halfWidth,
                startPos.y
            );
        }
        return (0f, 10f, 0f);
    }
}
