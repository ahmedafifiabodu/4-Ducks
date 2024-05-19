using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl1 : MonoBehaviour
{
    [Header("Frame Size")]
    [Space(3)]
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _minDistance;
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private CinemachineVirtualCamera _topDownCamera;
    [SerializeField] private CinemachineVirtualCamera _sideViewwCamera;

    private ServiceLocator _serviceLocator;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startMoveAction;

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;

        _inputManager = _serviceLocator.GetService<InputManager>();

        _startMoveAction = _ => MoveCamera();
        _inputManager.PlayerActions.Move.started += _startMoveAction;
    }

    private void MoveCamera()
    {
        float averageDistance = CalculateAverageDistance(_targetGroup);
        if(averageDistance < _minDistance)
        {
            
            _sideViewwCamera.enabled = true;
            _topDownCamera.enabled = false;
        }
        else if (averageDistance >= _maxDistance)
        {
            _sideViewwCamera.enabled = false;
            _topDownCamera.enabled = true;
        }
    }
    private float CalculateAverageDistance(CinemachineTargetGroup targetGroup)
    {
        float totalDistance = 0f;
        int count = 0;

        for (int i = 0; i < targetGroup.m_Targets.Length; i++)
        {
            for (int j = i + 1; j < targetGroup.m_Targets.Length; j++)
            {
                totalDistance += Vector3.Distance(targetGroup.m_Targets[i].target.position
                    , targetGroup.m_Targets[j].target.position);
                count++;
            }
        }
        return count > 0 ? totalDistance / count : 0f;

    }
}
