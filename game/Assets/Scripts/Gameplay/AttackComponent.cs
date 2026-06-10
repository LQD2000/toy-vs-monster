using UnityEngine;

/// <summary>
/// 攻击组件 - 通用攻击逻辑，可挂载到任意防御者
/// 负责检测敌人、发射弹丸、冷却管理
/// </summary>
public class AttackComponent : MonoBehaviour
{
    [Header("攻击配置")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private string _projectilePoolName = "MarbleProjectile";

    private int _attackPower;
    private float _attackSpeed;
    private int _range;
    private int _currentRow;
    private float _attackTimer;
    private bool _initialized;

    public void Initialize(int attackPower, float attackSpeed, int range, int row)
    {
        _attackPower = attackPower;
        _attackSpeed = attackSpeed;
        _range = range;
        _currentRow = row;
        _initialized = true;
    }

    private void Update()
    {
        if (!_initialized) return;

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= 1f / _attackSpeed)
        {
            _attackTimer = 0f;
            if (HasTargetInRange())
            {
                TryAttack();
            }
        }
    }

    private bool HasTargetInRange()
    {
        if (_range == -1) return true;

        if (GridSystem.Instance == null) return false;

        float rangeWorld = _range * GridSystem.Instance.CellWidth;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyGo in enemies)
        {
            float distanceX = enemyGo.transform.position.x - transform.position.x;
            if (distanceX > 0f && distanceX <= rangeWorld)
            {
                Enemy enemy = enemyGo.GetComponent<Enemy>();
                if (enemy != null && enemy.CurrentRow == _currentRow && !enemy.IsDead)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void TryAttack()
    {
        if (ObjectPool.Instance == null) return;

        Vector3 spawnPosition = _firePoint != null ? _firePoint.position : transform.position;

        GameObject projectileObj = ObjectPool.Instance.Get(
            _projectilePoolName,
            spawnPosition,
            Quaternion.identity
        );

        if (projectileObj != null)
        {
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(_attackPower, _currentRow);
            }
        }
    }
}
