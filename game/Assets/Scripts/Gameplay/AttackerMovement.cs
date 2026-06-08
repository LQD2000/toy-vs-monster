using UnityEngine;

/// <summary>
/// 进攻方移动组件 - 控制怪物沿跑道从右向左移动
/// 从屏幕右侧外走入，到达最左侧触发防线突破判定
/// </summary>
public class AttackerMovement : MonoBehaviour
{
    [Header("移动配置")]
    [SerializeField] private float _moveSpeed = 0.5f;

    private int _currentLane;
    private bool _isMoving;
    private float _targetX;

    public int CurrentLane => _currentLane;
    public bool IsMoving => _isMoving;

    public void Initialize(int lane, float speed)
    {
        _currentLane = lane;
        _moveSpeed = speed;
        _isMoving = true;

        if (GridSystem.Instance != null)
        {
            Vector3 cell0Pos = GridSystem.Instance.GridToWorld(lane, 0);
            _targetX = cell0Pos.x - GridSystem.Instance.CellWidth * 0.5f;
        }
        else
        {
            _targetX = -7f;
        }
    }

    private void Update()
    {
        if (!_isMoving) return;

        Move(Time.deltaTime);
    }

    public void Move(float deltaTime)
    {
        if (!_isMoving) return;

        transform.position += Vector3.left * _moveSpeed * deltaTime;

        if (transform.position.x <= _targetX)
        {
            _isMoving = false;
            OnReachedEnd();
        }
    }

    private void OnReachedEnd()
    {
        Debug.Log($"[AttackerMovement] 怪物到达终点 (Lane: {_currentLane})");

        if (LaneManager.Instance != null)
        {
            bool defended = LaneManager.Instance.OnMonsterReachedEnd(_currentLane);
            if (!defended)
            {
                Debug.Log($"[AttackerMovement] 跑道 {_currentLane} 防线被突破！");
            }
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return("Attacker", gameObject);
        }
        else
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }

    public void Stop()
    {
        _isMoving = false;
    }
}
