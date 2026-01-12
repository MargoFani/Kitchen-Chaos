using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_perfomed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_perfomed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }
    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact_perfomed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_perfomed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_perfomed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
         OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    private void InteractAlternate_perfomed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
