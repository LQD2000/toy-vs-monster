using UnityEngine;

/// <summary>
/// 伤害计算器 - 统一处理所有伤害计算逻辑
/// </summary>
public static class DamageCalculator
{
    public struct DamageResult
    {
        public int finalDamage;
        public bool isCritical;
        public float critMultiplier;

        public DamageResult(int damage, bool crit, float multiplier)
        {
            finalDamage = damage;
            isCritical = crit;
            critMultiplier = multiplier;
        }
    }

    private const float BASE_CRIT_CHANCE = 0.1f;
    private const float CRIT_DAMAGE_MULTIPLIER = 2.0f;
    private const int MIN_DAMAGE = 1;

    /// <summary>
    /// 计算最终伤害
    /// 公式: 基础攻击力 × 类型克制系数 × 暴击倍率 - 敌人防御力
    /// </summary>
    public static DamageResult CalculateDamage(
        int baseAttack,
        int defense = 0,
        float typeMultiplier = 1.0f,
        float critChance = -1f)
    {
        if (critChance < 0f)
        {
            critChance = BASE_CRIT_CHANCE;
        }

        bool isCritical = Random.value < critChance;
        float critMultiplier = isCritical ? CRIT_DAMAGE_MULTIPLIER : 1.0f;

        float rawDamage = baseAttack * typeMultiplier * critMultiplier;
        int finalDamage = Mathf.Max(MIN_DAMAGE, Mathf.RoundToInt(rawDamage - defense));

        return new DamageResult(finalDamage, isCritical, critMultiplier);
    }

    /// <summary>
    /// 计算爆炸伤害（范围伤害，伤害递减）
    /// </summary>
    public static int CalculateExplosionDamage(
        int baseAttack,
        float distanceFromCenter,
        float explosionRadius,
        float falloffFactor = 0.5f)
    {
        if (explosionRadius <= 0f) return baseAttack;

        float distanceRatio = Mathf.Clamp01(distanceFromCenter / explosionRadius);
        float damageMultiplier = 1f - (distanceRatio * falloffFactor);
        int damage = Mathf.Max(MIN_DAMAGE, Mathf.RoundToInt(baseAttack * damageMultiplier));

        return damage;
    }

    /// <summary>
    /// 获取类型克制系数
    /// </summary>
    public static float GetTypeMultiplier(AttackType attackType, string defenderType)
    {
        // TODO: 根据游戏设计扩展类型克制表
        return 1.0f;
    }
}
