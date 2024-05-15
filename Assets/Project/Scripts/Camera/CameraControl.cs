using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Frame Size")]
    [Space(3)]
    [SerializeField] private float _maxDistance;

    [SerializeField] private float _minDistance;
    [SerializeField] private CinemachineTargetGroup _targetGroup;

    [Header("Start And End Positions")]
    [Space(3)]
    [Header("Body")]
    [SerializeField] private Vector3 _startTransposerOffset;

    [SerializeField] private Vector3 _endtransposerendOffset;

    [Space(1)]
    [Header("Aim")]
    [SerializeField] private Vector3 _startAim0ffset;

    [SerializeField] private Vector3 _endAimEndOffset;

    private ServiceLocator _serviceLocator;
    private InputManager _inputManager;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineGroupComposer _groupComposer;
    private CinemachineTransposer _transposer;
    private float _lastTargetAverageDistance;

    private Action<InputAction.CallbackContext> _startMoveAction;

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;

        _inputManager = _serviceLocator.GetService<InputManager>();

        _startMoveAction = _ => MoveCamera();
        _inputManager.PlayerActions.Move.started += _startMoveAction;

        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _groupComposer = _virtualCamera.GetCinemachineComponent<CinemachineGroupComposer>();

        _transposer.m_FollowOffset = _startTransposerOffset;
        _groupComposer.m_TrackedObjectOffset = _startAim0ffset;

        _lastTargetAverageDistance = CalculateAverageDistance(_targetGroup);
    }

    private void MoveCamera()
    {
        float averageDistance = CalculateAverageDistance(_targetGroup);
        float screenHieght = Screen.height;

        float step = Mathf.Clamp((averageDistance - _lastTargetAverageDistance) / screenHieght, -1.0f, 1.0f);
        
        Logging.Log($"Average Distance {averageDistance} /n Last AD {_lastTargetAverageDistance} /n Screen hieght{screenHieght} /n Step{step}");

        _transposer.m_FollowOffset = Vector3.Lerp(_startTransposerOffset, _endtransposerendOffset, step);
        _groupComposer.m_TrackedObjectOffset = Vector3.Lerp(_startAim0ffset, _endAimEndOffset, step);

        _lastTargetAverageDistance = averageDistance;
    }

    private float CalculateAverageDistance(CinemachineTargetGroup targetGroup)
    {
        //float totalDistance = 0f;
        //int count = 0;

        //for (int i = 0; i < targetGroup.m_Targets.Length; i++)
        //{
        //    for (int j = i + 1; j < targetGroup.m_Targets.Length; j++)
        //    {
        //        totalDistance += Vector3.Distance(_serviceLocator.GetService<CameraInstance>().GetComponent<Camera>().WorldToScreenPoint(targetGroup.m_Targets[i].target.position)
        //            , _serviceLocator.GetService<CameraInstance>().GetComponent<Camera>().WorldToScreenPoint(targetGroup.m_Targets[j].target.position));
        //        count++;
        //    }
        //}
        float Distamce = Vector3.Distance(_serviceLocator.GetService<CameraInstance>().GetComponent<Camera>().WorldToScreenPoint(targetGroup.m_Targets[0].target.position)
                    , _serviceLocator.GetService<CameraInstance>().GetComponent<Camera>().WorldToScreenPoint(targetGroup.m_Targets[1].target.position));
        // return count > 0 ? totalDistance / count : 0f;
        return Distamce;
    }
}