using UnityEngine;

/// <summary>
/// 防守方工厂 - 根据 DefenderData 创建对应的 GameObject 实例
/// 单例模式，管理单位实例化
/// </summary>
public class DefenderFactory : MonoBehaviour
{
    public static DefenderFactory Instance { get; private set; }

    [Header("预制体配置")]
    [SerializeField] private DefenderData[] _availableDefenders;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Defender CreateDefender(DefenderData data, int row, int col)
    {
        if (data == null || data.Prefab == null)
        {
            Debug.LogError("[DefenderFactory] DefenderData 或 Prefab 为空");
            return null;
        }

        if (GridSystem.Instance == null)
        {
            Debug.LogError("[DefenderFactory] GridSystem.Instance 为空");
            return null;
        }

        Vector3 worldPos = GridSystem.Instance.GridToWorld(row, col);
        GameObject instance = Instantiate(data.Prefab, worldPos, Quaternion.identity);

        Defender defender = instance.GetComponent<Defender>();
        if (defender == null)
        {
            Debug.LogError("[DefenderFactory] Prefab 上没有 Defender 组件");
            if (Application.isPlaying)
            {
                Destroy(instance);
            }
            else
            {
                DestroyImmediate(instance);
            }
            return null;
        }

        defender.Initialize(data, row, col);
        instance.name = $"{data.DefenderName}_{row}_{col}";

        return defender;
    }

    public DefenderData[] GetAvailableDefenders()
    {
        return _availableDefenders ?? new DefenderData[0];
    }
}
