using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    private SerializedProperty autoInteractProperty;
    private SerializedProperty interactProperty;
    private SerializedProperty useEventsProperty;

    public override void OnInspectorGUI()
    {
        Interactable _interactable = (Interactable)target;
        SerializedObject so = new(target);

        _interactable.InteractableLayerMask = EditorGUILayout.MaskField("Interactable Layer Mask", _interactable.InteractableLayerMask, UnityEditorInternal.InternalEditorUtility.layers);

        autoInteractProperty = so.FindProperty("_autoInteract");
        interactProperty = so.FindProperty("_interact");
        useEventsProperty = so.FindProperty("_useEvents");

        if (!autoInteractProperty.boolValue)
            EditorGUILayout.PropertyField(interactProperty);

        if (!interactProperty.boolValue)
            EditorGUILayout.PropertyField(autoInteractProperty);

        if (autoInteractProperty.boolValue || interactProperty.boolValue)
        {
            if (interactProperty.boolValue)
            {
                SerializedProperty outlineMaterialProperty = so.FindProperty("_outlineMaterial");
                EditorGUILayout.PropertyField(outlineMaterialProperty);

                if (outlineMaterialProperty.objectReferenceValue != null)
                {
                    SerializedProperty renderersProperty = so.FindProperty("renderers");
                    EditorGUILayout.PropertyField(renderersProperty, true);
                }
            }

            EditorGUILayout.PropertyField(useEventsProperty);
        }
        else if (!autoInteractProperty.boolValue && !interactProperty.boolValue)
        {
            useEventsProperty.boolValue = false;
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