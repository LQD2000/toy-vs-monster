using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

[TestFixture]
public class AttackerTests
{
    private GameObject _attackerGo;
    private Attacker _attacker;
    private AttackerData _testData;

    [SetUp]
    public void SetUp()
    {
        _testData = ScriptableObject.CreateInstance<AttackerData>();
        _testData.name = "TestAttackerData";
        var so = new SerializedObject(_testData);
        so.FindProperty("_attackerName").stringValue = "TestAttackerData";
        so.ApplyModifiedPropertiesWithoutUndo();
        _attackerGo = new GameObject();
        _attacker = _attackerGo.AddComponent<Attacker>();
        SingletonTestHelper.InvokeAwake(_attacker);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_attackerGo);
        if (_testData != null)
        {
            UnityEngine.Object.DestroyImmediate(_testData);
        }
    }

    [Test]
    public void Initialize_ShouldSetHealthAndLane()
    {
        _attacker.Initialize(_testData, 2);
        Assert.AreEqual(_testData.MaxHealth, _attacker.CurrentHealth);
        Assert.AreEqual(2, _attacker.CurrentLane);
        Assert.IsFalse(_attacker.IsDead);
    }

    [Test]
    public void TakeDamage_ShouldReduceHealth()
    {
        _attacker.Initialize(_testData, 0);
        int initial = _attacker.CurrentHealth;
        _attacker.TakeDamage(30);
        Assert.AreEqual(initial - 30, _attacker.CurrentHealth);
        Assert.IsFalse(_attacker.IsDead);
    }

    [Test]
    public void TakeDamage_ExactKill_ShouldDie()
    {
        LogAssert.Expect(LogType.Log, "[Attacker] 怪物死亡: TestAttackerData (Lane: 0)");
        _attacker.Initialize(_testData, 0);
        _attacker.TakeDamage(_attacker.CurrentHealth);
        Assert.AreEqual(0, _attacker.CurrentHealth);
        Assert.IsTrue(_attacker.IsDead);
    }

    [Test]
    public void TakeDamage_WhenDead_ShouldNotChangeHealth()
    {
        LogAssert.Expect(LogType.Log, "[Attacker] 怪物死亡: TestAttackerData (Lane: 0)");
        _attacker.Initialize(_testData, 0);
        _attacker.TakeDamage(_attacker.CurrentHealth);
        Assert.IsTrue(_attacker.IsDead);
        _attacker.TakeDamage(50);
        Assert.AreEqual(0, _attacker.CurrentHealth);
    }

    [Test]
    public void Die_ShouldFireOnAttackerDeadEvent()
    {
        _attacker.Initialize(_testData, 0);
        Attacker deadAttacker = null;
        _attacker.OnAttackerDead += (a) => deadAttacker = a;
        LogAssert.Expect(LogType.Log, "[Attacker] 怪物死亡: TestAttackerData (Lane: 0)");
        _attacker.TakeDamage(_attacker.CurrentHealth);
        Assert.IsNotNull(deadAttacker);
        Assert.AreSame(_attacker, deadAttacker);
    }

    [Test]
    public void MultipleDamage_ShouldAccumulateCorrectly()
    {
        _attacker.Initialize(_testData, 0);
        int initial = _attacker.CurrentHealth;
        _attacker.TakeDamage(10);
        _attacker.TakeDamage(20);
        _attacker.TakeDamage(30);
        Assert.AreEqual(initial - 60, _attacker.CurrentHealth);
    }

    [Test]
    public void Data_ShouldReturnInitializedData()
    {
        _attacker.Initialize(_testData, 1);
        Assert.AreSame(_testData, _attacker.Data);
    }

    [Test]
    public void CurrentLane_ShouldReturnInitializedLane()
    {
        _attacker.Initialize(_testData, 3);
        Assert.AreEqual(3, _attacker.CurrentLane);
    }
}
