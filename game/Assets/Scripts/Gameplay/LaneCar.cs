using UnityEngine;
using System;

/// <summary>
/// 跑道小车 - 一次性防御机制
/// 位于跑道最左侧（格子1左侧），怪物突破时触发，清除整行怪物后消失
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class LaneCar : MonoBehaviour
{
    [Header("小车配置")]
    [SerializeField] private int _laneIndex = 0;

    /// <summary>所属跑道索引</summary>
    public int LaneIndex => _laneIndex;

    /// <summary>小车是否已被触发</summary>
    public bool IsTriggered { get; private set; } = false;

    /// <summary>小车触发事件 - 参数：跑道索引</summary>
    public event Action<int> OnCarTriggered;

    [Header("视觉效果")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _activeColor = Color.green;
    [SerializeField] private Color _triggeredColor = Color.red;
    [SerializeField] private float _triggerAnimationDuration = 0.5f;

    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        // 在 LaneManager 中注册
        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.RegisterCar(_laneIndex, this);
        }
        else
        {
            Debug.LogError($"[LaneCar] LaneManager.Instance 为空，小车 {_laneIndex} 未能注册");
        }

        UpdateVisual();
    }

    /// <summary>
    /// 触发小车 - 清除整行怪物并销毁小车
    /// </summary>
    public void Trigger()
    {
        if (IsTriggered)
        {
            Debug.LogWarning($"[LaneCar] 小车 {_laneIndex} 已被触发，忽略重复触发");
            return;
        }

        IsTriggered = true;
        Debug.Log($"[LaneCar] 小车 {_laneIndex} 触发！清除跑道全部怪物");

        // 通知 LaneManager 清除该跑道
        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.ClearLane(_laneIndex);
        }

        // 触发事件
        OnCarTriggered?.Invoke(_laneIndex);

        // 触发动画
        StartCoroutine(TriggerAnimation());
    }

    /// <summary>
    /// 触发动画效果
    /// </summary>
    private System.Collections.IEnumerator TriggerAnimation()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Color startColor = _spriteRenderer != null ? _spriteRenderer.color : _activeColor;

        while (elapsed < _triggerAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _triggerAnimationDuration;

            // 缩放抖动效果
            transform.localScale = startScale * (1f + Mathf.Sin(t * Mathf.PI * 4f) * 0.2f * (1f - t));

            // 颜色渐变到红色
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.Lerp(startColor, _triggeredColor, t);
            }

            yield return null;
        }

        // 动画结束后销毁
        Destroy(gameObject);
        Debug.Log($"[LaneCar] 小车 {_laneIndex} 已销毁");
    }

    /// <summary>
    /// 更新视觉状态
    /// </summary>
    private void UpdateVisual()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = IsTriggered ? _triggeredColor : _activeColor;
        }
    }

    private void OnDestroy()
    {
        // 从 LaneManager 注销
        if (LaneManager.Instance != null)
        {
            LaneManager.Instance.UnregisterCar(_laneIndex);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        Gizmos.color = IsTriggered ? Color.red : Color.green;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f,
            $"LaneCar [{_laneIndex}] {(IsTriggered ? "已触发" : "就绪")}");
    }
#endif
}
