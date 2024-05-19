using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackingScriptableObject))]
public class AttackingScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AttackingScriptableObject script = (AttackingScriptableObject)target;

        EditorGUILayout.LabelField("Attack Configuration", EditorStyles.boldLabel);
        script.Damage = EditorGUILayout.IntField("Damage", script.Damage);
        script.AttackRadius = EditorGUILayout.FloatField("Attack Radius", script.AttackRadius);
        script.AttackDelay = EditorGUILayout.FloatField("Attack Delay", script.AttackDelay);

        EditorGUILayout.LabelField("Ranged Configuration", EditorStyles.boldLabel);
        script.IsRanged = EditorGUILayout.Toggle("Is Ranged", script.IsRanged);

        if (script.IsRanged)
        {
            script.IsHomingBullet = EditorGUILayout.Toggle("Is Homing Bullet", script.IsHomingBullet);
            if (script.IsHomingBullet)
            {
                script.HomingBulletPrefab = (HomingBullet)EditorGUILayout.ObjectField("Homing Bullet Prefab", script.HomingBulletPrefab, typeof(HomingBullet), false);
            }
            else
            {
                Bullet selectedBullet = (Bullet)EditorGUILayout.ObjectField("Bullet Prefab", script.BulletPrefab, typeof(Bullet), false);
                if (selectedBullet != null && selectedBullet.GetType() != typeof(Bullet))
                {
                    // If a HomingBullet is selected, clear the field and display a warning
                    selectedBullet = null;
                    EditorGUILayout.HelpBox("Only Bullet can be selected.", MessageType.Warning);
                }
                script.BulletPrefab = selectedBullet;
            }

            script.BulletSpawnOffset = EditorGUILayout.Vector3Field("Bullet Spawn Offset", script.BulletSpawnOffset);
            script.LineOfSightLayers = LayerMaskField("Layer Mask", script.LineOfSightLayers);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        var layers = UnityEditorInternal.InternalEditorUtility.layers;

        layerMask.value = EditorGUILayout.MaskField(label, layerMask.value, layers);

        return layerMask;
    }
}