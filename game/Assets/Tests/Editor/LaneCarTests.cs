using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道管理器初始化, 跑道数量: 1");
        SingletonTestHelper.InvokeAwake(_laneManager);

        _carGameObject = new GameObject();
        _carGameObject.AddComponent<SpriteRenderer>();
        _laneCar = _carGameObject.AddComponent<LaneCar>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_carGameObject);
        UnityEngine.Object.DestroyImmediate(_managerGameObject);
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
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, _laneCar);

        LogAssert.Expect(LogType.Log, "[LaneCar] 小车 0 触发！清除跑道全部怪物");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        _laneCar.Trigger();

        Assert.IsTrue(_laneCar.IsTriggered);
    }

    [Test]
    public void Trigger_ShouldFireOnCarTriggeredEvent()
    {
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, _laneCar);

        int? triggeredLane = null;
        _laneCar.OnCarTriggered += (lane) => triggeredLane = lane;

        LogAssert.Expect(LogType.Log, "[LaneCar] 小车 0 触发！清除跑道全部怪物");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        _laneCar.Trigger();

        Assert.AreEqual(0, triggeredLane);
    }

    [Test]
    public void Trigger_CalledTwice_ShouldNotThrow()
    {
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, _laneCar);

        LogAssert.Expect(LogType.Log, "[LaneCar] 小车 0 触发！清除跑道全部怪物");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        _laneCar.Trigger();

        LogAssert.Expect(LogType.Warning, "[LaneCar] 小车 0 已被触发，忽略重复触发");
        Assert.DoesNotThrow(() => _laneCar.Trigger());
    }

    [Test]
    public void Trigger_CalledTwice_ShouldOnlyTriggerOnce()
    {
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 小车已注册");
        _laneManager.RegisterCar(0, _laneCar);

        int triggerCount = 0;
        _laneCar.OnCarTriggered += (_) => triggerCount++;

        LogAssert.Expect(LogType.Log, "[LaneCar] 小车 0 触发！清除跑道全部怪物");
        LogAssert.Expect(LogType.Log, "[LaneManager] 跑道 0 已清除");
        _laneCar.Trigger();
        LogAssert.Expect(LogType.Warning, "[LaneCar] 小车 0 已被触发，忽略重复触发");
        _laneCar.Trigger();

        Assert.AreEqual(1, triggerCount);
    }
}
