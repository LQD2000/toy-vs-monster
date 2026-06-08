using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class LaneManagerTests
{
    private GameObject _gridGameObject;
    private GridSystem _gridSystem;
    private GameObject _managerGameObject;
    private LaneManager _laneManager;

    [SetUp]
    public void SetUp()
    {
        _gridGameObject = new GameObject();
        _gridSystem = _gridGameObject.AddComponent<GridSystem>();
        LogAssert.Expect(LogType.Log, "[GridSystem] 网格初始化完成: 5 行 x 10 列, 原点: (-6.00, -4.00, 0.00)");
        SingletonTestHelper.InvokeAwake(_gridSystem);

        _managerGameObject = new GameObject();
        _laneManager = _managerGameObject.AddComponent<LaneManager>();
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道管理器初始化, 跑道数量: 1");
        SingletonTestHelper.InvokeAwake(_laneManager);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_managerGameObject);
        UnityEngine.Object.DestroyImmediate(_gridGameObject);
        SingletonTestHelper.ResetSingleton<LaneManager>();
        SingletonTestHelper.ResetSingleton<GridSystem>();
    }

    [Test]
    public void InitialState_ShouldHaveLaneCount1()
    {
        Assert.AreEqual(1, _laneManager.LaneCount);
    }

    [Test]
    public void RegisterCar_ShouldMakeCarAvailable()
    {
        var carObj = new GameObject();
        var car = carObj.AddComponent<LaneCar>();

        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, car);

        Assert.IsTrue(_laneManager.IsCarAvailable(0));

        UnityEngine.Object.DestroyImmediate(carObj);
    }

    [Test]
    public void UnregisterCar_ShouldMakeCarUnavailable()
    {
        var carObj = new GameObject();
        var car = carObj.AddComponent<LaneCar>();

        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, car);
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注销");
        _laneManager.UnregisterCar(0);

        Assert.IsFalse(_laneManager.IsCarAvailable(0));

        UnityEngine.Object.DestroyImmediate(carObj);
    }

    [Test]
    public void IsCarAvailable_NoCar_ShouldReturnFalse()
    {
        Assert.IsFalse(_laneManager.IsCarAvailable(0));
    }

    [Test]
    public void TriggerCar_ShouldTriggerCarAndFireEvent()
    {
        var carObj = new GameObject();
        carObj.AddComponent<SpriteRenderer>();
        var car = carObj.AddComponent<LaneCar>();

        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, car);

        int? triggeredLane = null;
        _laneManager.OnLaneCarTriggered += (lane, _) => triggeredLane = lane;

        LogAssert.Expect(LogType.Log, "[LaneCar] 小车 0 触发！清除跑道全部怪物");
        LogAssert.Expect(LogType.Log, "[GridSystem] 已释放第 0 行所有格子");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        LogAssert.Expect(LogType.Log, "[LaneManager] 所有小车已消耗！");
        _laneManager.TriggerCar(0);

        Assert.AreEqual(0, triggeredLane);
    }

    [Test]
    public void OnAllCarsTriggered_ShouldFireWhenAllCarsTriggered()
    {
        var carObj = new GameObject();
        carObj.AddComponent<SpriteRenderer>();
        var car = carObj.AddComponent<LaneCar>();

        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, car);

        bool allTriggered = false;
        _laneManager.OnAllCarsTriggered += () => allTriggered = true;

        LogAssert.Expect(LogType.Log, "[LaneCar] 小车 0 触发！清除跑道全部怪物");
        LogAssert.Expect(LogType.Log, "[GridSystem] 已释放第 0 行所有格子");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        LogAssert.Expect(LogType.Log, "[LaneManager] 所有小车已消耗！");
        _laneManager.TriggerCar(0);

        Assert.IsTrue(allTriggered);
    }

    [Test]
    public void ClearLane_ShouldNotThrow()
    {
        LogAssert.Expect(LogType.Log, "[GridSystem] 已释放第 0 行所有格子");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        Assert.DoesNotThrow(() => _laneManager.ClearLane(0));
    }

    [Test]
    public void GetCarPosition_ShouldReturnPositionLeftOfFirstCell()
    {
        Vector3 carPos = _laneManager.GetCarPosition(0);
        Vector3 cell0Pos = _gridSystem.GridToWorld(0, 0);

        Assert.Less(carPos.x, cell0Pos.x);
        Assert.AreEqual(cell0Pos.y, carPos.y, 0.001f);
    }

    [Test]
    public void GetLaneBounds_ShouldReturnCorrectRange()
    {
        var bounds = _laneManager.GetLaneBounds(0);
        Vector3 cell0Pos = _gridSystem.GridToWorld(0, 0);
        Vector3 cell9Pos = _gridSystem.GridToWorld(0, 9);

        Assert.Less(bounds.minX, cell0Pos.x);
        Assert.Greater(bounds.maxX, cell9Pos.x);
        Assert.AreEqual(cell0Pos.y, bounds.centerY, 0.001f);
    }
}
