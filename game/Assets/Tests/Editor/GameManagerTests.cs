using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        UnityEngine.Object.DestroyImmediate(_gameObject);
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
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();

        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);
    }

    [Test]
    public void StartGame_NotFromPreparation_ShouldNotChangeState()
    {
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.SetState(GameState.InProgress);

        LogAssert.Expect(LogType.Warning, "[GameManager] 只能在准备阶段开始游戏");
        _gameManager.StartGame();

        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);
    }

    [Test]
    public void WinGame_FromInProgress_ShouldTransitionToVictory()
    {
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: InProgress -> Victory");
        _gameManager.WinGame();

        Assert.AreEqual(GameState.Victory, _gameManager.CurrentState);
    }

    [Test]
    public void WinGame_NotFromInProgress_ShouldNotChangeState()
    {
        LogAssert.Expect(LogType.Warning, "[GameManager] 只有在游戏进行中才能判定胜利");
        _gameManager.WinGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void LoseGame_FromInProgress_ShouldTransitionToDefeat()
    {
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: InProgress -> Defeat");
        _gameManager.LoseGame();

        Assert.AreEqual(GameState.Defeat, _gameManager.CurrentState);
    }

    [Test]
    public void LoseGame_NotFromInProgress_ShouldNotChangeState()
    {
        LogAssert.Expect(LogType.Warning, "[GameManager] 只有在游戏进行中才能判定失败");
        _gameManager.LoseGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void ResetGame_ShouldTransitionToPreparation()
    {
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: InProgress -> Preparation");
        _gameManager.ResetGame();

        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);
    }

    [Test]
    public void ResetGame_FromVictory_ShouldTransitionToPreparation()
    {
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: InProgress -> Victory");
        _gameManager.WinGame();

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Victory -> Preparation");
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

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
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

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();
        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: InProgress -> Victory");
        _gameManager.WinGame();
        Assert.AreEqual(GameState.Victory, _gameManager.CurrentState);
    }

    [Test]
    public void FullLifecycle_PreparationToDefeat()
    {
        Assert.AreEqual(GameState.Preparation, _gameManager.CurrentState);

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> InProgress");
        _gameManager.StartGame();
        Assert.AreEqual(GameState.InProgress, _gameManager.CurrentState);

        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: InProgress -> Defeat");
        _gameManager.LoseGame();
        Assert.AreEqual(GameState.Defeat, _gameManager.CurrentState);
    }

    [Test]
    public void SetState_ManualTransition_ShouldWork()
    {
        LogAssert.Expect(LogType.Log, "[GameManager] 状态变更: Preparation -> Victory");
        _gameManager.SetState(GameState.Victory);

        Assert.AreEqual(GameState.Victory, _gameManager.CurrentState);
    }
}
