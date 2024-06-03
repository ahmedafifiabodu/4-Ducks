using Unity.Cinemachine;
using UnityEngine;

public class CM_CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineGroupComposer _groupComposer;
    private CinemachineFollow _transposer;

    [Header("Frame Size")]
    [Space(3)]
    [SerializeField] private float _maxDistance = 100.0f;
    [SerializeField] private float _minDistance = 400.0f;
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

    [Range(0, 10)][SerializeField] private float duration = 2.0f;
    [Range(0, 1)][SerializeField] private float _elapsedTime = 0.0f;

    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFollow>();
        _groupComposer = _virtualCamera.GetCinemachineComponent<CinemachineGroupComposer>();

        _transposer.FollowOffset = _startTransposerOffset;
        _groupComposer.m_TrackedObjectOffset = _startAim0ffset;
    }
    private void LateUpdate()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        float averageDistance = CalculateAverageDistance(_targetGroup);
        if (averageDistance >= _minDistance && averageDistance < _maxDistance)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(_elapsedTime / duration, 0.0f, 1.0f);
            _transposer.FollowOffset = Vector3.Slerp(_startTransposerOffset, _endtransposerendOffset, t);
            _groupComposer.m_TrackedObjectOffset = Vector3.Slerp(_startAim0ffset, _endAimEndOffset, t);
        }
        if (averageDistance < _minDistance)
        {
            _elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(_elapsedTime / duration, 0.0f, 1.0f);
            _transposer.FollowOffset = Vector3.Slerp(_endtransposerendOffset, _startTransposerOffset, t);
            _groupComposer.m_TrackedObjectOffset = Vector3.Slerp(_endAimEndOffset, _startAim0ffset, t);
        }
    }
    float CalculateAverageDistance(CinemachineTargetGroup targetGroup)
    {
        float totalDistance = 0f;
        int count = 0;
        for (int i = 0; i < targetGroup.Targets.Count; i++)
        {
            for (int j = i + 1; j < targetGroup.Targets.Count; j++)
            {
                totalDistance += Vector3.Distance(targetGroup.Targets[i].Object.position, targetGroup.Targets[j].Object.position);
                count++;
            }
        }
        return count > 0 ? totalDistance / count : 0f;
    }
}
