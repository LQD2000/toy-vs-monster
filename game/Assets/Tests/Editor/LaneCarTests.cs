using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class LaneCarTests
{
    private GameObject _carGameObject;
    private LaneCar _laneCar;
    private GameObject _managerGameObject;
    private LaneManager _laneManager;

    [SetUp]
    public void SetUp()
    {
        _managerGameObject = new GameObject();
        _laneManager = _managerGameObject.AddComponent<LaneManager>();

        _carGameObject = new GameObject();
        _carGameObject.AddComponent<SpriteRenderer>();
        _laneCar = _carGameObject.AddComponent<LaneCar>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_carGameObject);
        Object.DestroyImmediate(_managerGameObject);
        SingletonTestHelper.ResetSingleton<LaneManager>();
    }

    [Test]
    public void InitialState_ShouldNotBeTriggered()
    {
        Assert.IsFalse(_laneCar.IsTriggered);
    }

    [Test]
    public void Trigger_ShouldSetIsTriggeredToTrue()
    {
        _laneManager.RegisterCar(0, _laneCar);

        _laneCar.Trigger();

        Assert.IsTrue(_laneCar.IsTriggered);
    }

    [Test]
    public void Trigger_ShouldFireOnCarTriggeredEvent()
    {
        _laneManager.RegisterCar(0, _laneCar);

        int? triggeredLane = null;
        _laneCar.OnCarTriggered += (lane) => triggeredLane = lane;

        _laneCar.Trigger();

        Assert.AreEqual(0, triggeredLane);
    }

    [Test]
    public void Trigger_CalledTwice_ShouldNotThrow()
    {
        _laneManager.RegisterCar(0, _laneCar);

        _laneCar.Trigger();

        Assert.DoesNotThrow(() => _laneCar.Trigger());
    }

    [Test]
    public void Trigger_CalledTwice_ShouldOnlyTriggerOnce()
    {
        _laneManager.RegisterCar(0, _laneCar);

        int triggerCount = 0;
        _laneCar.OnCarTriggered += (_) => triggerCount++;

        _laneCar.Trigger();
        _laneCar.Trigger();

        Assert.AreEqual(1, triggerCount);
    }
}
