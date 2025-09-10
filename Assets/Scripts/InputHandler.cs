using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputActions;
    private InputAction pointAction;
    private InputAction clickAction;
    private InputAction cameraAction;

    public event Action OnCameraAction;
    public event Action<Vector2> OnClick;
    public event Action<Vector2> OnClickRelease;
    public event Action<Vector2> OnPoint;

    void Start()
    {
        inputActions.FindActionMap("Gameplay").Enable();
        pointAction = inputActions.FindAction("Point");
        clickAction = inputActions.FindAction("Click");
        cameraAction = inputActions.FindAction("Camera");

        cameraAction.started += HandleCameraAction;
        clickAction.started += HandleClick;
        clickAction.canceled += HandleRelease;
        pointAction.performed += HandlePoint;
    }

    private void HandleCameraAction(InputAction.CallbackContext context)
    {
        OnCameraAction?.Invoke();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        Vector2 position = pointAction.ReadValue<Vector2>();
        OnClick?.Invoke(position);
    }

    private void HandleRelease(InputAction.CallbackContext context)
    {
        Vector2 position = pointAction.ReadValue<Vector2>();
        OnClickRelease?.Invoke(position);
    }

    private void HandlePoint(InputAction.CallbackContext context)
    {
        Vector2 position = context.ReadValue<Vector2>();
        OnPoint?.Invoke(position);
    }
}
