using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[CreateAssetMenu(fileName = " InputReader", menuName = "WildWest/Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private PlayerInputActions _inputActions;

    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2, bool> Look = delegate { };
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };

    public Vector3 Direction => _inputActions.Player.Move.ReadValue<Vector2>();

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.SetCallbacks(this);
        }     
    }

    public void EnablePlayerActions()
    {
        _inputActions.Enable();
    }

    public void OnFire(InputAction.CallbackContext context)
    {

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look?.Invoke(context.ReadValue<Vector2>(), context);
    }

    public bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera?.Invoke();
                break;
            case InputActionPhase.Canceled:
                DisableMouseControlCamera?.Invoke();
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move?.Invoke(context.ReadValue<Vector2>());
    }
}
