using NUnit.Framework;
using UnityEngine;

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

        _managerGameObject = new GameObject();
        _laneManager = _managerGameObject.AddComponent<LaneManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_managerGameObject);
        Object.DestroyImmediate(_gridGameObject);
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

        _laneManager.RegisterCar(0, car);

        Assert.IsTrue(_laneManager.IsCarAvailable(0));

        Object.DestroyImmediate(carObj);
    }

    [Test]
    public void UnregisterCar_ShouldMakeCarUnavailable()
    {
        var carObj = new GameObject();
        var car = carObj.AddComponent<LaneCar>();

        _laneManager.RegisterCar(0, car);
        _laneManager.UnregisterCar(0);

        Assert.IsFalse(_laneManager.IsCarAvailable(0));

        Object.DestroyImmediate(carObj);
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

        _laneManager.RegisterCar(0, car);

        int? triggeredLane = null;
        _laneManager.OnLaneCarTriggered += (lane, _) => triggeredLane = lane;

        _laneManager.TriggerCar(0);

        Assert.AreEqual(0, triggeredLane);
    }

    [Test]
    public void OnAllCarsTriggered_ShouldFireWhenAllCarsTriggered()
    {
        var carObj = new GameObject();
        carObj.AddComponent<SpriteRenderer>();
        var car = carObj.AddComponent<LaneCar>();

        _laneManager.RegisterCar(0, car);

        bool allTriggered = false;
        _laneManager.OnAllCarsTriggered += () => allTriggered = true;

        _laneManager.TriggerCar(0);

        Assert.IsTrue(allTriggered);
    }

    [Test]
    public void ClearLane_ShouldNotThrow()
    {
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
