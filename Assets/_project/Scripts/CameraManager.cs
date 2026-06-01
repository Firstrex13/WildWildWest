using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private InputReader _input;
    [SerializeField] private CinemachineFreeLook _freeLookCamera;

    [Header("Settings")]
    [SerializeField, Range(0.5f, 3f)] private float _speedMultiplayer;

    bool _isRMBPressed;
    bool _cameraMovementLock;

    private void OnEnable()
    {
        _input.Look += OnLook;
        _input.EnableMouseControlCamera += OnEnableMouseControlCamera;
        _input.DisableMouseControlCamera += OnDisableMouseControlCamera;
    }

    private void OnDisable()
    {
        _input.Look -= OnLook;
        _input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
        _input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
    }

    private void OnDisableMouseControlCamera()
    {
        _isRMBPressed = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
        _freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
    }

    private void OnEnableMouseControlCamera()
    {
        _isRMBPressed = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(DisableMouseForFrame());
    }

    private IEnumerator DisableMouseForFrame()
    {
        _cameraMovementLock = true;
        yield return new WaitForEndOfFrame();
        _cameraMovementLock = false;
    }

    private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
    {
        if (_cameraMovementLock) return;
        if (isDeviceMouse && !_isRMBPressed) return;

        float deviceMultiplayer = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

        _freeLookCamera.m_XAxis.m_InputAxisValue = cameraMovement.x * _speedMultiplayer * deviceMultiplayer;
        _freeLookCamera.m_YAxis.m_InputAxisValue = cameraMovement.y * _speedMultiplayer * deviceMultiplayer;
    }
}
