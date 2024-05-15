using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable _interactable = (Interactable)target;
        SerializedObject so = new(target);

        SerializedProperty possessableProperty = so.FindProperty("_possessable");
        SerializedProperty possessableScriptProperty = so.FindProperty("_possessableScript");
        SerializedProperty autoInteractProperty = so.FindProperty("_autoInteract");
        SerializedProperty useEventsProperty = so.FindProperty("_useEvents");

        EditorGUILayout.PropertyField(possessableProperty);

        if (possessableProperty.boolValue)
        {
            EditorGUILayout.PropertyField(possessableScriptProperty);
            autoInteractProperty.boolValue = false;
            useEventsProperty.boolValue = false;
        }
        else
        {
            EditorGUILayout.PropertyField(autoInteractProperty);
            EditorGUILayout.PropertyField(useEventsProperty);
        }

        if (_interactable.gameObject.TryGetComponent<Collider>(out var collider))
            collider.isTrigger = true;
        else
        {
            collider = _interactable.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        if (!autoInteractProperty.boolValue)
        {
            if (target.GetType() == typeof(EventOnlyInteractables))
            {
                _interactable.PromptMessage = EditorGUILayout.TextField("Prompt Message", _interactable.PromptMessage);
                EditorGUILayout.HelpBox("This interactable will only use events.", MessageType.Info);
            }
        }

        if (_interactable.UseEvents)
        {
            if (_interactable.gameObject.GetComponent<InteractableEvents>() == null)
                _interactable.gameObject.AddComponent<InteractableEvents>();
        }
        else
        {
            if (_interactable.gameObject.GetComponent<InteractableEvents>() != null)
                DestroyImmediate(_interactable.gameObject.GetComponent<InteractableEvents>());
        }

        so.ApplyModifiedProperties();
    }
}