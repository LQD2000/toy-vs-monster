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

    /// <summary>
    /// 手动调用组件的 Awake 方法（Edit Mode 测试中 Awake 不会自动调用）
    /// </summary>
    public static void InvokeAwake(MonoBehaviour component)
    {
        if (component == null) return;
        var method = component.GetType().GetMethod("Awake", 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        method?.Invoke(component, null);
    }
}
