using UnityEngine;
using System;

/// <summary>
/// 游戏状态枚举
/// </summary>
public enum GameState
{
    Preparation,  // 准备阶段：放置防御者
    InProgress,   // 进行中：敌人进攻
    Victory,      // 胜利
    Defeat        // 失败
}

/// <summary>
/// 游戏管理器 - 单例模式，管理游戏全局状态
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("游戏状态")]
    [SerializeField] private GameState _currentState = GameState.Preparation;

    /// <summary>
    /// 当前游戏状态（只读）
    /// </summary>
    public GameState CurrentState => _currentState;

    /// <summary>
    /// 状态变更事件 - 当游戏状态改变时触发
    /// </summary>
    public event Action<GameState, GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 切换游戏状态
    /// </summary>
    /// <param name="newState">目标状态</param>
    public void SetState(GameState newState)
    {
        if (_currentState == newState) return;

        GameState oldState = _currentState;
        _currentState = newState;

        Debug.Log($"[GameManager] 状态变更: {oldState} -> {newState}");
        OnGameStateChanged?.Invoke(oldState, newState);
    }

    /// <summary>
    /// 开始游戏（从准备阶段进入进行中）
    /// </summary>
    public void StartGame()
    {
        if (_currentState != GameState.Preparation)
        {
            Debug.LogWarning("[GameManager] 只能在准备阶段开始游戏");
            return;
        }
        SetState(GameState.InProgress);
    }

    /// <summary>
    /// 游戏胜利
    /// </summary>
    public void WinGame()
    {
        if (_currentState != GameState.InProgress)
        {
            Debug.LogWarning("[GameManager] 只有在游戏进行中才能判定胜利");
            return;
        }
        SetState(GameState.Victory);
    }

    /// <summary>
    /// 游戏失败
    /// </summary>
    public void LoseGame()
    {
        if (_currentState != GameState.InProgress)
        {
            Debug.LogWarning("[GameManager] 只有在游戏进行中才能判定失败");
            return;
        }
        SetState(GameState.Defeat);
    }

    /// <summary>
    /// 重置游戏到准备阶段
    /// </summary>
    public void ResetGame()
    {
        SetState(GameState.Preparation);
    }
}
