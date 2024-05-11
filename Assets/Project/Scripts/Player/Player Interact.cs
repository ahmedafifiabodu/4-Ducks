using Fungus;
using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Material _outLineMaterial;
    [SerializeField] private LayerMask _interactableLayerMask;

    private ServiceLocator serviceLocator;
    private UISystem _playerUI;
    private InputManager _inputManager;
    private AudioManager _audioManager;

    private Interactable currentInteractable;

    private bool hasPlayedInteractSFX = false;

    private void Start()
    {
        serviceLocator = ServiceLocator.Instance;

        _playerUI = serviceLocator.GetService<UISystem>();
        _inputManager = serviceLocator.GetService<InputManager>();
        _audioManager = serviceLocator.GetService<AudioManager>();

        _playerUI.DisablePromptText();

        _inputManager.PlayerActions.Interact.performed += _ => StartInteraction();
    }

    private void OnDestroy() => _inputManager.PlayerActions.Interact.performed -= _ => StartInteraction();

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _interactableLayerMask) != 0)
        {
            if (other.TryGetComponent<Interactable>(out var _interactable))
            {
                _interactable.Initialize(_outLineMaterial);
                _interactable.ApplyOutline();
            }

            currentInteractable = _interactable;

            if (!_interactable.AutoInteract)
            {
                _playerUI.UpdatePromptText(_interactable.PromptMessage);
                StartInteraction();
            }
            else
                StartInteraction();
        }

    }

    void Test()
    {
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _interactableLayerMask) != 0)
        {
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
            bool shouldInteract = currentInteractable.AutoInteract || _inputManager.PlayerActions.Interact.triggered;

            if (shouldInteract)
            {
                if (!hasPlayedInteractSFX)
                {
                    _audioManager.PlaySFX(_audioManager._interact);
                    hasPlayedInteractSFX = true;
                }

                currentInteractable.BaseInteract();
            }
        }
    }
}