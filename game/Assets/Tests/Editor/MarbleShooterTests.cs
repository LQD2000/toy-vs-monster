using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class MarbleShooterTests
{
    private GameObject _shooterGo;
    private MarbleShooter _shooter;
    private DefenderData _testData;

    [SetUp]
    public void SetUp()
    {
        _testData = ScriptableObject.CreateInstance<DefenderData>();

        var speedField = typeof(DefenderData).GetField("_attackSpeed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        speedField?.SetValue(_testData, 1f);

        _shooterGo = new GameObject();
        var firePointGo = new GameObject("FirePoint");
        firePointGo.transform.SetParent(_shooterGo.transform);

        _shooter = _shooterGo.AddComponent<MarbleShooter>();

        var fpField = typeof(MarbleShooter).GetField("_firePoint",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fpField?.SetValue(_shooter, firePointGo.transform);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_shooterGo);
        UnityEngine.Object.DestroyImmediate(_testData);
    }

    [Test]
    public void Initialize_ShouldSetDataAndHealth()
    {
        _shooter.Initialize(_testData, 1, 3);
        Assert.AreEqual(_testData, _shooter.Data);
        Assert.AreEqual(_testData.MaxHealth, _shooter.CurrentHealth);
        Assert.AreEqual(1, _shooter.CurrentRow);
        Assert.AreEqual(3, _shooter.CurrentCol);
    }

    [Test]
    public void Update_WithNullData_ShouldNotThrow()
    {
        Assert.Throws<NullReferenceException>(() => _shooter.Initialize(null, 0, 0));
    }

    [Test]
    public void IsDead_AfterFatalDamage_ShouldBeTrue()
    {
        _shooter.Initialize(_testData, 0, 0);
        _shooter.TakeDamage(_testData.MaxHealth);
        Assert.IsTrue(_shooter.IsDead);
    }
}
