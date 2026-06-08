using UnityEngine;

/// <summary>
/// 进攻方单位数据 - ScriptableObject 定义怪物属性
/// 用于配置不同类型的进攻方单位
/// </summary>
[CreateAssetMenu(fileName = "NewAttackerData", menuName = "Game/Attacker Data")]
public class AttackerData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("怪物名称")]
    [SerializeField] private string _attackerName = "New Attacker";

    [Tooltip("怪物描述")]
    [SerializeField] private string _description = "";

    [Tooltip("怪物图标")]
    [SerializeField] private Sprite _icon;

    [Tooltip("怪物预制体")]
    [SerializeField] private GameObject _prefab;

    [Header("战斗属性")]
    [Tooltip("血量")]
    [SerializeField] private int _maxHealth = 100;

    [Tooltip("攻击力")]
    [SerializeField] private int _attackPower = 10;

    [Tooltip("攻击速度（次/秒）")]
    [SerializeField] private float _attackSpeed = 1f;

    [Tooltip("移动速度（格/秒）")]
    [SerializeField] private float _moveSpeed = 0.5f;

    [Header("特殊能力")]
    [Tooltip("是否有特殊能力")]
    [SerializeField] private bool _hasSpecialAbility = false;

    [Tooltip("特殊能力描述")]
    [SerializeField] private string _specialAbilityDescription = "";

    /// <summary>怪物名称</summary>
    public string AttackerName => _attackerName;

    /// <summary>怪物描述</summary>
    public string Description => _description;

    /// <summary>怪物图标</summary>
    public Sprite Icon => _icon;

    /// <summary>怪物预制体</summary>
    public GameObject Prefab => _prefab;

    /// <summary>最大血量</summary>
    public int MaxHealth => _maxHealth;

    /// <summary>攻击力</summary>
    public int AttackPower => _attackPower;

    /// <summary>攻击速度（次/秒）</summary>
    public float AttackSpeed => _attackSpeed;

    /// <summary>移动速度（格/秒）</summary>
    public float MoveSpeed => _moveSpeed;

    /// <summary>是否有特殊能力</summary>
    public bool HasSpecialAbility => _hasSpecialAbility;

    /// <summary>特殊能力描述</summary>
    public string SpecialAbilityDescription => _specialAbilityDescription;
}
