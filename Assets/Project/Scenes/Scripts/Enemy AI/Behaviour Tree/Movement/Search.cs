using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Movement")]
public class Search : NavMeshMovement
{
    #region Parameters

    [UnityEngine.Serialization.FormerlySerializedAs("minWanderDistance")]
    public SharedFloat m_MinWanderDistance = 20;

    [UnityEngine.Serialization.FormerlySerializedAs("maxWanderDistance")]
    public SharedFloat m_MaxWanderDistance = 20;

    [UnityEngine.Serialization.FormerlySerializedAs("wanderRate")]
    public SharedFloat m_WanderRate = 1;

    [UnityEngine.Serialization.FormerlySerializedAs("minPauseDuration")]
    public SharedFloat m_MinPauseDuration = 0;

    [UnityEngine.Serialization.FormerlySerializedAs("maxPauseDuration")]
    public SharedFloat m_MaxPauseDuration = 0;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum number of retries per tick (set higher if using a slow tick time)")]
    [UnityEngine.Serialization.FormerlySerializedAs("targetRetries")]
    public SharedInt m_TargetRetries = 1;

    [UnityEngine.Serialization.FormerlySerializedAs("fieldOfViewAngle")]
    public SharedFloat m_FieldOfViewAngle = 90;

    [UnityEngine.Serialization.FormerlySerializedAs("viewDistance")]
    public SharedFloat m_ViewDistance = 30;

    #region Audio

    [BehaviorDesigner.Runtime.Tasks.Tooltip("Should the search end if audio was heard?")]
    [UnityEngine.Serialization.FormerlySerializedAs("senseAudio")]
    public SharedBool m_SenseAudio = true;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("How far away the unit can hear")]
    [UnityEngine.Serialization.FormerlySerializedAs("hearingRadius")]
    public SharedFloat m_HearingRadius = 30;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The further away a sound source is the less likely the agent will be able to hear it. " +
         "Set a threshold for the the minimum audibility level that the agent can hear")]
    [UnityEngine.Serialization.FormerlySerializedAs("audibilityThreshold")]
    public SharedFloat m_AudibilityThreshold = 0.05f;

    #endregion Audio

    [UnityEngine.Serialization.FormerlySerializedAs("offset")]
    public SharedVector3 m_Offset;

    [UnityEngine.Serialization.FormerlySerializedAs("targetOffset")]
    public SharedVector3 m_TargetOffset;

    [UnityEngine.Serialization.FormerlySerializedAs("objectLayerMask")]
    public LayerMask m_TargetLayerMask;

    [UnityEngine.Serialization.FormerlySerializedAs("maxCollisionCount")]
    public int m_MaxCollisionCount = 200;

    [UnityEngine.Serialization.FormerlySerializedAs("drawDebugRay")]
    public SharedBool m_DrawDebugRay;

    [UnityEngine.Serialization.FormerlySerializedAs("returnedObject")]
    public SharedGameObject m_ReturnedObject;

    #endregion Parameters

    private float m_PauseTime;
    private float m_DestinationReachTime;

    private Collider[] m_OverlapColliders;

    public override TaskStatus OnUpdate()
    {
        if (HasArrived())
        {
            if (m_MaxPauseDuration.Value > 0)
            {
                if (m_DestinationReachTime == -1)
                {
                    m_DestinationReachTime = Time.time;
                    m_PauseTime = Random.Range(m_MinPauseDuration.Value, m_MaxPauseDuration.Value);
                }
                else if (m_DestinationReachTime + m_PauseTime <= Time.time)
                {
                    if (TrySetTarget())
                        m_DestinationReachTime = -1;
                }
            }
            else
                TrySetTarget();
        }

        m_OverlapColliders ??= new Collider[m_MaxCollisionCount];
        m_ReturnedObject.Value = MovementUtility.WithinSight(transform, m_Offset.Value, m_FieldOfViewAngle.Value, m_ViewDistance.Value, m_OverlapColliders, m_TargetLayerMask, m_TargetOffset.Value, GameConstant.Layer.IgnoreRaycast, m_DrawDebugRay.Value);

        if (m_ReturnedObject.Value != null)
            return TaskStatus.Success;

        if (m_SenseAudio.Value)
        {
            m_ReturnedObject.Value = MovementUtility.WithinHearingRange(transform, m_Offset.Value, m_AudibilityThreshold.Value, m_HearingRadius.Value, m_OverlapColliders, m_TargetLayerMask);

            if (m_ReturnedObject.Value != null)
                return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private bool TrySetTarget()
    {
        var direction = transform.forward;
        var attempts = m_TargetRetries.Value;
        var destination = transform.position;

        while (attempts > 0)
        {
            direction += Random.insideUnitSphere * m_WanderRate.Value;
            destination = transform.position + direction.normalized * Random.Range(m_MinWanderDistance.Value, m_MaxWanderDistance.Value);

            if (SamplePosition(ref destination))
            {
                SetDestination(destination);
                return true;
            }

            attempts--;
        }

        return false;
    }

    public override void OnReset()
    {
        base.OnReset();

        m_MinWanderDistance = 20;
        m_MaxWanderDistance = 20;
        m_WanderRate = 2;
        m_MinPauseDuration = 0;
        m_MaxPauseDuration = 0;
        m_TargetRetries = 1;
        m_FieldOfViewAngle = 90;
        m_ViewDistance = 30;
        m_DrawDebugRay = false;
        m_SenseAudio = true;
        m_HearingRadius = 30;
        m_AudibilityThreshold = 0.05f;
    }
}