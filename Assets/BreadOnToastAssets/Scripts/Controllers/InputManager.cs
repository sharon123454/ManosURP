using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    internal InputAction CameraZoom { get; private set; }
    internal InputAction RotateLeft { get; private set; }
    internal InputAction RotateRight { get; private set; }

    //Questionable
    internal InputAction Test { get; private set; }
    internal InputAction Space { get; private set; }
    internal InputAction Pause { get; private set; }
    internal InputAction Interact { get; private set; }
    internal InputAction NumberKey { get; private set; }
    internal InputAction HighlightInfo { get; private set; }
    internal InputAction SwitchSelectedPlayer { get; private set; }

    private PlayerInputActions _playerInputActions;
    private Vector3 _cameraRotationVector;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"There's more than one InputManager! {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        CameraZoom = _playerInputActions.Player.CameraZoom;
        RotateLeft = _playerInputActions.Player.RotateLeft;
        RotateRight = _playerInputActions.Player.RotateRight;

        Test = _playerInputActions.Player.Test;
        Space = _playerInputActions.Player.Space;
        Pause = _playerInputActions.Player.Pause;
        Interact = _playerInputActions.Player.Interact;
        NumberKey = _playerInputActions.Player.NumberKey;
        HighlightInfo = _playerInputActions.Player.HighlightInfo;
        SwitchSelectedPlayer = _playerInputActions.Player.SwitchSelectedPlayer;
    }
    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
    }

    public bool IsMouseButtonDown() { return Mouse.current.leftButton.wasPressedThisFrame; }
    public bool IsRightMouseButtonDown() { return Mouse.current.rightButton.wasPressedThisFrame; }

    /// <summary>
    /// Returns the mouse screen position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPointerPosition() { return Mouse.current.position.ReadValue(); }
    public Vector3 GetPointerDelta() { return Mouse.current.delta.ReadValue(); }

    public Vector3 GetCameraMoveVector(Transform movingObject)
    {
        Vector2 _playerMoveInput = _playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
        Vector3 _InputAsVector3 = new Vector3(_playerMoveInput.x, 0, _playerMoveInput.y);
        Vector3 _moveDirection = movingObject.forward * _InputAsVector3.z + movingObject.right * _InputAsVector3.x;

        return _moveDirection.normalized;
    }
    public Vector3 GetCameraRotateVector()
    {
        float _playerRotateInput = _playerInputActions.Player.CameraRotate.ReadValue<float>();
        _cameraRotationVector = new Vector3(0, _playerRotateInput, 0);
        return _cameraRotationVector;
    }

}