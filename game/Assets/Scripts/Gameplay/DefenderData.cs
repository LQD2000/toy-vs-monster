using UnityEngine;

/// <summary>
/// 攻击类型枚举
/// </summary>
public enum AttackType
{
    Projectile,  // 弹射类（弹珠射手）
    Block,       // 阻挡类（积木墙）
    Generate     // 产电类（小太阳宝宝）
}

/// <summary>
/// 防守方单位数据 - ScriptableObject 定义单位属性
/// 用于配置不同类型的防守方单位
/// </summary>
[CreateAssetMenu(fileName = "NewDefenderData", menuName = "Game/Defender Data")]
public class DefenderData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("单位名称")]
    [SerializeField] private string _defenderName = "New Defender";

    [Tooltip("单位描述")]
    [SerializeField] private string _description = "";

    [Tooltip("单位图标")]
    [SerializeField] private Sprite _icon;

    [Tooltip("单位预制体")]
    [SerializeField] private GameObject _prefab;

    [Header("战斗属性")]
    [Tooltip("攻击力")]
    [SerializeField] private int _attackPower = 10;

    [Tooltip("初始血量")]
    [SerializeField] private int _maxHealth = 100;

    [Tooltip("攻击速度（次/秒）")]
    [SerializeField] private float _attackSpeed = 1f;

    [Tooltip("射程（格子数，-1表示整条跑道）")]
    [SerializeField] private int _range = -1;

    [Tooltip("攻击类型")]
    [SerializeField] private AttackType _attackType = AttackType.Projectile;

    [Header("放置消耗")]
    [Tooltip("电力消耗")]
    [SerializeField] private int _powerCost = 50;

    [Header("升级配置")]
    [Tooltip("最大等级")]
    [SerializeField] private int _maxLevel = 12;

    /// <summary>单位名称</summary>
    public string DefenderName => _defenderName;

    /// <summary>单位描述</summary>
    public string Description => _description;

    /// <summary>单位图标</summary>
    public Sprite Icon => _icon;

    /// <summary>单位预制体</summary>
    public GameObject Prefab => _prefab;

    /// <summary>攻击力</summary>
    public int AttackPower => _attackPower;

    /// <summary>最大血量</summary>
    public int MaxHealth => _maxHealth;

    /// <summary>攻击速度（次/秒）</summary>
    public float AttackSpeed => _attackSpeed;

    /// <summary>射程（格子数，-1表示整条跑道）</summary>
    public int Range => _range;

    /// <summary>攻击类型</summary>
    public AttackType AttackType => _attackType;

    /// <summary>电力消耗</summary>
    public int PowerCost => _powerCost;

    /// <summary>最大等级</summary>
    public int MaxLevel => _maxLevel;
}
