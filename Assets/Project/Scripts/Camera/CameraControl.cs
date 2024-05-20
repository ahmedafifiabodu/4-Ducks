using Cinemachine;
using System;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("start and End Values")]
    [Space(3)]
    [SerializeField] private float _startFOV;

    [SerializeField] private float _endFOV;

    [SerializeField] private Quaternion _startXRotation;
    [SerializeField] private Quaternion _endXRotation;

    [SerializeField] private float _maxDistance = 10.0f;
    [SerializeField] private float _minDistance = 10.0f;

    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private Camera _camera;

    [Range(0, 10)][SerializeField] private float duration = 2.0f;
    [Range(0, 1)][SerializeField] private float _elapsedTime = 0.0f;

    private void LateUpdate() => MoveCamera();

    private void MoveCamera()
    {
        float averageDistance = CalculateAverageDistance(_targetGroup);
        if (averageDistance >= _minDistance && averageDistance < _maxDistance)
        {
            _elapsedTime += Time.deltaTime;

            // Lerp Factor
            float step = Mathf.Clamp(_elapsedTime / duration, 0.0f, 1.0f);
            _camera.fieldOfView = Mathf.Lerp(_startFOV, _endFOV, step);
            transform.rotation = Quaternion.Lerp(
                 _startXRotation, _endXRotation, step);
        }
        if (averageDistance < _minDistance)
        {
            _elapsedTime += Time.deltaTime;

            float step = Mathf.Clamp(_elapsedTime / duration, 0.0f, 1.0f);
            _camera.fieldOfView = Mathf.Lerp(_endFOV, _startFOV, step);
            transform.rotation = Quaternion.Slerp(
                 _endXRotation, _startXRotation, step);
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
                totalDistance += Vector3.Distance(targetGroup.m_Targets[i].target.position, targetGroup.m_Targets[j].target.position);
                count++;
            }
        }
        return count > 0 ? totalDistance / count : 0f;
    }
}