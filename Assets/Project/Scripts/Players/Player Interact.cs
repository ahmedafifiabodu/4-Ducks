using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private PlayerType _playerType;

    private ServiceLocator serviceLocator;
    private UISystem _playerUI;
    private InputManager _inputManager;
    private Interactable currentInteractable;

    private bool hasPlayedInteractSFX = false;
    private bool _outlineEnabled = true;

    private void Start()
    {
        serviceLocator = ServiceLocator.Instance;

        _playerUI = serviceLocator.GetService<UISystem>();
        _inputManager = serviceLocator.GetService<InputManager>();

        _playerUI.DisablePromptText();

        if (_playerType.Cat)
            _inputManager.PlayerActions.Interact.performed += _ => StartInteraction();
        else if (_playerType.Ghost)
            _inputManager.GhostActions.Interact.performed += _ => StartInteraction();
    }

    private void OnDestroy()
    {
        if (_playerType.Cat)
            _inputManager.PlayerActions.Interact.performed -= _ => StartInteraction();
        else if (_playerType.Ghost)
            _inputManager.GhostActions.Interact.performed -= _ => StartInteraction();
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
            bool shouldInteract = currentInteractable.AutoInteract || _inputManager.PlayerActions.Interact.triggered;

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