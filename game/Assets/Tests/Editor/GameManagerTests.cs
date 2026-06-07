using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GameManagerTests
{
    private GameObject _gameObject;
    private GameManager _gameManager;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _gameManager = _gameObject.AddComponent<GameManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
        SingletonTestHelper.ResetSingleton<GameManager>();
    }

    [Test]
    public void InitialState_ShouldBePreparation()
    {
        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void StartGame_FromPreparation_ShouldTransitionToInProgress()
    {
        _gameManager.StartGame();

        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);
    }

    [Test]
    public void StartGame_NotFromPreparation_ShouldNotChangeState()
    {
        _gameManager.SetState(GameState.InProgress);

        _gameManager.StartGame();

        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);
    }

    [Test]
    public void WinGame_FromInProgress_ShouldTransitionToVictory()
    {
        _gameManager.StartGame();

        _gameManager.WinGame();

        Assert.AreEqual(GameState.Victory, _gameManager.CurrentState);
    }

    [Test]
    public void WinGame_NotFromInProgress_ShouldNotChangeState()
    {
        _gameManager.WinGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void LoseGame_FromInProgress_ShouldTransitionToDefeat()
    {
        _gameManager.StartGame();

        _gameManager.LoseGame();

        Assert.AreEqual(GameState.Defeat, _gameManager.CurrentState);
    }

    [Test]
    public void LoseGame_NotFromInProgress_ShouldNotChangeState()
    {
        _gameManager.LoseGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void ResetGame_ShouldTransitionToPreparation()
    {
        _gameManager.StartGame();

        _gameManager.ResetGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void ResetGame_FromVictory_ShouldTransitionToPreparation()
    {
        _gameManager.StartGame();
        _gameManager.WinGame();

        _gameManager.ResetGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void StateChange_ShouldFireEvent_WithCorrectOldAndNewStates()
    {
        GameState? oldStateReceived = null;
        GameState? newStateReceived = null;
        _gameManager.OnGameStateChanged += (oldState, newState) =>
        {
            oldStateReceived = oldState;
            newStateReceived = newState;
        };

        _gameManager.StartGame();

        Assert.AreEqual(GameState.Preparation, oldStateReceived);
        Assert.AreEqual(GameState.InProgress, newStateReceived);
    }

    [Test]
    public void SetSameState_ShouldNotFireEvent()
    {
        int eventCallCount = 0;
        _gameManager.OnGameStateChanged += (_, _) => eventCallCount++;

        _gameManager.SetState(GameState.Preparation);

        Assert.AreEqual(0, eventCallCount,
            "Setting the same state should not fire OnGameStateChanged");
    }

    [Test]
    public void SetSameState_ShouldNotChangeState()
    {
        _gameManager.SetState(GameState.Preparation);

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void FullLifecycle_PreparationToVictory()
    {
        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);

        _gameManager.StartGame();
        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);

        _gameManager.WinGame();
        Assert.AreEqual(GameState.Victory, _gameManager.CurrentState);
    }

    [Test]
    public void FullLifecycle_PreparationToDefeat()
    {
        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);

        _gameManager.StartGame();
        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);

        _gameManager.LoseGame();
        Assert.AreEqual(GameState.Defeat, _gameManager.CurrentState);
    }

    [Test]
    public void SetState_ManualTransition_ShouldWork()
    {
        _gameManager.SetState(GameState.Victory);

        Assert.AreEqual(GameState.Victory, _gameManager.CurrentState);
    }
}
