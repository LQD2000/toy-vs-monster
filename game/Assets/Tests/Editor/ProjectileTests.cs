using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class ProjectileTests
{
    private GameObject _projectileGo;
    private Projectile _projectile;
    private GameObject _gridGo;
    private GridSystem _gridSystem;

    [SetUp]
    public void SetUp()
    {
        _gridGo = new GameObject();
        _gridSystem = _gridGo.AddComponent<GridSystem>();
        LogAssert.Expect(LogType.Log, "[GridSystem] 网格初始化完成: 5 行 x 10 列, 原点: (-6.00, -4.00, 0.00)");
        SingletonTestHelper.InvokeAwake(_gridSystem);

        _projectileGo = new GameObject();
        _projectile = _projectileGo.AddComponent<Projectile>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_projectileGo);
        UnityEngine.Object.DestroyImmediate(_gridGo);
        SingletonTestHelper.ResetSingleton<GridSystem>();
    }

    [Test]
    public void Initialize_ShouldSetDamageAndLane()
    {
        _projectile.Initialize(20, 2);
        Assert.IsTrue(IsProjectileInitialized());
    }

    [Test]
    public void Initialize_MultipleTimes_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() =>
        {
            _projectile.Initialize(10, 0);
            _projectile.Initialize(20, 1);
            _projectile.Initialize(30, 2);
        });
    }

    [Test]
    public void GetRightBoundary_WithGridSystem_ShouldReturnPositive()
    {
        var method = typeof(Projectile).GetMethod("GetRightBoundary",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsNotNull(method);
        float boundary = (float)method.Invoke(_projectile, null);
        Assert.Greater(boundary, 0f);
    }

    private bool IsProjectileInitialized()
    {
        var field = typeof(Projectile).GetField("_initialized",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null && (bool)field.GetValue(_projectile);
    }
}
