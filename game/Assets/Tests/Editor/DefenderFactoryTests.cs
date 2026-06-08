using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class DefenderFactoryTests
{
    private GameObject _factoryGo;
    private DefenderFactory _factory;
    private GameObject _gridGo;
    private GridSystem _gridSystem;

    [SetUp]
    public void SetUp()
    {
        _gridGo = new GameObject();
        _gridSystem = _gridGo.AddComponent<GridSystem>();
        LogAssert.Expect(LogType.Log, "[GridSystem] 网格初始化完成: 5 行 x 10 列, 原点: (-6.00, -4.00, 0.00)");
        SingletonTestHelper.InvokeAwake(_gridSystem);

        _factoryGo = new GameObject();
        _factory = _factoryGo.AddComponent<DefenderFactory>();
        SingletonTestHelper.InvokeAwake(_factory);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_factoryGo);
        UnityEngine.Object.DestroyImmediate(_gridGo);
        SingletonTestHelper.ResetSingleton<DefenderFactory>();
        SingletonTestHelper.ResetSingleton<GridSystem>();
    }

    [Test]
    public void CreateDefender_WithNullData_ShouldReturnNull()
    {
        LogAssert.Expect(LogType.Error, "[DefenderFactory] DefenderData 或 Prefab 为空");
        Assert.IsNull(_factory.CreateDefender(null, 0, 0));
    }

    [Test]
    public void CreateDefender_WithNullPrefab_ShouldReturnNull()
    {
        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();
        LogAssert.Expect(LogType.Error, "[DefenderFactory] DefenderData 或 Prefab 为空");
        Assert.IsNull(_factory.CreateDefender(data, 0, 0));
        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void CreateDefender_WithValidData_ShouldReturnDefender()
    {
        GameObject prefab = new GameObject("TestPrefab");
        prefab.AddComponent<MarbleShooter>();
        prefab.SetActive(false);

        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();
        var prefabField = typeof(DefenderData).GetField("_prefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prefabField?.SetValue(data, prefab);

        Defender result = _factory.CreateDefender(data, 2, 3);

        Assert.IsNotNull(result);
        Assert.AreEqual(data.MaxHealth, result.CurrentHealth);
        Assert.AreEqual(2, result.CurrentRow);
        Assert.AreEqual(3, result.CurrentCol);

        UnityEngine.Object.DestroyImmediate(result.gameObject);
        UnityEngine.Object.DestroyImmediate(prefab);
        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void CreateDefender_WithoutDefenderComponent_ShouldReturnNull()
    {
        GameObject prefab = new GameObject("BadPrefab");
        prefab.SetActive(false);

        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();
        var prefabField = typeof(DefenderData).GetField("_prefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        prefabField?.SetValue(data, prefab);

        LogAssert.Expect(LogType.Error, "[DefenderFactory] Prefab 上没有 Defender 组件");
        Assert.IsNull(_factory.CreateDefender(data, 0, 0));

        UnityEngine.Object.DestroyImmediate(prefab);
        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void GetAvailableDefenders_ShouldReturnArray()
    {
        Assert.IsNotNull(_factory.GetAvailableDefenders());
    }
}
