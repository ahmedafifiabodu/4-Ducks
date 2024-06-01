using DG.Tweening;
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

            ServiceLocator.Instance.GetService<InputManager>().GhostActions.Disable();

            _playerType.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).SetEase(Ease.InOutElastic).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f).OnComplete(() =>
            {
                // Play the particle effect
                if (UseParticleEffect)
                {
                    Logging.Log("Playing particle effect");

                    // Instantiate the particle system prefab at the ghost's position
                    ParticleSystem particleInstance = Instantiate(InteractionParticals, _playerType.transform.position, Quaternion.identity);

                    // Play the instantiated particle system
                    particleInstance.Play();

                    // Set the particle system's game object to inactive after it finishes playing
                    DOVirtual.DelayedCall(particleInstance.main.duration, () => particleInstance.gameObject.SetActive(false));
                }

                // Reset the scale back to (1,1,1)
                _playerType.transform.localScale = Vector3.one;

                // Call the Possess method of the IPossessable interface
                _possessableScript.GetComponent<IPossessable>().Possess();
            });
        }
    }
}