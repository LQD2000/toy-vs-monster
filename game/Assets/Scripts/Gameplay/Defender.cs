using UnityEngine;

/// <summary>
/// 防守方基类 - 所有防守方单位的父类
/// 管理单位生命周期、受伤、死亡
/// </summary>
public abstract class Defender : MonoBehaviour
{
    [Header("运行时数据")]
    [SerializeField] private DefenderData _data;

    private int _currentHealth;
    private int _currentRow;
    private int _currentCol;
    private bool _isDead;

    public DefenderData Data => _data;
    public int CurrentHealth => _currentHealth;
    public int CurrentRow => _currentRow;
    public int CurrentCol => _currentCol;
    public bool IsDead => _isDead;

    public event System.Action<Defender> OnDefenderDead;

    protected virtual void Awake()
    {
    }

    public virtual void Initialize(DefenderData data, int row, int col)
    {
        _data = data;
        _currentHealth = data.MaxHealth;
        _currentRow = row;
        _currentCol = col;
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

    protected virtual void Die()
    {
        if (_isDead) return;
        _isDead = true;

        if (GridSystem.Instance != null)
        {
            GridSystem.Instance.ReleaseCell(_currentRow, _currentCol);
        }

        OnDefenderDead?.Invoke(this);
        
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
