using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IPossessable))]
public class IPossessableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        MonoBehaviour mb = property.objectReferenceValue as MonoBehaviour;

        if (mb is IPossessable)
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(MonoBehaviour), true);
        else
            EditorGUI.LabelField(position, label.text, "Must assign a MonoBehaviour that implements IPossessable.");

        EditorGUI.EndProperty();
    }
}