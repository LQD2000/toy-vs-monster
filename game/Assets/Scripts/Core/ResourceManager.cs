using UnityEngine;
using System;

/// <summary>
/// 资源管理器 - 管理电力等游戏资源
/// 单例模式
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("电力配置")]
    [SerializeField] private int _maxPower = 200;
    [SerializeField] private int _currentPower;

    public int MaxPower => _maxPower;
    public int CurrentPower => _currentPower;

    public event Action<int, int> OnPowerChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // DontDestroyOnLoad 只能在 Play Mode 下使用
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        _currentPower = _maxPower;
    }

    public bool HasEnoughPower(int cost)
    {
        return _currentPower >= cost;
    }

    public bool ConsumePower(int cost)
    {
        if (!HasEnoughPower(cost))
        {
            return false;
        }

        _currentPower -= cost;
        OnPowerChanged?.Invoke(_currentPower, _maxPower);
        return true;
    }

    public void AddPower(int amount)
    {
        _currentPower = Mathf.Min(_currentPower + amount, _maxPower);
        OnPowerChanged?.Invoke(_currentPower, _maxPower);
    }

    public void ResetPower()
    {
        _currentPower = _maxPower;
        OnPowerChanged?.Invoke(_currentPower, _maxPower);
    }
}
