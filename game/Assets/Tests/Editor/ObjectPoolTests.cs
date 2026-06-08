using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class ObjectPoolTests
{
    private GameObject _gameObject;
    private ObjectPool _objectPool;
    private GameObject _prefab;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _objectPool = _gameObject.AddComponent<ObjectPool>();
        SingletonTestHelper.InvokeAwake(_objectPool);

        _prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _prefab.SetActive(false);
    }

    [TearDown]
    public void TearDown()
    {
        if (_prefab != null)
            UnityEngine.Object.DestroyImmediate(_prefab);

        UnityEngine.Object.DestroyImmediate(_gameObject);
        SingletonTestHelper.ResetSingleton<ObjectPool>();
    }

    private ObjectPool.PoolConfig CreateTestConfig(string poolName, int initialSize = 10, bool autoExpand = true)
    {
        return new ObjectPool.PoolConfig
        {
            poolName = poolName,
            prefab = _prefab,
            initialSize = initialSize,
            autoExpand = autoExpand
        };
    }

    [Test]
    public void CreatePool_ShouldPreInstantiateObjects()
    {
        var config = CreateTestConfig("Bullet", initialSize: 5);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 5");
        _objectPool.CreatePool(config);

        Assert.AreEqual(5, _objectPool.GetCount("Bullet"));
    }

    [Test]
    public void CreatePool_DuplicateName_ShouldNotCreateDuplicate()
    {
        var config = CreateTestConfig("Orb", initialSize: 3);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Orb', 初始大小: 3");
        _objectPool.CreatePool(config);
        LogAssert.Expect(LogType.Warning, "[ObjectPool] 对象池 'Orb' 已存在");
        _objectPool.CreatePool(config);

        Assert.AreEqual(3, _objectPool.GetCount("Orb"),
            "Duplicate pool creation should not affect existing pool");
    }

    [Test]
    public void Get_ShouldReturnActiveObject()
    {
        var config = CreateTestConfig("Bullet");
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 10");
        _objectPool.CreatePool(config);

        var obj = _objectPool.Get("Bullet", Vector3.one, Quaternion.identity);

        Assert.IsNotNull(obj);
        Assert.IsTrue(obj.activeSelf);
        Assert.AreEqual(Vector3.one, obj.transform.position);
    }

    [Test]
    public void Get_ShouldDecrementPoolCount()
    {
        var config = CreateTestConfig("Bullet", initialSize: 5);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 5");
        _objectPool.CreatePool(config);

        var obj = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);

        Assert.AreEqual(4, _objectPool.GetCount("Bullet"));
    }

    [Test]
    public void Return_ShouldDeactivateAndReparent()
    {
        var config = CreateTestConfig("Bullet");
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 10");
        _objectPool.CreatePool(config);
        var obj = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);

        _objectPool.Return("Bullet", obj);

        Assert.IsFalse(obj.activeSelf);
        Assert.AreEqual(_gameObject.transform, obj.transform.parent);
        Assert.AreEqual(10, _objectPool.GetCount("Bullet"), "Count should return to initial after Return");
    }

    [Test]
    public void GetAfterReturn_ShouldReuseSameObject()
    {
        var config = CreateTestConfig("Bullet", initialSize: 1);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 1");
        _objectPool.CreatePool(config);
        var obj1 = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);

        _objectPool.Return("Bullet", obj1);
        var obj2 = _objectPool.Get("Bullet", Vector3.one, Quaternion.identity);

        Assert.AreSame(obj1, obj2, "Should reuse the same object instance");
        Assert.AreEqual(Vector3.one, obj2.transform.position, "Position should be updated on reuse");
    }

    [Test]
    public void Get_WhenPoolEmpty_WithAutoExpand_ShouldCreateNewObject()
    {
        var config = CreateTestConfig("Bullet", initialSize: 0, autoExpand: true);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 0");
        _objectPool.CreatePool(config);

        LogAssert.Expect(LogType.Log, "[ObjectPool] 对象池 'Bullet' 自动扩展");
        var obj = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);

        Assert.IsNotNull(obj);
        Assert.IsTrue(obj.activeSelf);
        Assert.AreEqual(0, _objectPool.GetCount("Bullet"),
            "After auto-expand, pool count should still be 0 (object not in pool)");
    }

    [Test]
    public void Get_WhenPoolEmpty_WithoutAutoExpand_ShouldReturnNull()
    {
        var config = CreateTestConfig("Bullet", initialSize: 0, autoExpand: false);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 0");
        _objectPool.CreatePool(config);

        LogAssert.Expect(LogType.Warning, "[ObjectPool] 对象池 'Bullet' 已空且未启用自动扩展");
        var obj = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);

        Assert.IsNull(obj);
    }

    [Test]
    public void Get_NonExistentPool_ShouldReturnNull()
    {
        LogAssert.Expect(LogType.Error, "[ObjectPool] 对象池 'NonExistent' 不存在");
        var obj = _objectPool.Get("NonExistent", Vector3.zero, Quaternion.identity);

        Assert.IsNull(obj);
    }

    [Test]
    public void Return_NonExistentPool_ShouldDestroyObject()
    {
        var obj = InstantiateForTest();
        Assert.IsTrue(obj != null);

        LogAssert.Expect(LogType.Warning, "[ObjectPool] 对象池 'NonExistent' 不存在，直接销毁对象");
        _objectPool.Return("NonExistent", obj);

        // Object should be destroyed (Unity fake-null after DestroyImmediate)
        Assert.IsTrue(obj == null, "Object should be destroyed when returned to non-existent pool");
    }

    [Test]
    public void ClearPool_ShouldDestroyAllObjectsInPool()
    {
        var config = CreateTestConfig("Bullet", initialSize: 3);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 3");
        _objectPool.CreatePool(config);

        var obj1 = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);
        var obj2 = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);
        _objectPool.Return("Bullet", obj1);
        _objectPool.Return("Bullet", obj2);

        _objectPool.ClearPool("Bullet");

        Assert.IsTrue(obj1 == null, "Returned object in pool should be destroyed");
        Assert.IsTrue(obj2 == null, "Returned object in pool should be destroyed");
    }

    [Test]
    public void ClearAllPools_ShouldDestroyAllObjects()
    {
        var config1 = CreateTestConfig("Bullet", initialSize: 3);
        var config2 = CreateTestConfig("Orb", initialSize: 2);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 3");
        _objectPool.CreatePool(config1);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Orb', 初始大小: 2");
        _objectPool.CreatePool(config2);

        _objectPool.ClearAllPools();

        Assert.AreEqual(0, _objectPool.GetCount("Bullet"));
        Assert.AreEqual(0, _objectPool.GetCount("Orb"));
    }

    [Test]
    public void GetCount_NonExistentPool_ShouldReturnZero()
    {
        Assert.AreEqual(0, _objectPool.GetCount("NonExistent"));
    }

    [Test]
    public void MultiplePools_ShouldBeIndependent()
    {
        var bulletConfig = CreateTestConfig("Bullet", initialSize: 5);
        var orbConfig = CreateTestConfig("Orb", initialSize: 3);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 5");
        _objectPool.CreatePool(bulletConfig);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Orb', 初始大小: 3");
        _objectPool.CreatePool(orbConfig);

        var bullet = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);

        Assert.AreEqual(4, _objectPool.GetCount("Bullet"), "Bullet pool count should decrease");
        Assert.AreEqual(3, _objectPool.GetCount("Orb"), "Orb pool count should remain unchanged");
    }

    [Test]
    public void AutoExpand_ShouldCreateMultipleObjects_WhenPoolEmpty()
    {
        var config = CreateTestConfig("Bullet", initialSize: 0, autoExpand: true);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 创建对象池 'Bullet', 初始大小: 0");
        _objectPool.CreatePool(config);

        LogAssert.Expect(LogType.Log, "[ObjectPool] 对象池 'Bullet' 自动扩展");
        var obj1 = _objectPool.Get("Bullet", Vector3.zero, Quaternion.identity);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 对象池 'Bullet' 自动扩展");
        var obj2 = _objectPool.Get("Bullet", Vector3.one, Quaternion.identity);
        LogAssert.Expect(LogType.Log, "[ObjectPool] 对象池 'Bullet' 自动扩展");
        var obj3 = _objectPool.Get("Bullet", Vector3.one * 2, Quaternion.identity);

        Assert.IsNotNull(obj1);
        Assert.IsNotNull(obj2);
        Assert.IsNotNull(obj3);
        Assert.AreNotSame(obj1, obj2, "Auto-expanded objects should be distinct instances");
    }

    private GameObject InstantiateForTest()
    {
        var obj = Object.Instantiate(_prefab);
        return obj;
    }
}
