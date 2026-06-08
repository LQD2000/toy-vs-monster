using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 进攻方生成器 - 按配置的时间轴生成怪物
/// 支持多条跑道、多种怪物类型、时间轴控制
/// </summary>
public class AttackerSpawner : MonoBehaviour
{
    public static AttackerSpawner Instance { get; private set; }

    [System.Serializable]
    public class SpawnEntry
    {
        public AttackerData attackerData;
        public int laneIndex;
        public float spawnTime;
    }

    [Header("生成配置")]
    [SerializeField] private List<SpawnEntry> _spawnTimeline = new List<SpawnEntry>();

    [Header("生成位置")]
    [SerializeField] private float _spawnOffsetX = 1.5f;

    private float _gameTimer;
    private int _nextSpawnIndex;
    private bool _isSpawning;

    public event System.Action<Attacker> OnAttackerSpawned;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        if (newState == GameState.InProgress)
        {
            StartSpawning();
        }
        else
        {
            StopSpawning();
        }
    }

    public void StartSpawning()
    {
        _gameTimer = 0f;
        _nextSpawnIndex = 0;
        _isSpawning = true;
        Debug.Log("[AttackerSpawner] 开始生成怪物");
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        Debug.Log("[AttackerSpawner] 停止生成怪物");
    }

    private void Update()
    {
        if (!_isSpawning) return;
        if (_nextSpawnIndex >= _spawnTimeline.Count) return;

        _gameTimer += Time.deltaTime;

        while (_nextSpawnIndex < _spawnTimeline.Count)
        {
            SpawnEntry entry = _spawnTimeline[_nextSpawnIndex];
            if (_gameTimer >= entry.spawnTime)
            {
                SpawnAttacker(entry);
                _nextSpawnIndex++;
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnAttacker(SpawnEntry entry)
    {
        if (entry.attackerData == null || entry.attackerData.Prefab == null)
        {
            Debug.LogWarning($"[AttackerSpawner] 生成配置无效: {entry.attackerData?.AttackerName}");
            return;
        }

        Vector3 spawnPos = GetSpawnPosition(entry.laneIndex);

        GameObject attackerObj;
        if (ObjectPool.Instance != null)
        {
            attackerObj = ObjectPool.Instance.Get("Attacker", spawnPos, Quaternion.identity);
        }
        else
        {
            attackerObj = Instantiate(entry.attackerData.Prefab, spawnPos, Quaternion.identity);
        }

        if (attackerObj == null)
        {
            Debug.LogError("[AttackerSpawner] 生成怪物失败");
            return;
        }

        Attacker attacker = attackerObj.GetComponent<Attacker>();
        if (attacker == null)
        {
            attacker = attackerObj.AddComponent<Attacker>();
        }
        attacker.Initialize(entry.attackerData, entry.laneIndex);

        AttackerMovement movement = attackerObj.GetComponent<AttackerMovement>();
        if (movement == null)
        {
            movement = attackerObj.AddComponent<AttackerMovement>();
        }
        movement.Initialize(entry.laneIndex, entry.attackerData.MoveSpeed);

        Debug.Log($"[AttackerSpawner] 生成怪物: {entry.attackerData.AttackerName} (Lane: {entry.laneIndex}, Time: {entry.spawnTime:F1}s)");
        OnAttackerSpawned?.Invoke(attacker);
    }

    private Vector3 GetSpawnPosition(int laneIndex)
    {
        if (GridSystem.Instance != null)
        {
            Vector3 lanePos = GridSystem.Instance.GridToWorld(laneIndex, GridSystem.Instance.Cols - 1);
            return new Vector3(lanePos.x + _spawnOffsetX, lanePos.y, 0f);
        }
        return new Vector3(8f, 0f, 0f);
    }

    public void SetTimeline(List<SpawnEntry> timeline)
    {
        _spawnTimeline = timeline ?? new List<SpawnEntry>();
        _spawnTimeline.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    public void AddSpawnEntry(AttackerData data, int lane, float time)
    {
        _spawnTimeline.Add(new SpawnEntry
        {
            attackerData = data,
            laneIndex = lane,
            spawnTime = time
        });
        _spawnTimeline.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }
}
