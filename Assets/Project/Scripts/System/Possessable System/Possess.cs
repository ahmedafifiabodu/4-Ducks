using DG.Tweening;
using UnityEngine;

// The Possess class inherits from the Interactable class
public class Possess : Interactable
{
    // A serialized MonoBehaviour field that can be set in the Unity editor
    [SerializeField] private MonoBehaviour _possessableScript;

    private InputManager _inputManager;
    private Vector3 _originalScale;

    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    // The Interact method is overridden from the Interactable class
    protected override void Interact(ObjectType _playerType)
    {
        // Check if the _ObjectType is not null and if it's a ghost
        if (_playerType != null && _playerType.IsGhost)
        {
            // Set the original scale of the player
            _originalScale = _playerType.transform.localScale;

            // Set the GhostPlayer property of the IPossessable interface to the player's game object
            _possessableScript.GetComponent<IPossessable>().GhostPlayer = _playerType.gameObject;

            // Disable the GhostActions in the InputManager
            _inputManager.GhostActions.Disable();

            // Play the possess animation
            _playerType.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).SetEase(Ease.InOutElastic).SetLoops(2, LoopType.Yoyo).SetDelay(0.5f).OnComplete(() =>
            {
                // Reset the scale back to (1,1,1)
                _playerType.transform.localScale = _originalScale;

                // Call the Interact method of the base class
                base.Interact(_playerType);

                // Call the Possess method of the IPossessable interface
                _possessableScript.GetComponent<IPossessable>().Possess();
            });
        }
    }
}