using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 事件系统 - 单例模式，提供发布/订阅功能，实现模块间解耦通信
/// </summary>
public class EventSystem : MonoBehaviour
{
    public static EventSystem Instance { get; private set; }

    /// <summary>
    /// 事件字典 - key: 事件名, value: 回调列表
    /// </summary>
    private readonly Dictionary<string, List<Delegate>> _eventTable = new Dictionary<string, List<Delegate>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 订阅无参数事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void On(string eventName, Action callback)
    {
        if (!_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName] = new List<Delegate>();
        }
        _eventTable[eventName].Add(callback);
    }

    /// <summary>
    /// 订阅带参数事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void On<T>(string eventName, Action<T> callback)
    {
        if (!_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName] = new List<Delegate>();
        }
        _eventTable[eventName].Add(callback);
    }

    /// <summary>
    /// 取消订阅无参数事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void Off(string eventName, Action callback)
    {
        if (_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName].Remove(callback);
            if (_eventTable[eventName].Count == 0)
            {
                _eventTable.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 取消订阅带参数事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="callback">回调函数</param>
    public void Off<T>(string eventName, Action<T> callback)
    {
        if (_eventTable.ContainsKey(eventName))
        {
            _eventTable[eventName].Remove(callback);
            if (_eventTable[eventName].Count == 0)
            {
                _eventTable.Remove(eventName);
            }
        }
    }

    /// <summary>
    /// 触发无参数事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void Emit(string eventName)
    {
        if (_eventTable.ContainsKey(eventName))
        {
            foreach (var callback in _eventTable[eventName].ToArray())
            {
                try
                {
                    (callback as Action)?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventSystem] 事件 '{eventName}' 回调异常: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 触发带参数事件
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventName">事件名称</param>
    /// <param name="arg">事件参数</param>
    public void Emit<T>(string eventName, T arg)
    {
        if (_eventTable.ContainsKey(eventName))
        {
            foreach (var callback in _eventTable[eventName].ToArray())
            {
                try
                {
                    (callback as Action<T>)?.Invoke(arg);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventSystem] 事件 '{eventName}' 回调异常: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 清除指定事件的所有订阅
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void Clear(string eventName)
    {
        _eventTable.Remove(eventName);
    }

    /// <summary>
    /// 清除所有事件订阅
    /// </summary>
    public void ClearAll()
    {
        _eventTable.Clear();
    }
}
