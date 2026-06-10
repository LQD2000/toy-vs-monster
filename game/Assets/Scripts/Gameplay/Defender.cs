using UnityEngine;

[RequireComponent(typeof(Health))]
public abstract class Defender : MonoBehaviour
{
    [Header("运行时数据")]
    [SerializeField] private DefenderData _data;

    private Health _health;
    private int _currentRow;
    private int _currentCol;

    public DefenderData Data => _data;
    public int CurrentHealth => _health.CurrentHealth;
    public int CurrentRow => _currentRow;
    public int CurrentCol => _currentCol;
    public bool IsDead => _health.IsDead;

    public event System.Action<Defender> OnDefenderDead;

    protected virtual void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDead += HandleDeath;
    }

    public virtual void Initialize(DefenderData data, int row, int col)
    {
        _data = data;
        _currentRow = row;
        _currentCol = col;

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
        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ReleaseCell(_currentRow, _currentCol);
        }

        OnDefenderDead?.Invoke(this);

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return("Defender", gameObject);
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
