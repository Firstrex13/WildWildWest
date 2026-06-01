using Cinemachine;
using System;
using UnityEngine;


public class PlayerMover :MonoBehaviour
{
    private const float ZeroF = 0f;

    [SerializeField] private InputReader _input;
    [SerializeField] private float _forwardSpeed = 6f;
    [SerializeField] private float _strafeSpeed;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private CinemachineFreeLook _virtualCamera;
    [SerializeField] private CharacterController _characterController;

    private Transform _mainCamera;

    private float _currentSpeed;
    private float _velocity;

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
        _virtualCamera.Follow = transform;
        _virtualCamera.LookAt = transform;

        _virtualCamera.OnTargetObjectWarped(transform, transform.position - _virtualCamera.transform.position - Vector3.forward);
    }

    private void Start()
    {
        _input.EnablePlayerActions();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        var movementDirection = new Vector3(_input.Direction.x, 0, _input.Direction.y).normalized;
        var adjustedDirection = Quaternion.AngleAxis(_mainCamera.eulerAngles.y, Vector3.up) * movementDirection;

        if(adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(adjustedDirection);
            HandleMovement(adjustedDirection);

            SmoothSpeed(adjustedDirection.magnitude);
        }
        else
        {
            SmoothSpeed(ZeroF);
        }
    }

    private void HandleMovement(Vector3 adjustedDirection)
    {
        var adjustedMovement = adjustedDirection * (_forwardSpeed * Time.deltaTime);
        _characterController.Move(adjustedMovement);
    }

    private void HandleRotation(Vector3 adjustedDirection)
    {
        var targetRotation = Quaternion.LookRotation(adjustedDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        transform.LookAt(transform.position + adjustedDirection);
    }

    private void SmoothSpeed(float value)
    {
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, smoothTime);
    }
}
