using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对象池 - 管理弹丸、怪物、电力球等频繁创建/销毁的对象
/// </summary>
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public string poolName;           // 池名称
        public GameObject prefab;         // 预制体
        public int initialSize = 10;      // 初始大小
        public bool autoExpand = true;    // 自动扩展
    }

    [Header("对象池配置")]
    [SerializeField] private List<PoolConfig> _poolConfigs = new List<PoolConfig>();

    /// <summary>
    /// 池字典 - key: 池名称, value: 对象池
    /// </summary>
    private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// 池配置字典 - key: 池名称, value: 配置
    /// </summary>
    private readonly Dictionary<string, PoolConfig> _configMap = new Dictionary<string, PoolConfig>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    /// <summary>
    /// 初始化所有配置的对象池
    /// </summary>
    private void InitializePools()
    {
        foreach (var config in _poolConfigs)
        {
            CreatePool(config);
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <param name="config">池配置</param>
    public void CreatePool(PoolConfig config)
    {
        if (_pools.ContainsKey(config.poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池 '{config.poolName}' 已存在");
            return;
        }

        _pools[config.poolName] = new Queue<GameObject>();
        _configMap[config.poolName] = config;

        // 预创建对象
        for (int i = 0; i < config.initialSize; i++)
        {
            GameObject obj = CreateObject(config);
            _pools[config.poolName].Enqueue(obj);
        }

        Debug.Log($"[ObjectPool] 创建对象池 '{config.poolName}', 初始大小: {config.initialSize}");
    }

    /// <summary>
    /// 创建池对象
    /// </summary>
    private GameObject CreateObject(PoolConfig config)
    {
        GameObject obj = Instantiate(config.prefab, transform);
        obj.SetActive(false);
        obj.name = config.poolName;
        return obj;
    }

    /// <summary>
    /// 从池中获取对象
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    /// <returns>激活的游戏对象</returns>
    public GameObject Get(string poolName, Vector3 position, Quaternion rotation)
    {
        if (!_pools.ContainsKey(poolName))
        {
            Debug.LogError($"[ObjectPool] 对象池 '{poolName}' 不存在");
            return null;
        }

        GameObject obj;

        if (_pools[poolName].Count > 0)
        {
            obj = _pools[poolName].Dequeue();
        }
        else if (_configMap[poolName].autoExpand)
        {
            // 自动扩展
            obj = CreateObject(_configMap[poolName]);
            Debug.Log($"[ObjectPool] 对象池 '{poolName}' 自动扩展");
        }
        else
        {
            Debug.LogWarning($"[ObjectPool] 对象池 '{poolName}' 已空且未启用自动扩展");
            return null;
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 归还对象到池中
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <param name="obj">要归还的游戏对象</param>
    public void Return(string poolName, GameObject obj)
    {
        if (!_pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"[ObjectPool] 对象池 '{poolName}' 不存在，直接销毁对象");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _pools[poolName].Enqueue(obj);
    }

    /// <summary>
    /// 清空指定对象池
    /// </summary>
    /// <param name="poolName">池名称</param>
    public void ClearPool(string poolName)
    {
        if (_pools.ContainsKey(poolName))
        {
            foreach (var obj in _pools[poolName])
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            _pools[poolName].Clear();
        }
    }

    /// <summary>
    /// 清空所有对象池
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var poolName in _pools.Keys)
        {
            ClearPool(poolName);
        }
        _pools.Clear();
        _configMap.Clear();
    }

    /// <summary>
    /// 获取池中剩余对象数量
    /// </summary>
    /// <param name="poolName">池名称</param>
    /// <returns>剩余对象数量</returns>
    public int GetCount(string poolName)
    {
        if (_pools.ContainsKey(poolName))
        {
            return _pools[poolName].Count;
        }
        return 0;
    }
}
