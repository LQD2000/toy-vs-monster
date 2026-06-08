using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class UnitPlacementTests
{
    private GameObject _placementGo;
    private UnitPlacement _placement;

    [SetUp]
    public void SetUp()
    {
        _placementGo = new GameObject();
        _placement = _placementGo.AddComponent<UnitPlacement>();
        SingletonTestHelper.InvokeAwake(_placement);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_placementGo);
        SingletonTestHelper.ResetSingleton<UnitPlacement>();
    }

    [Test]
    public void SelectDefender_ShouldSetSelectedDefender()
    {
        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();
        _placement.SelectDefender(data);
        Assert.AreSame(data, _placement.SelectedDefender);
        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void ClearSelection_ShouldSetToNull()
    {
        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();
        _placement.SelectDefender(data);
        _placement.ClearSelection();
        Assert.IsNull(_placement.SelectedDefender);
        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void SelectDefender_ShouldFireEvent()
    {
        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();
        DefenderData received = null;
        _placement.OnDefenderSelected += (d) => received = d;
        _placement.SelectDefender(data);
        Assert.AreSame(data, received);
        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void Instance_AfterAwake_ShouldNotBeNull()
    {
        Assert.AreSame(_placement, UnitPlacement.Instance);
    }
}
