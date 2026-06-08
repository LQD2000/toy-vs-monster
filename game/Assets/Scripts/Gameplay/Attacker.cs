using UnityEngine;
using System;

/// <summary>
/// 进攻方基类 - 所有进攻方单位的父类
/// 管理单位生命周期、受伤、死亡
/// </summary>
public class Attacker : MonoBehaviour
{
    [Header("运行时数据")]
    [SerializeField] private AttackerData _data;

    private int _currentHealth;
    private int _currentLane;
    private bool _isDead;

    public AttackerData Data => _data;
    public int CurrentHealth => _currentHealth;
    public int CurrentLane => _currentLane;
    public bool IsDead => _isDead;

    public event Action<Attacker> OnAttackerDead;

    public void Initialize(AttackerData data, int lane)
    {
        _data = data;
        _currentHealth = data.MaxHealth;
        _currentLane = lane;
        _isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Debug.Log($"[Attacker] 怪物死亡: {_data?.AttackerName} (Lane: {_currentLane})");
        OnAttackerDead?.Invoke(this);

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return("Attacker", gameObject);
        }
        else
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
