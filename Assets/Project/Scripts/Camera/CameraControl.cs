using Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineGroupComposer _groupComposer;
    private CinemachineTransposer _transposer;

    [Header("Frame Size")]
    [Space(3)]
    [SerializeField] private float _maxDistance = 10.0f;
    [SerializeField] private float _minDistance = 10.0f;
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

    [Range(0, 1)][SerializeField] private float duration = 2.0f;
    [Range(0, 1)][SerializeField] private float _elapsedTime = 0.0f;

    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _groupComposer = _virtualCamera.GetCinemachineComponent<CinemachineGroupComposer>();

        _transposer.m_FollowOffset = _startTransposerOffset;
        _groupComposer.m_TrackedObjectOffset = _startAim0ffset;
    }
    void Update()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        float averageDistance = CalculateAverageDistance(_targetGroup);
        if (averageDistance >= _minDistance && averageDistance < _maxDistance)
        {
            _elapsedTime += Time.deltaTime;
            // Calculate the interpolation factor
            float t = Mathf.Clamp(_elapsedTime/ duration, 0.0f, 1.0f);

            _transposer.m_FollowOffset = Vector3.Lerp(_startTransposerOffset, _endtransposerendOffset, t);
            _groupComposer.m_TrackedObjectOffset = Vector3.Lerp(_startAim0ffset, _endAimEndOffset, t);
        }
    }
    float CalculateAverageDistance(CinemachineTargetGroup targetGroup)
    {
        float totalDistance = 0f;
        int count = 0;
        // Calculate the sum of all distances between targets
        for (int i = 0; i < targetGroup.m_Targets.Length; i++)
        {
            for (int j = i + 1; j < targetGroup.m_Targets.Length; j++)
            {
                totalDistance += Vector3.Distance(targetGroup.m_Targets[i].target.position, targetGroup.m_Targets[j].target.position);
                count++;
            }
        }
        // Return the average distance
        return count > 0 ? totalDistance / count : 0f;
    }
}
