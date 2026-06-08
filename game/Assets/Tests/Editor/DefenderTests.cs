using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class DefenderTests
{
    private GameObject _defenderGo;
    private Defender _defender;
    private DefenderData _testData;

    [SetUp]
    public void SetUp()
    {
        _testData = ScriptableObject.CreateInstance<DefenderData>();
        _defenderGo = new GameObject();
        _defender = _defenderGo.AddComponent<MarbleShooter>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_defenderGo);
        UnityEngine.Object.DestroyImmediate(_testData);
    }

    [Test]
    public void Initialize_ShouldSetHealthAndPosition()
    {
        _defender.Initialize(_testData, 2, 5);
        Assert.AreEqual(_testData.MaxHealth, _defender.CurrentHealth);
        Assert.AreEqual(2, _defender.CurrentRow);
        Assert.AreEqual(5, _defender.CurrentCol);
        Assert.IsFalse(_defender.IsDead);
    }

    [Test]
    public void TakeDamage_ShouldReduceHealth()
    {
        _defender.Initialize(_testData, 0, 0);
        int initial = _defender.CurrentHealth;
        _defender.TakeDamage(30);
        Assert.AreEqual(initial - 30, _defender.CurrentHealth);
        Assert.IsFalse(_defender.IsDead);
    }

    [Test]
    public void TakeDamage_ExactKill_ShouldDie()
    {
        _defender.Initialize(_testData, 0, 0);
        _defender.TakeDamage(_defender.CurrentHealth);
        Assert.AreEqual(0, _defender.CurrentHealth);
        Assert.IsTrue(_defender.IsDead);
    }

    [Test]
    public void TakeDamage_WhenDead_ShouldNotChangeHealth()
    {
        _defender.Initialize(_testData, 0, 0);
        _defender.TakeDamage(_defender.CurrentHealth);
        Assert.IsTrue(_defender.IsDead);
        _defender.TakeDamage(50);
        Assert.AreEqual(0, _defender.CurrentHealth);
    }

    [Test]
    public void Die_ShouldFireOnDefenderDeadEvent()
    {
        _defender.Initialize(_testData, 0, 0);
        Defender deadDefender = null;
        _defender.OnDefenderDead += (d) => deadDefender = d;
        _defender.TakeDamage(_defender.CurrentHealth);
        Assert.IsNotNull(deadDefender);
        Assert.AreSame(_defender, deadDefender);
    }

    [Test]
    public void MultipleDamage_ShouldAccumulateCorrectly()
    {
        _defender.Initialize(_testData, 0, 0);
        int initial = _defender.CurrentHealth;
        _defender.TakeDamage(10);
        _defender.TakeDamage(20);
        _defender.TakeDamage(30);
        Assert.AreEqual(initial - 60, _defender.CurrentHealth);
    }
}
