using UnityEngine;

/// <summary>
/// 弹珠弹丸 - 沿跑道直线飞行，命中第一个敌人造成伤害
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    private int _damage;
    private int _laneRow;
    private bool _initialized;

    public void Initialize(int damage, int laneRow)
    {
        _damage = damage;
        _laneRow = laneRow;
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized) return;

        transform.position += Vector3.right * _speed * Time.deltaTime;

        float rightBoundary = GetRightBoundary();
        if (transform.position.x > rightBoundary)
        {
            ReturnToPool();
        }
    }

    private float GetRightBoundary()
    {
        if (GridSystem.Instance != null)
        {
            return GridSystem.Instance.GridOrigin.x
                + GridSystem.Instance.Cols * GridSystem.Instance.CellWidth
                + GridSystem.Instance.CellWidth;
        }
        return 10f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_initialized) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy.CurrentRow == _laneRow)
            {
                DamageCalculator.DamageResult result = DamageCalculator.CalculateDamage(_damage);
                enemy.TakeDamage(result.finalDamage);
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        _initialized = false;
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return("MarbleProjectile", gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
