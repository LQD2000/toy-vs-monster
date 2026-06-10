using UnityEngine;
using System;

[RequireComponent(typeof(Health))]
public class Attacker : MonoBehaviour
{
    [Header("运行时数据")]
    [SerializeField] private AttackerData _data;

    private Health _health;
    private int _currentLane;

    public AttackerData Data => _data;
    public int CurrentHealth => _health.CurrentHealth;
    public int CurrentLane => _currentLane;
    public bool IsDead => _health.IsDead;

    public event Action<Attacker> OnAttackerDead;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDead += HandleDeath;
    }

    public void Initialize(AttackerData data, int lane)
    {
        _data = data;
        _currentLane = lane;

        if (_health == null)
        {
            _health = GetComponent<Health>();
            _health.OnDead += HandleDeath;
        }
        _health.Initialize(data.MaxHealth);
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);
    }

    private void HandleDeath()
    {
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
