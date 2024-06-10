using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class WithinDistance : Conditional
{
    public SharedDetectionMode m_DetectionMode = DetectionMode.Object | DetectionMode.ObjectList | DetectionMode.Tag | DetectionMode.LayerMask;

    public SharedGameObject m_TargetObject;

    [UnityEngine.Serialization.FormerlySerializedAs("targetObjects")]
    public SharedGameObjectList m_TargetObjects;

    public SharedString m_TargetTag;

    public SharedLayerMask m_TargetLayerMask;

    public int m_MaxCollisionCount = 200;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance that the object needs to be within")]
    public SharedFloat m_Magnitude = 5;

    [BehaviorDesigner.Runtime.Tasks.Tooltip("If true, the object must be within line of sight to be within distance. For example, if this option is enabled then an object behind a wall will not be within distance even though it may " +
         "be physically close to the other object")]
    public SharedBool m_LineOfSight;

    public SharedVector3 m_Offset;

    public SharedVector3 m_TargetOffset;

    public SharedBool m_DrawDebugRay;

    public SharedGameObject m_ReturnedObject;

    private float m_SqrMagnitude;
    private Collider[] m_OverlapColliders;

    public override void OnStart() => m_SqrMagnitude = m_Magnitude.Value * m_Magnitude.Value;

    public override TaskStatus OnUpdate()
    {
        m_ReturnedObject.Value = null;

        if ((m_DetectionMode.Value & DetectionMode.Object) != 0 && m_TargetObject.Value != null)
            if (IsWithinDistance(m_TargetObject.Value))
                m_ReturnedObject.Value = m_TargetObject.Value;

        if (m_ReturnedObject.Value == null && (m_DetectionMode.Value & DetectionMode.ObjectList) != 0)
        {
            for (int i = 0; i < m_TargetObjects.Value.Count; ++i)
            {
                if (m_TargetObjects.Value[i] == null || m_TargetObjects.Value[i] == gameObject)
                    continue;

                if (IsWithinDistance(m_TargetObjects.Value[i]))
                {
                    m_ReturnedObject.Value = m_TargetObjects.Value[i];
                    break;
                }
            }
        }

        if (m_ReturnedObject.Value == null && (m_DetectionMode.Value & DetectionMode.Tag) != 0 && !string.IsNullOrEmpty(m_TargetTag.Value))
        {
            var objects = GameObject.FindGameObjectsWithTag(m_TargetTag.Value);
            for (int i = 0; i < objects.Length; ++i)
            {
                if (objects[i] == null || objects[i] == gameObject)
                    continue;

                if (IsWithinDistance(objects[i]))
                {
                    m_ReturnedObject.Value = objects[i];
                    break;
                }
            }
        }

        if (m_ReturnedObject.Value == null && (m_DetectionMode.Value & DetectionMode.LayerMask) != 0)
        {
            m_OverlapColliders ??= new Collider[m_MaxCollisionCount];

            var count = Physics.OverlapSphereNonAlloc(transform.position, m_Magnitude.Value, m_OverlapColliders, m_TargetLayerMask.Value);
            for (int i = 0; i < count; ++i)
            {
                if (IsWithinDistance(m_OverlapColliders[i].gameObject))
                {
                    m_ReturnedObject.Value = m_OverlapColliders[i].gameObject;
                    break;
                }
            }
        }

        if (m_ReturnedObject.Value != null)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    private bool IsWithinDistance(GameObject target)
    {
        var direction = target.transform.position - (transform.position + m_Offset.Value);

        if (Vector3.SqrMagnitude(direction) < m_SqrMagnitude)
        {
            if (m_LineOfSight.Value)
            {
                var hitTransform = MovementUtility.LineOfSight(transform, m_Offset.Value, target, m_TargetOffset.Value, GameConstant.Layer.IgnoreRaycast, m_DrawDebugRay.Value);

                if (hitTransform != null && MovementUtility.IsAncestor(hitTransform, target.transform))
                    return true;
            }
            else
                return true;
        }

        return false;
    }

    public override void OnReset()
    {
        m_TargetObject = null;
        m_TargetTag = string.Empty;
        m_TargetLayerMask = (LayerMask)0;
        m_Magnitude = 5;
        m_LineOfSight = true;
        m_Offset = Vector3.zero;
        m_TargetOffset = Vector3.zero;
    }

    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR

        if (Owner == null || m_Magnitude == null)
            return;

        var oldColor = UnityEditor.Handles.color;

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, m_Magnitude.Value);
        UnityEditor.Handles.color = oldColor;
#endif
    }
}