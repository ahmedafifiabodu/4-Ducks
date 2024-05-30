using UnityEngine;

// The Possess class inherits from the Interactable class
public class Possess : Interactable
{
    // A serialized MonoBehaviour field that can be set in the Unity editor
    [SerializeField] private MonoBehaviour _possessableScript;

    // The Interact method is overridden from the Interactable class
    protected override void Interact(ObjectType _playerType)
    {
        // Check if the _ObjectType is not null and if it's a ghost
        if (_playerType != null && _playerType.IsGhost)
        {
            // If the object uses events for interaction
            if (UseEvents)
            {
                // If the object has an InteractableEvents component
                if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                    // Invoke the onInteract event
                    _events.onInteract.Invoke();
            }

            // Set the GhostPlayer property of the IPossessable interface to the player's game object
            _possessableScript.GetComponent<IPossessable>().GhostPlayer = _playerType.gameObject;
            // Call the Possess method of the IPossessable interface
            _possessableScript.GetComponent<IPossessable>().Possess();
        }
    }
}
