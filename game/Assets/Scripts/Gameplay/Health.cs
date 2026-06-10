using UnityEngine;
using System;

/// <summary>
/// 血量组件 - 可挂载到任意 GameObject 的通用血量管理
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    private int _currentHealth;
    private bool _isDead;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public bool IsDead => _isDead;
    public float HealthPercent => _maxHealth > 0 ? (float)_currentHealth / _maxHealth : 0f;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDead;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    public void Initialize(int maxHealth)
    {
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        OnDead?.Invoke();
    }
}
