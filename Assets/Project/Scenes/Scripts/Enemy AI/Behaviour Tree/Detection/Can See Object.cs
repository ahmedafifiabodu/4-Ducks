using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanSeeObject : Conditional
{
    #region Parameters

    public SharedDetectionMode m_DetectionMode = DetectionMode.Object | DetectionMode.ObjectList | DetectionMode.Tag | DetectionMode.LayerMask;

    [UnityEngine.Serialization.FormerlySerializedAs("targetObject")]
    public SharedGameObject m_TargetObject;

    [UnityEngine.Serialization.FormerlySerializedAs("targetObjects")]
    public SharedGameObjectList m_TargetObjects;

    [UnityEngine.Serialization.FormerlySerializedAs("targetTag")]
    public SharedString m_TargetTag;

    [UnityEngine.Serialization.FormerlySerializedAs("objectLayerMask")]
    public SharedLayerMask m_TargetLayerMask;

    [UnityEngine.Serialization.FormerlySerializedAs("maxCollisionCount")]
    public int m_MaxCollisionCount = 200;

    [UnityEngine.Serialization.FormerlySerializedAs("fieldOfViewAngle")]
    public SharedFloat m_FieldOfViewAngle = 90;

    [UnityEngine.Serialization.FormerlySerializedAs("viewDistance")]
    public SharedFloat m_ViewDistance = 1000;

    [UnityEngine.Serialization.FormerlySerializedAs("offset")]
    public SharedVector3 m_Offset;

    [UnityEngine.Serialization.FormerlySerializedAs("targetOffset")]
    public SharedVector3 m_TargetOffset;

    [UnityEngine.Serialization.FormerlySerializedAs("drawDebugRay")]
    public SharedBool m_DrawDebugRay;

    [UnityEngine.Serialization.FormerlySerializedAs("disableAgentColliderLayer")]
    public SharedBool m_DisableAgentColliderLayer;

    [UnityEngine.Serialization.FormerlySerializedAs("returnedObject")]
    public SharedGameObject m_ReturnedObject;

    #endregion Parameters

    private GameObject[] m_AgentColliderGameObjects;
    private int[] m_OriginalColliderLayer;
    private Collider[] m_OverlapColliders;

    public override TaskStatus OnUpdate()
    {
        m_ReturnedObject.Value = null;

        if (m_DisableAgentColliderLayer.Value)
        {
            if (m_AgentColliderGameObjects == null)
            {
                var colliders = gameObject.GetComponentsInChildren<Collider>();
                m_AgentColliderGameObjects = new GameObject[colliders.Length];

                for (int i = 0; i < m_AgentColliderGameObjects.Length; ++i)
                    m_AgentColliderGameObjects[i] = colliders[i].gameObject;

                m_OriginalColliderLayer = new int[m_AgentColliderGameObjects.Length];
            }

            for (int i = 0; i < m_AgentColliderGameObjects.Length; ++i)
            {
                m_OriginalColliderLayer[i] = m_AgentColliderGameObjects[i].layer;
                m_AgentColliderGameObjects[i].layer = GameConstant.Layer.IgnoreRaycast;
            }
        }

        if ((m_DetectionMode.Value & DetectionMode.Object) != 0 && m_TargetObject.Value != null)
            m_ReturnedObject.Value = MovementUtility.WithinSight(transform, m_Offset.Value, m_FieldOfViewAngle.Value, m_ViewDistance.Value, m_TargetObject.Value, m_TargetOffset.Value, GameConstant.Layer.IgnoreRaycast, m_DrawDebugRay.Value);

        if (m_ReturnedObject.Value == null && (m_DetectionMode.Value & DetectionMode.ObjectList) != 0)
        {
            var minAngle = Mathf.Infinity;
            for (int i = 0; i < m_TargetObjects.Value.Count; ++i)
            {
                GameObject obj;
                if ((obj = MovementUtility.WithinSight(transform, m_Offset.Value, m_FieldOfViewAngle.Value, m_ViewDistance.Value, m_TargetObjects.Value[i], m_TargetOffset.Value, out var angle, GameConstant.Layer.IgnoreRaycast, m_DrawDebugRay.Value)) != null)
                {
                    if (angle < minAngle)
                    {
                        minAngle = angle;
                        m_ReturnedObject.Value = obj;
                    }
                }
            }
        }

        if (m_ReturnedObject.Value == null && (m_DetectionMode.Value & DetectionMode.Tag) != 0 && !string.IsNullOrEmpty(m_TargetTag.Value))
        {
            var targets = GameObject.FindGameObjectsWithTag(m_TargetTag.Value);
            if (targets != null)
            {
                var minAngle = Mathf.Infinity;
                for (int i = 0; i < targets.Length; ++i)
                {
                    GameObject obj;
                    if ((obj = MovementUtility.WithinSight(transform, m_Offset.Value, m_FieldOfViewAngle.Value, m_ViewDistance.Value,
                        targets[i], m_TargetOffset.Value, out var angle, GameConstant.Layer.IgnoreRaycast, m_DrawDebugRay.Value)) != null)
                    {
                        if (angle < minAngle)
                        {
                            minAngle = angle;
                            m_ReturnedObject.Value = obj;
                        }
                    }
                }
            }
        }

        if (m_ReturnedObject.Value == null && (m_DetectionMode.Value & DetectionMode.LayerMask) != 0)
        {
            m_OverlapColliders ??= new Collider[m_MaxCollisionCount];
            m_ReturnedObject.Value = MovementUtility.WithinSight(transform, m_Offset.Value, m_FieldOfViewAngle.Value, m_ViewDistance.Value, m_OverlapColliders, m_TargetLayerMask.Value, m_TargetOffset.Value, GameConstant.Layer.IgnoreRaycast, m_DrawDebugRay.Value);
        }

        if (m_DisableAgentColliderLayer.Value)
        {
            for (int i = 0; i < m_AgentColliderGameObjects.Length; ++i)
                m_AgentColliderGameObjects[i].layer = m_OriginalColliderLayer[i];
        }

        if (m_ReturnedObject.Value != null)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override void OnReset()
    {
        m_DetectionMode = DetectionMode.Object | DetectionMode.ObjectList | DetectionMode.Tag | DetectionMode.LayerMask;
        m_FieldOfViewAngle = 90;
        m_ViewDistance = 1000;
        m_Offset = Vector3.zero;
        m_TargetOffset = Vector3.zero;
        m_TargetTag = "";
        m_DrawDebugRay = false;
    }

    public override void OnDrawGizmos() => MovementUtility.DrawLineOfSight(Owner.transform, m_Offset.Value, m_FieldOfViewAngle.Value, 0, m_ViewDistance.Value);

    public override void OnBehaviorComplete() => MovementUtility.ClearCache();
}