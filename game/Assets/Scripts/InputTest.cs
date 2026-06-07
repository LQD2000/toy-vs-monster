using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Gameplay.Tap.performed += OnTap;
    }

    private void OnDisable()
    {
        _controls.Gameplay.Tap.performed -= OnTap;
        _controls.Disable();
    }

    private void OnTap(InputAction.CallbackContext ctx)
    {
        Debug.Log($"Tap detected at: {_controls.Gameplay.TapPosition.ReadValue<Vector2>()}");
    }
}
