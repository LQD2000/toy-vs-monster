using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class EnemyTests
{
    private GameObject _enemyGo;
    private Enemy _enemy;

    [SetUp]
    public void SetUp()
    {
        _enemyGo = new GameObject();
        _enemy = _enemyGo.AddComponent<Enemy>();
        SingletonTestHelper.InvokeAwake(_enemy);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_enemyGo);
    }

    [Test]
    public void InitialHealth_ShouldEqualMaxHealth()
    {
        Assert.AreEqual(_enemy.MaxHealth, _enemy.CurrentHealth);
    }

    [Test]
    public void Initialize_ShouldSetHealthAndRow()
    {
        _enemy.Initialize(150, 2);
        Assert.AreEqual(150, _enemy.MaxHealth);
        Assert.AreEqual(150, _enemy.CurrentHealth);
        Assert.AreEqual(2, _enemy.CurrentRow);
    }

    [Test]
    public void TakeDamage_ShouldReduceHealth()
    {
        int initial = _enemy.CurrentHealth;
        _enemy.TakeDamage(25);
        Assert.AreEqual(initial - 25, _enemy.CurrentHealth);
        Assert.IsFalse(_enemy.IsDead);
    }

    [Test]
    public void TakeDamage_ExactKill_ShouldDie()
    {
        LogAssert.Expect(LogType.Log, "[Enemy] 敌人死亡 (Row: 0)");
        _enemy.TakeDamage(_enemy.CurrentHealth);
        Assert.AreEqual(0, _enemy.CurrentHealth);
        Assert.IsTrue(_enemy.IsDead);
    }

    [Test]
    public void TakeDamage_WhenDead_ShouldNotReduceHealth()
    {
        LogAssert.Expect(LogType.Log, "[Enemy] 敌人死亡 (Row: 0)");
        _enemy.TakeDamage(_enemy.CurrentHealth);
        _enemy.TakeDamage(50);
        Assert.AreEqual(0, _enemy.CurrentHealth);
    }

    [Test]
    public void Die_ShouldFireOnEnemyDeadEvent()
    {
        Enemy deadEnemy = null;
        _enemy.OnEnemyDead += (e) => deadEnemy = e;
        LogAssert.Expect(LogType.Log, "[Enemy] 敌人死亡 (Row: 0)");
        _enemy.TakeDamage(_enemy.CurrentHealth);
        Assert.IsNotNull(deadEnemy);
        Assert.AreSame(_enemy, deadEnemy);
    }
}
