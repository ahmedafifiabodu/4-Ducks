using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable _interactable = (Interactable)target;
        SerializedObject so = new(target);

        _interactable.InteractableLayerMask = EditorGUILayout.MaskField("Interactable Layer Mask", _interactable.InteractableLayerMask, UnityEditorInternal.InternalEditorUtility.layers);

        SerializedProperty outlineMaterialProperty = so.FindProperty("_outlineMaterial");
        EditorGUILayout.PropertyField(outlineMaterialProperty);

        if (outlineMaterialProperty.objectReferenceValue != null)
        {
            SerializedProperty renderersProperty = so.FindProperty("renderers");
            EditorGUILayout.PropertyField(renderersProperty, true);
        }

        SerializedProperty interactProperty = so.FindProperty("_interact");
        SerializedProperty autoInteractProperty = so.FindProperty("_autoInteract");
        SerializedProperty useEventsProperty = so.FindProperty("_useEvents");
        SerializedProperty possessableProperty = so.FindProperty("_possessable");
        SerializedProperty possessableScriptProperty = so.FindProperty("_possessableScript");

        if (!autoInteractProperty.boolValue && !interactProperty.boolValue)
        {
            EditorGUILayout.PropertyField(possessableProperty);
        }

        if (possessableProperty.boolValue)
        {
            _interactable.PromptMessage = EditorGUILayout.TextField("Prompt Message", _interactable.PromptMessage);
        }

        if (possessableProperty.boolValue)
        {
            EditorGUILayout.PropertyField(possessableScriptProperty);

            if (possessableScriptProperty.objectReferenceValue is not IPossessable)
            {
                EditorGUILayout.HelpBox("The assigned script does not implement the IPossessable interface!", MessageType.Error);
                possessableScriptProperty.objectReferenceValue = null;
            }

            autoInteractProperty.boolValue = false;
            useEventsProperty.boolValue = false;
            interactProperty.boolValue = false;
        }
        else
        {
            possessableScriptProperty.objectReferenceValue = null;

            if (!interactProperty.boolValue)
                EditorGUILayout.PropertyField(autoInteractProperty);

            if (!autoInteractProperty.boolValue)
                EditorGUILayout.PropertyField(interactProperty);

            if (autoInteractProperty.boolValue || interactProperty.boolValue)
            {
                possessableProperty.boolValue = false;
                EditorGUILayout.PropertyField(useEventsProperty);
            }
        }

        if (_interactable.gameObject.TryGetComponent<Collider>(out var collider))
            collider.isTrigger = true;
        else
        {
            collider = _interactable.gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        if (interactProperty.boolValue)
            _interactable.PromptMessage = EditorGUILayout.TextField("Prompt Message", _interactable.PromptMessage);

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