using NUnit.Framework;
using UnityEngine;
using System.Reflection;

/// <summary>
/// 单例清理工具 — 通过反射重置 Monobehaviour 单例的 Instance 属性
/// </summary>
public static class SingletonTestHelper
{
    public static void ResetSingleton<T>() where T : MonoBehaviour
    {
        var prop = typeof(T).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        var setter = prop?.GetSetMethod(nonPublic: true);
        setter?.Invoke(null, new object[] { null });
    }
}
