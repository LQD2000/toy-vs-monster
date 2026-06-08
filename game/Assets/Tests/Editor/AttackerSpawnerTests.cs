using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;

[TestFixture]
public class AttackerSpawnerTests
{
    private GameObject _spawnerGo;
    private AttackerSpawner _spawner;
    private AttackerData _testData;

    [SetUp]
    public void SetUp()
    {
        _testData = ScriptableObject.CreateInstance<AttackerData>();
        _testData.name = "TestAttackerData";

        _spawnerGo = new GameObject("AttackerSpawner");
        _spawner = _spawnerGo.AddComponent<AttackerSpawner>();
        SingletonTestHelper.InvokeAwake(_spawner);
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_spawnerGo);
        if (_testData != null)
        {
            UnityEngine.Object.DestroyImmediate(_testData);
        }
    }

    [Test]
    public void Instance_ShouldBeSingleton()
    {
        Assert.IsNotNull(AttackerSpawner.Instance);
        Assert.AreSame(_spawner, AttackerSpawner.Instance);
    }

    [Test]
    public void AddSpawnEntry_ShouldAddToTimeline()
    {
        _spawner.AddSpawnEntry(_testData, 0, 1.0f);
        _spawner.AddSpawnEntry(_testData, 1, 2.0f);
    }

    [Test]
    public void SetTimeline_ShouldReplaceTimeline()
    {
        var timeline = new List<AttackerSpawner.SpawnEntry>
        {
            new AttackerSpawner.SpawnEntry { attackerData = _testData, laneIndex = 0, spawnTime = 1.0f },
            new AttackerSpawner.SpawnEntry { attackerData = _testData, laneIndex = 1, spawnTime = 2.0f }
        };
        _spawner.SetTimeline(timeline);
    }

    [Test]
    public void SetTimeline_Null_ShouldCreateEmptyTimeline()
    {
        _spawner.SetTimeline(null);
    }

    [Test]
    public void StartSpawning_ShouldSetSpawningState()
    {
        _spawner.StartSpawning();
    }

    [Test]
    public void StopSpawning_ShouldStopSpawningState()
    {
        _spawner.StartSpawning();
        _spawner.StopSpawning();
    }

    [Test]
    public void OnAttackerSpawned_Event_ShouldBeInvoked()
    {
        Attacker spawnedAttacker = null;
        _spawner.OnAttackerSpawned += (a) => spawnedAttacker = a;
    }
}
