using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class DroneMovement : MonoBehaviour
    {
        [Header("Drone Movement")]
        [Tooltip("Move speed of the drone in m/s")]
        public float MoveSpeed = 5.0f;
        [Tooltip("Sprint speed of the drone in m/s")]
        public float SprintSpeed = 8.0f;
        [Tooltip("Rotation speed of the drone")]
        public float RotationSpeed = 2.0f;
        [Tooltip("Smooth acceleration/deceleration")]
        public float SpeedChangeRate = 5.0f;

        [Header("Drone Flight")]
        [Tooltip("Vertical speed for ascending/descending")]
        public float VerticalSpeed = 3.0f;
        [Tooltip("Smooth vertical acceleration")]
        public float VerticalAcceleration = 5.0f;

        [Header("Cinemachine")]
        [Tooltip("Camera target for rotation control")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("Max camera look up")]
        public float TopClamp = 90.0f;
        [Tooltip("Max camera look down")]
        public float BottomClamp = -90.0f;

        // Private variables
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        }

        private void Update()
        {
            HandleFlight();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // Apply movement
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

 private void HandleFlight()
{
    float targetVerticalSpeed = 0f; // Default no movement

    // Ascend when Space is pressed
    if (_input.jump)
    {
        targetVerticalSpeed = VerticalSpeed;
    }
    // Descend when CTRL is pressed
    else if (_input.descend)
    {
        targetVerticalSpeed = -VerticalSpeed;
    }

    // Smooth transition for realistic drone-like movement
    _verticalVelocity = Mathf.Lerp(_verticalVelocity, targetVerticalSpeed, Time.deltaTime * VerticalAcceleration);

    // Stop movement when input is released
    if (!_input.jump && !_input.descend && Mathf.Abs(_verticalVelocity) < 0.1f)
    {
        _verticalVelocity = 0f; // Hard stop to prevent drift
    }
}



        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}
