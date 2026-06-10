using UnityEngine;
using System;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    private Health _health;
    private int _currentRow;

    public int MaxHealth => _health.MaxHealth;
    public int CurrentHealth => _health.CurrentHealth;
    public int CurrentRow => _currentRow;
    public bool IsDead => _health.IsDead;

    public event Action<Enemy> OnEnemyDead;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.Initialize(_maxHealth);
        _health.OnDead += HandleDeath;
    }

    public void Initialize(int health, int row)
    {
        if (_health == null)
        {
            _health = GetComponent<Health>();
            _health.OnDead += HandleDeath;
        }
        _health.Initialize(health);
        _currentRow = row;
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);
    }

    private void HandleDeath()
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
