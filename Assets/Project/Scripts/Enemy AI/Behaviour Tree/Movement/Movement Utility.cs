using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementUtility : MonoBehaviour
{
    private static readonly Dictionary<GameObject, Dictionary<Type, Component>> gameObjectComponentMap = new();
    private static readonly Dictionary<GameObject, Dictionary<Type, Component>> gameObjectParentComponentMap = new();
    private static readonly Dictionary<GameObject, Dictionary<Type, Component[]>> gameObjectComponentsMap = new();

    public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, Collider[] overlapColliders, LayerMask objectLayerMask, Vector3 targetOffset, LayerMask ignoreLayerMask, bool drawDebugRay)
    {
        GameObject objectFound = null;
        var hitCount = Physics.OverlapSphereNonAlloc(transform.TransformPoint(positionOffset), viewDistance, overlapColliders, objectLayerMask, QueryTriggerInteraction.Ignore);
        if (hitCount > 0)
        {
#if UNITY_EDITOR
            if (hitCount == overlapColliders.Length)
                Logging.LogWarning("Warning: The hit count is equal to the max collider array size. This will cause objects to be missed. Consider increasing the max collision count size.");

#endif
            float minAngle = Mathf.Infinity;
            for (int i = 0; i < hitCount; ++i)
            {
                GameObject obj;

                if ((obj = WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, overlapColliders[i].gameObject, targetOffset, out float angle, ignoreLayerMask, drawDebugRay)) != null)
                {
                    if (angle < minAngle)
                    {
                        minAngle = angle;
                        objectFound = obj;
                    }
                }
            }
        }
        return objectFound;
    }

    public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, LayerMask ignoreLayerMask, bool drawDebugRay)
        => WithinSight(transform, positionOffset, fieldOfViewAngle, viewDistance, targetObject, targetOffset, out _, ignoreLayerMask, drawDebugRay);

    public static GameObject WithinSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float viewDistance, GameObject targetObject, Vector3 targetOffset, out float angle, int ignoreLayerMask, bool drawDebugRay)
    {
        if (targetObject == null)
        {
            angle = 0;
            return null;
        }

        var direction = targetObject.transform.TransformPoint(targetOffset) - transform.TransformPoint(positionOffset);

        angle = Vector3.Angle(direction, transform.forward);
        direction.y = 0;

        if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
        {
            var hitTransform = LineOfSight(transform, positionOffset, targetObject, targetOffset, ignoreLayerMask, drawDebugRay);
            if (hitTransform != null)
            {
                if (IsAncestor(targetObject.transform, hitTransform))
                {
#if UNITY_EDITOR
                    if (drawDebugRay)
                        Debug.DrawLine(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), Color.green);

#endif
                    return targetObject;
#if UNITY_EDITOR
                }
                else
                {
                    if (drawDebugRay)
                        Debug.DrawLine(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), Color.yellow);

#endif
                }
            }
            else if (GetComponentForType<Collider>(targetObject) == null)
            {
                if (targetObject.activeSelf)
                    return targetObject;
            }
        }
        else
        {
#if UNITY_EDITOR
            if (drawDebugRay)
                Debug.DrawLine(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), angle >= fieldOfViewAngle * 0.5f ? Color.red : Color.magenta);

#endif
        }

        return null;
    }

    public static Transform LineOfSight(Transform transform, Vector3 positionOffset, GameObject targetObject, Vector3 targetOffset, int ignoreLayerMask, bool drawDebugRay)
    {
        Transform hitTransform = null;

        if (Physics.Linecast(transform.TransformPoint(positionOffset), targetObject.transform.TransformPoint(targetOffset), out RaycastHit hit, ~ignoreLayerMask, QueryTriggerInteraction.Ignore))
            hitTransform = hit.transform;

        return hitTransform;
    }

    public static bool IsAncestor(Transform target, Transform hitTransform) => hitTransform.IsChildOf(target) || target.IsChildOf(hitTransform);

    public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, float hearingRadius, Collider[] overlapColliders, LayerMask objectLayerMask)
    {
        GameObject objectHeard = null;
        var hitCount = Physics.OverlapSphereNonAlloc(transform.TransformPoint(positionOffset), hearingRadius, overlapColliders, objectLayerMask, QueryTriggerInteraction.Ignore);
        if (hitCount > 0)
        {
#if UNITY_EDITOR
            if (hitCount == overlapColliders.Length)
                Logging.LogWarning("Warning: The hit count is equal to the max collider array size. This will cause objects to be missed. Consider increasing the max collision count size.");

#endif
            float maxAudibility = 0;
            for (int i = 0; i < hitCount; ++i)
            {
                float audibility = 0;
                GameObject obj;

                if ((obj = WithinHearingRange(transform, positionOffset, audibilityThreshold, overlapColliders[i].gameObject, ref audibility)) != null)
                {
                    if (audibility > maxAudibility)
                    {
                        maxAudibility = audibility;
                        objectHeard = obj;
                    }
                }
            }
        }

        return objectHeard;
    }

    public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject)
    {
        float audibility = 0;
        return WithinHearingRange(transform, positionOffset, audibilityThreshold, targetObject, ref audibility);
    }

    public static GameObject WithinHearingRange(Transform transform, Vector3 positionOffset, float audibilityThreshold, GameObject targetObject, ref float audibility)
    {
        AudioSource[] colliderAudioSource;

        if ((colliderAudioSource = GetComponentsForType<AudioSource>(targetObject)) != null)
        {
            for (int i = 0; i < colliderAudioSource.Length; ++i)
            {
                if (colliderAudioSource[i].isPlaying)
                {
                    var distance = Vector3.Distance(transform.position, targetObject.transform.position);
                    if (distance >= colliderAudioSource[i].maxDistance)
                        audibility = 0;
                    else
                    {
                        if (colliderAudioSource[i].rolloffMode == AudioRolloffMode.Logarithmic)
                            audibility = 1 / (1 + colliderAudioSource[i].maxDistance * (distance - 1));
                        else
                            audibility = colliderAudioSource[i].volume * Mathf.Clamp01((distance - colliderAudioSource[i].minDistance) / (colliderAudioSource[i].maxDistance - colliderAudioSource[i].minDistance));
                    }

                    if (audibility > audibilityThreshold)
                        return targetObject;
                }
            }
        }

        return null;
    }

    public static void DrawLineOfSight(Transform transform, Vector3 positionOffset, float fieldOfViewAngle, float angleOffset, float viewDistance)
    {
#if UNITY_EDITOR
        var oldColor = UnityEditor.Handles.color;
        var color = Color.yellow;
        color.a = 0.1f;
        UnityEditor.Handles.color = color;

        var halfFOV = fieldOfViewAngle * 0.5f + angleOffset;
        var beginDirection = Quaternion.AngleAxis(-halfFOV, transform.up) * transform.forward;
        UnityEditor.Handles.DrawSolidArc(transform.TransformPoint(positionOffset), transform.up, beginDirection, fieldOfViewAngle, viewDistance);

        UnityEditor.Handles.color = oldColor;
#endif
    }

    public static T GetComponentForType<T>(GameObject target) where T : Component
    {
        Component targetComponent;

        if (gameObjectComponentMap.TryGetValue(target, out Dictionary<Type, Component> typeComponentMap))
        {
            if (typeComponentMap.TryGetValue(typeof(T), out targetComponent))
                return targetComponent as T;
        }
        else
        {
            typeComponentMap = new Dictionary<Type, Component>();
            gameObjectComponentMap.Add(target, typeComponentMap);
        }

        targetComponent = target.GetComponent<T>();
        typeComponentMap.Add(typeof(T), targetComponent);
        return targetComponent as T;
    }

    public static T GetParentComponentForType<T>(GameObject target) where T : Component
    {
        Component targetComponent;

        if (gameObjectParentComponentMap.TryGetValue(target, out Dictionary<Type, Component> typeComponentMap))
        {
            if (typeComponentMap.TryGetValue(typeof(T), out targetComponent))
                return targetComponent as T;
        }
        else
        {
            typeComponentMap = new Dictionary<Type, Component>();
            gameObjectParentComponentMap.Add(target, typeComponentMap);
        }

        targetComponent = target.GetComponentInParent<T>();
        typeComponentMap.Add(typeof(T), targetComponent);
        return targetComponent as T;
    }

    public static T[] GetComponentsForType<T>(GameObject target) where T : Component
    {
        Component[] targetComponents;

        if (gameObjectComponentsMap.TryGetValue(target, out Dictionary<Type, Component[]> typeComponentsMap))
        {
            if (typeComponentsMap.TryGetValue(typeof(T), out targetComponents))
                return targetComponents as T[];
        }
        else
        {
            typeComponentsMap = new Dictionary<Type, Component[]>();
            gameObjectComponentsMap.Add(target, typeComponentsMap);
        }

        targetComponents = target.GetComponents<T>();
        typeComponentsMap.Add(typeof(T), targetComponents);
        return targetComponents as T[];
    }

    public static void ClearCache()
    {
        gameObjectComponentMap.Clear();
        gameObjectComponentsMap.Clear();
    }
}