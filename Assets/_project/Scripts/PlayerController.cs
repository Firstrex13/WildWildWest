using Cinemachine;
using Cinemachine.Utility;
using System;
using UnityEngine;

public class PlayerController :MonoBehaviour
{
    private const float ZeroF = 0f;

    [SerializeField] private InputReader _input;
    [SerializeField] private float _forwardSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private CinemachineFreeLook _virtualCamera;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _cameraFollowPoint;

    private Transform _mainCamera;

    private float _currentSpeed;
    private float _velocity;

    private Vector3 _adjustedDirection;

    public bool Aiming;

    private void Awake()
    {
        _mainCamera = Camera.main.transform;
        _virtualCamera.Follow = _cameraFollowPoint.transform;
        _virtualCamera.LookAt = _cameraFollowPoint.transform;

        _virtualCamera.OnTargetObjectWarped(transform, transform.position - _virtualCamera.transform.position - Vector3.forward);
    }

    private void OnEnable()
    {
        _input.Aim += OnAim;
    }

    private void OnDisable()
    {
        _input.Aim -= OnAim;
    }

    private void OnAim(bool isAimed)
    {
        Aiming = isAimed;  
    }

    private void Start()
    {
        _input.EnablePlayerActions();
    }

    private void Update()
    {
        HandleMovement();

        if (Aiming)
        {
            Vector3 rotation = Vector3.ProjectOnPlane(_mainCamera.forward, Vector3.up);
            HandleRotation(rotation);         
        }     
    }

    private void HandleMovement()
    {
        var movementDirection = new Vector3(_input.Direction.x, 0, _input.Direction.y).normalized;
        _adjustedDirection = Quaternion.AngleAxis(_mainCamera.eulerAngles.y, Vector3.up) * movementDirection;

        if(_adjustedDirection.magnitude > ZeroF)
        {
            HandleRotation(_adjustedDirection);
            HandleMovement(_adjustedDirection);

            SmoothSpeed(_adjustedDirection.magnitude);
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
    }

    private void SmoothSpeed(float value)
    {
        _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, smoothTime);
    }
}
