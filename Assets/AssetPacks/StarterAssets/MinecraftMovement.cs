using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MinecraftCreativeFlight : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 100.0f;       // Normal movement speed
    public float sprintMultiplier = 5.0f; // Sprint speed multiplier
    public float verticalSpeed = 100.0f;    // Up/down speed

    [Header("Mouse Look Settings")]
    public float lookSensitivity = 15.0f;  // Mouse sensitivity

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera")]
    public GameObject CinemachineCameraTarget;

    private CharacterController _controller;
    private Vector2 _lookInput;
    private Vector3 _moveDirection;
    private float _yaw;
    private float _pitch;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor for FPS-style look
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        float speed = moveSpeed;

        // Sprinting (Shift makes you move faster)
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            speed *= sprintMultiplier;
        }

        // Movement input (WASD)
        Vector3 forward = transform.forward * (Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0);
        Vector3 right = transform.right * (Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0);
        Vector3 movement = (forward + right).normalized * speed;

        // Vertical movement (Space = Up, Ctrl = Down)
        float vertical = 0f;
        if (Keyboard.current.spaceKey.isPressed) vertical += verticalSpeed;
        if (Keyboard.current.leftCtrlKey.isPressed) vertical -= verticalSpeed;

        _moveDirection = new Vector3(movement.x, vertical, movement.z);
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        _lookInput = Mouse.current.delta.ReadValue() * lookSensitivity * Time.deltaTime;

        _yaw += _lookInput.x;
        _pitch -= _lookInput.y;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f); // Prevent looking too far up/down

        // Rotate player body (yaw)
        transform.rotation = Quaternion.Euler(0, _yaw, 0);

        // Rotate Cinemachine Camera (pitch)
        if (CinemachineCameraTarget != null)
        {
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_pitch, 0, 0);
        }
    }
}
