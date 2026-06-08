using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class UnitPlacement : MonoBehaviour
{
    public static UnitPlacement Instance { get; private set; }

    [Header("放置配置")]
    [SerializeField] private LayerMask _gridCellLayer;

    private DefenderData _selectedDefender;
    private PlayerControls _controls;

    public DefenderData SelectedDefender => _selectedDefender;

    public event Action<DefenderData> OnDefenderSelected;
    public event Action<Defender> OnDefenderPlaced;
    public event Action<string> OnPlacementFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _controls = new PlayerControls();
        _controls.Enable();
        _controls.Gameplay.Tap.performed += OnTap;
    }

    private void OnDisable()
    {
        if (_controls == null) return;
        _controls.Gameplay.Tap.performed -= OnTap;
        _controls.Disable();
    }

    private void OnTap(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState != GameState.Preparation)
        {
            return;
        }

        if (_selectedDefender != null)
        {
            TryPlaceDefender();
        }
    }

    public void SelectDefender(DefenderData data)
    {
        _selectedDefender = data;
        OnDefenderSelected?.Invoke(data);
    }

    public void ClearSelection()
    {
        _selectedDefender = null;
    }

    private void TryPlaceDefender()
    {
        Vector2 tapPos = _controls.Gameplay.TapPosition.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(tapPos);
        worldPos.z = 0f;

        if (GridSystem.Instance == null) return;

        var gridPos = GridSystem.Instance.WorldToGrid(worldPos);
        if (gridPos == null)
        {
            OnPlacementFailed?.Invoke("点击位置超出网格范围");
            return;
        }

        int row = gridPos.Value.row;
        int col = gridPos.Value.col;

        if (GridSystem.Instance.IsCellOccupied(row, col))
        {
            OnPlacementFailed?.Invoke("该位置已被占用");
            return;
        }

        if (ResourceManager.Instance == null) return;

        if (!ResourceManager.Instance.HasEnoughPower(_selectedDefender.PowerCost))
        {
            OnPlacementFailed?.Invoke("电力不足");
            return;
        }

        if (DefenderFactory.Instance == null) return;

        Defender defender = DefenderFactory.Instance.CreateDefender(_selectedDefender, row, col);
        if (defender == null)
        {
            OnPlacementFailed?.Invoke("创建单位失败");
            return;
        }

        ResourceManager.Instance.ConsumePower(_selectedDefender.PowerCost);
        GridSystem.Instance.OccupyCell(row, col, defender.gameObject);

        OnDefenderPlaced?.Invoke(defender);
        ClearSelection();
    }
}
