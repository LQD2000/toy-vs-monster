using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

[TestFixture]
public class EventSystemTests
{
    private GameObject _gameObject;
    private EventSystem _eventSystem;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _eventSystem = _gameObject.AddComponent<EventSystem>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_gameObject);
        SingletonTestHelper.ResetSingleton<EventSystem>();
    }

    [Test]
    public void SubscribeAndEmit_ShouldInvokeCallback()
    {
        bool wasCalled = false;
        _eventSystem.On("TestEvent", () => wasCalled = true);

        _eventSystem.Emit("TestEvent");

        Assert.IsTrue(wasCalled);
    }

    [Test]
    public void SubscribeAndEmit_WithParameter_ShouldPassCorrectValue()
    {
        int received = 0;
        _eventSystem.On<int>("TestEvent", (value) => received = value);

        _eventSystem.Emit("TestEvent", 42);

        Assert.AreEqual(42, received);
    }

    [Test]
    public void Emit_NoSubscribers_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() => _eventSystem.Emit("NonExistentEvent"));
    }

    [Test]
    public void Emit_WithParameter_NoSubscribers_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() => _eventSystem.Emit("NonExistentEvent", 123));
    }

    [Test]
    public void MultipleSubscribers_ShouldAllBeCalled()
    {
        int callCount = 0;
        _eventSystem.On("MultiEvent", () => callCount++);
        _eventSystem.On("MultiEvent", () => callCount++);
        _eventSystem.On("MultiEvent", () => callCount++);

        _eventSystem.Emit("MultiEvent");

        Assert.AreEqual(3, callCount);
    }

    [Test]
    public void Unsubscribe_ShouldStopCallback()
    {
        int callCount = 0;
        Action callback = () => callCount++;
        _eventSystem.On("TestEvent", callback);
        _eventSystem.Off("TestEvent", callback);

        _eventSystem.Emit("TestEvent");

        Assert.AreEqual(0, callCount);
    }

    [Test]
    public void Unsubscribe_ShouldNotAffectOtherSubscribers()
    {
        int firstCallCount = 0;
        int secondCallCount = 0;
        Action callback1 = () => firstCallCount++;
        Action callback2 = () => secondCallCount++;
        _eventSystem.On("TestEvent", callback1);
        _eventSystem.On("TestEvent", callback2);
        _eventSystem.Off("TestEvent", callback1);

        _eventSystem.Emit("TestEvent");

        Assert.AreEqual(0, firstCallCount);
        Assert.AreEqual(1, secondCallCount);
    }

    [Test]
    public void Unsubscribe_NonExistentCallback_ShouldNotThrow()
    {
        Action callback = () => { };
        Assert.DoesNotThrow(() => _eventSystem.Off("NonExistentEvent", callback));
    }

    [Test]
    public void Unsubscribe_Generic_ShouldStopTypedCallback()
    {
        int received = 0;
        Action<int> callback = (v) => received = v;
        _eventSystem.On<int>("TypedEvent", callback);
        _eventSystem.Off<int>("TypedEvent", callback);

        _eventSystem.Emit("TypedEvent", 99);

        Assert.AreEqual(0, received);
    }

    [Test]
    public void ExceptionInCallback_ShouldNotBlockOtherCallbacks()
    {
        int callCount = 0;
        _eventSystem.On("ErrorEvent", () => throw new System.Exception("Test failure"));
        _eventSystem.On("ErrorEvent", () => callCount++);

        LogAssert.Expect(LogType.Error, "[EventSystem] 事件 'ErrorEvent' 回调异常: Test failure");
        Assert.DoesNotThrow(() => _eventSystem.Emit("ErrorEvent"));
        Assert.AreEqual(1, callCount,
            "Other callbacks should still execute after one callback throws");
    }

    [Test]
    public void Clear_ShouldRemoveEvent()
    {
        bool wasCalled = false;
        _eventSystem.On("ToClear", () => wasCalled = true);
        _eventSystem.Clear("ToClear");

        _eventSystem.Emit("ToClear");

        Assert.IsFalse(wasCalled);
    }

    [Test]
    public void ClearAll_ShouldRemoveAllEvents()
    {
        bool event1Called = false;
        bool event2Called = false;
        _eventSystem.On("Event1", () => event1Called = true);
        _eventSystem.On("Event2", () => event2Called = true);
        _eventSystem.ClearAll();

        _eventSystem.Emit("Event1");
        _eventSystem.Emit("Event2");

        Assert.IsFalse(event1Called);
        Assert.IsFalse(event2Called);
    }

    [Test]
    public void DuplicateSubscription_ShouldCallTwice()
    {
        int callCount = 0;
        Action callback = () => callCount++;
        _eventSystem.On("DupEvent", callback);
        _eventSystem.On("DupEvent", callback);

        _eventSystem.Emit("DupEvent");

        Assert.AreEqual(2, callCount, "Duplicate subscription should result in callback being called twice");
    }
}
