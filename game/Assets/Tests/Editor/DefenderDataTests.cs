using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class DefenderDataTests
{
    [Test]
    public void CreateInstance_ShouldHaveDefaultValues()
    {
        DefenderData data = ScriptableObject.CreateInstance<DefenderData>();

        Assert.AreEqual("New Defender", data.DefenderName);
        Assert.AreEqual(10, data.AttackPower);
        Assert.AreEqual(100, data.MaxHealth);
        Assert.AreEqual(1f, data.AttackSpeed);
        Assert.AreEqual(-1, data.Range);
        Assert.AreEqual(AttackType.Projectile, data.AttackType);
        Assert.AreEqual(50, data.PowerCost);
        Assert.AreEqual(12, data.MaxLevel);

        UnityEngine.Object.DestroyImmediate(data);
    }

    [Test]
    public void AttackType_Enum_ShouldHaveThreeValues()
    {
        Assert.AreEqual(0, (int)AttackType.Projectile);
        Assert.AreEqual(1, (int)AttackType.Block);
        Assert.AreEqual(2, (int)AttackType.Generate);
    }
}
