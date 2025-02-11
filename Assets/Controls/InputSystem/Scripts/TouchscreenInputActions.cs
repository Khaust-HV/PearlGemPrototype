using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class TouchscreenInputActions {
    private InputMap _inputMap;

    public event Action<bool> FirstTouchContact;
    public event Action<Vector2> SingleSwipeOnScreenDelta;

    public TouchscreenInputActions(InputMap inputMap) {
        _inputMap = inputMap;
    }

    public void SetGameplayInputActive(bool isActive) {
        if (isActive) {
            _inputMap.GameplayInput.Enable();

            _inputMap.GameplayInput.FirstTouchContact.started += _ => FirstTouchEnable();
            _inputMap.GameplayInput.FirstTouchContact.canceled += _ => FirstTouchDisable();
            _inputMap.GameplayInput.SingleSwipeOnScreen.performed += SingleSwipeOnScreen;
        } else {
            _inputMap.GameplayInput.Disable();

            _inputMap.GameplayInput.FirstTouchContact.started -= _ => FirstTouchEnable();
            _inputMap.GameplayInput.FirstTouchContact.canceled -= _ => FirstTouchDisable();
            _inputMap.GameplayInput.SingleSwipeOnScreen.performed -= SingleSwipeOnScreen;
        }
    }

    private void FirstTouchEnable() {
        FirstTouchContact?.Invoke(true);
    }

    private void FirstTouchDisable() {
        FirstTouchContact?.Invoke(false);
    }

    private void SingleSwipeOnScreen(InputAction.CallbackContext context) {
        SingleSwipeOnScreenDelta?.Invoke(context.ReadValue<Vector2>());
    }
}