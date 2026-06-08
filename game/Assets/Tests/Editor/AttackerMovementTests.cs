using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class AttackerMovementTests
{
    private GameObject _attackerGo;
    private AttackerMovement _movement;
    private GameObject _gridSystemGo;
    private GridSystem _gridSystem;

    [SetUp]
    public void SetUp()
    {
        _gridSystemGo = new GameObject("GridSystem");
        _gridSystem = _gridSystemGo.AddComponent<GridSystem>();
        SingletonTestHelper.InvokeAwake(_gridSystem);

        _attackerGo = new GameObject("Attacker");
        _movement = _attackerGo.AddComponent<AttackerMovement>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_attackerGo);
        if (_gridSystemGo != null)
        {
            UnityEngine.Object.DestroyImmediate(_gridSystemGo);
        }
    }

    [Test]
    public void Initialize_ShouldSetLaneAndSpeed()
    {
        _movement.Initialize(2, 1.5f);
        Assert.AreEqual(2, _movement.CurrentLane);
        Assert.IsTrue(_movement.IsMoving);
    }

    [Test]
    public void Initialize_ShouldCalculateTargetX()
    {
        _movement.Initialize(0, 1.0f);
        Assert.IsTrue(_movement.IsMoving);
    }

    [Test]
    public void Stop_ShouldStopMovement()
    {
        _movement.Initialize(0, 1.0f);
        Assert.IsTrue(_movement.IsMoving);
        _movement.Stop();
        Assert.IsFalse(_movement.IsMoving);
    }

    [Test]
    public void Update_WhenNotMoving_ShouldNotChangePosition()
    {
        _movement.Initialize(0, 1.0f);
        _movement.Stop();
        Vector3 initialPos = _attackerGo.transform.position;
        
        _movement.Move(0.02f);
        
        Assert.AreEqual(initialPos, _attackerGo.transform.position);
    }

    [Test]
    public void Movement_ShouldMoveLeft()
    {
        _movement.Initialize(0, 1.0f);
        Vector3 initialPos = _attackerGo.transform.position;
        
        _movement.Move(0.02f);
        
        Assert.Less(_attackerGo.transform.position.x, initialPos.x);
    }

    [Test]
    public void CurrentLane_ShouldReturnInitializedLane()
    {
        _movement.Initialize(3, 1.0f);
        Assert.AreEqual(3, _movement.CurrentLane);
    }

    [Test]
    public void IsMoving_ShouldBeTrueAfterInitialize()
    {
        _movement.Initialize(0, 1.0f);
        Assert.IsTrue(_movement.IsMoving);
    }

    [Test]
    public void IsMoving_ShouldBeFalseAfterStop()
    {
        _movement.Initialize(0, 1.0f);
        _movement.Stop();
        Assert.IsFalse(_movement.IsMoving);
    }
}
