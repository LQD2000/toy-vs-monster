using UnityEngine;

/// <summary>
/// 弹珠射手 - 发射弹珠攻击跑道上的敌人
/// 继承自 Defender 基类
/// </summary>
public class MarbleShooter : Defender
{
    [Header("弹珠射手配置")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _firePoint;

    private float _attackTimer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Data == null || IsDead) return;

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= 1f / Data.AttackSpeed)
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
        if (Data.Range == -1) return true;

        if (GridSystem.Instance == null) return false;

        float rangeWorld = Data.Range * GridSystem.Instance.CellWidth;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyGo in enemies)
        {
            float distanceX = enemyGo.transform.position.x - transform.position.x;
            if (distanceX > 0f && distanceX <= rangeWorld)
            {
                Enemy enemy = enemyGo.GetComponent<Enemy>();
                if (enemy != null && enemy.CurrentRow == CurrentRow && !enemy.IsDead)
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

        GameObject projectileObj = ObjectPool.Instance.Get(
            "MarbleProjectile",
            _firePoint.position,
            Quaternion.identity
        );

        if (projectileObj != null)
        {
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(Data.AttackPower, CurrentRow);
            }
        }
    }
}
