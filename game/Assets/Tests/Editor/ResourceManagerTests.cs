using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ResourceManagerTests
{
    private GameObject _gameObject;
    private ResourceManager _resourceManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _resourceManager = _gameObject.AddComponent<ResourceManager>();
        SingletonTestHelper.InvokeAwake(_resourceManager);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_gameObject);
        SingletonTestHelper.ResetSingleton<ResourceManager>();
    }

    [Test]
    public void InitialPower_ShouldEqualMaxPower()
    {
        Assert.AreEqual(_resourceManager.MaxPower, _resourceManager.CurrentPower);
    }

    [Test]
    public void HasEnoughPower_WhenEnough_ShouldReturnTrue()
    {
        Assert.IsTrue(_resourceManager.HasEnoughPower(50));
    }

    [Test]
    public void HasEnoughPower_WhenNotEnough_ShouldReturnFalse()
    {
        Assert.IsFalse(_resourceManager.HasEnoughPower(_resourceManager.MaxPower + 1));
    }

    [Test]
    public void ConsumePower_WhenEnough_ShouldDeduct()
    {
        int before = _resourceManager.CurrentPower;
        Assert.IsTrue(_resourceManager.ConsumePower(30));
        Assert.AreEqual(before - 30, _resourceManager.CurrentPower);
    }

    [Test]
    public void ConsumePower_WhenNotEnough_ShouldFail()
    {
        int before = _resourceManager.CurrentPower;
        Assert.IsFalse(_resourceManager.ConsumePower(_resourceManager.MaxPower + 1));
        Assert.AreEqual(before, _resourceManager.CurrentPower);
    }

    [Test]
    public void AddPower_ShouldNotExceedMax()
    {
        _resourceManager.ConsumePower(100);
        int before = _resourceManager.CurrentPower;
        _resourceManager.AddPower(30);
        Assert.AreEqual(before + 30, _resourceManager.CurrentPower);
        _resourceManager.AddPower(9999);
        Assert.AreEqual(_resourceManager.MaxPower, _resourceManager.CurrentPower);
    }

    [Test]
    public void ResetPower_ShouldRestoreToMax()
    {
        _resourceManager.ConsumePower(100);
        _resourceManager.ResetPower();
        Assert.AreEqual(_resourceManager.MaxPower, _resourceManager.CurrentPower);
    }

    [Test]
    public void ConsumePower_ShouldFireOnPowerChanged()
    {
        int? received = null;
        _resourceManager.OnPowerChanged += (cur, _) => received = cur;
        _resourceManager.ConsumePower(10);
        Assert.IsNotNull(received);
        Assert.AreEqual(_resourceManager.CurrentPower, received.Value);
    }
}
