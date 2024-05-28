using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask;

    private InputManager _inputManager;
    private UISystem _playerUI;
    private Interactable currentInteractable;
    private PlayerType _playerType;

    private ServiceLocator serviceLocator;

    private bool hasPlayedInteractSFX = false;
    private bool _outlineEnabled = true;

    internal LayerMask InteractableLayerMask => _interactableLayerMask;

    private void Awake() => _playerType = GetComponent<PlayerType>();

    private void Start()
    {
        serviceLocator = ServiceLocator.Instance;

        _playerUI = serviceLocator.GetService<UISystem>();
        _inputManager = serviceLocator.GetService<InputManager>();

        _playerUI.DisablePromptText();

        // Check Player Type
        if (_playerType != null)
        {
            if (_playerType.IsPlayerCat)
                _inputManager.CatActions.Interact.performed += _ => StartInteraction();
            else if (_playerType.IsPlayerGhost)
                _inputManager.GhostActions.Interact.performed += _ => StartInteraction();
            else
                Logging.LogError("Player Type not set!");
        }
    }

    private void OnDestroy()
    {
        // Check Player Type
        if (_playerType != null)
        {
            if (_playerType.IsPlayerCat)
                _inputManager.CatActions.Interact.performed -= _ => StartInteraction();
            else if (_playerType.IsPlayerGhost)
                _inputManager.GhostActions.Interact.performed -= _ => StartInteraction();
            else
                Logging.LogError("Player Type not set!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _interactableLayerMask) != 0)
        {
            if (other.TryGetComponent<Interactable>(out var _interactable))
            {
                currentInteractable = _interactable;
                _outlineEnabled = true;
                currentInteractable.ApplyOutline(_outlineEnabled);
            }

            if (!_interactable.AutoInteract)
            {
                _playerUI.UpdatePromptText(_interactable.PromptMessage);
            }
            else
                StartInteraction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _interactableLayerMask) != 0)
        {
            _outlineEnabled = false;
            if (currentInteractable != null)
            {
                currentInteractable.ApplyOutline(_outlineEnabled);
            }
            hasPlayedInteractSFX = false;
            SetCurrentInteractableToNull();
        }
    }

    public void SetCurrentInteractableToNull()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.RemoveOutline();

        _playerUI.DisablePromptText();
        currentInteractable = null;
    }

    private void StartInteraction()
    {
        if (currentInteractable != null)
        {
            currentInteractable.RemoveOutline();
            bool shouldInteract = currentInteractable.AutoInteract || _inputManager.CatActions.Interact.triggered;

            if (shouldInteract)
            {
                if (!hasPlayedInteractSFX)
                {
                    //_audioManager.PlaySFX(_audioManager._interact);
                    hasPlayedInteractSFX = true;
                }

                currentInteractable.BaseInteract(_playerType);
            }
        }
    }
}