using System.Collections.Generic;
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
        script.LineOfSightLayers = LayerMaskField(new GUIContent("Layer Mask"), script.LineOfSightLayers);

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
                    selectedBullet = null;
                    EditorGUILayout.HelpBox("Only Bullet can be selected.", MessageType.Warning);
                }
                script.BulletPrefab = selectedBullet;
            }

            script.BulletSpawnOffset = EditorGUILayout.Vector3Field("Bullet Spawn Offset", script.BulletSpawnOffset);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private List<string> GetAllLayers()
    {
        List<string> layers = new List<string>();
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layers.Add(layerName);
            }
        }
        return layers;
    }

    private LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
    {
        var layers = GetAllLayers();

        List<int> layerNumbers = new List<int>();
        for (int i = 0; i < layers.Count; i++)
        {
            int layerNumber = LayerMask.NameToLayer(layers[i]);
            if (layerMask == (layerMask | (1 << layerNumber)))
            {
                layerNumbers.Add(i);
            }
        }

        int mask = 0;
        foreach (var number in layerNumbers)
        {
            mask |= (1 << number);
        }

        mask = EditorGUILayout.MaskField(label, mask, layers.ToArray());

        LayerMask correctedMask = 0;
        for (int i = 0; i < layers.Count; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                int layerIndex = LayerMask.NameToLayer(layers[i]);
                correctedMask |= (1 << layerIndex);
            }
        }

        return correctedMask;
    }
}