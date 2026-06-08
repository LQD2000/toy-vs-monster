using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    private int _currentRow;

    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public int CurrentRow => _currentRow;
    public bool IsDead => _currentHealth <= 0;

    public event Action<Enemy> OnEnemyDead;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void Initialize(int health, int row)
    {
        _maxHealth = health;
        _currentHealth = health;
        _currentRow = row;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[Enemy] 敌人死亡 (Row: {_currentRow})");
        OnEnemyDead?.Invoke(this);

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return("Enemy", gameObject);
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
