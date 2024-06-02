using DG.Tweening;
using UnityEngine;

// The Possess class inherits from the Interactable class
public class Possess : Interactable
{
    // A serialized MonoBehaviour field that can be set in the Unity editor
    [SerializeField] private MonoBehaviour _possessableScript;

    private InputManager _inputManager;
    private ObjectPool _objectPool;

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();
    }

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

            _inputManager.GhostActions.Disable();

            _playerType.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).SetEase(Ease.InOutElastic).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f).OnComplete(() =>
            {
                // Play the particle effect
                if (UseParticleEffect)
                {
                    // Get the particle system from the object pool
                    GameObject particleInstanceObject = _objectPool.GetPooledObject(InteractionParticals.gameObject);

                    // If a particle system was found in the pool
                    if (particleInstanceObject != null)
                    {
                        // Get the ParticleSystem component
                        ParticleSystem particleInstance = particleInstanceObject.GetComponent<ParticleSystem>();

                        // Set the position of the particle system to the ghost's position
                        particleInstance.transform.position = _playerType.transform.position;

                        // Play the particle system
                        particleInstance.Play();

                        // Set the particle system's game object to inactive after it finishes playing
                        DOVirtual.DelayedCall(particleInstance.main.duration, () => particleInstance.gameObject.SetActive(false));
                    }
                }

                // Reset the scale back to (1,1,1)
                _playerType.transform.localScale = Vector3.one;

                // Call the Possess method of the IPossessable interface
                _possessableScript.GetComponent<IPossessable>().Possess();
            });
        }
    }
}