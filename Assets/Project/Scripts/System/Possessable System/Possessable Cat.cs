using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessableCat : MonoBehaviour, IPossessable
{
    [Header("Movement")]
    [SerializeField] private float _invisibilityAbility = 5f;

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f;

    [SerializeField] private Collider _collider;

    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _unpossess;
    private Action<InputAction.CallbackContext> _startInvisibility;
    private Action<InputAction.CallbackContext> _stoptInvisibility;

    private WaitForSeconds waitForSeconds;

    GameObject IPossessable.GhostPlayer { get; set; }

    private void Awake()
    {
        _unpossess = _ => Unpossess();

        _startInvisibility = _ => StartInvisibility();
        _stoptInvisibility = _ => StopInvisibility();

        if (_collider == null)
            _collider = GetComponent<Collider>();

        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);
    }

    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    private void OnDisable()
    {
        if (_inputManager != null)
            _inputManager.PossessCatActions.Disable();
    }

    public void Possess()
    {
        ((IPossessable)this).GhostPlayer.SetActive(false);

        _inputManager.PossessCatActions.Unpossess.performed += _unpossess;
        _inputManager.PossessCatActions.Invisibility.performed += _startInvisibility;
        _inputManager.PossessCatActions.Invisibility.canceled += _stoptInvisibility;

        _inputManager.GhostActions.Disable();
        _inputManager.PossessCatActions.Enable();
    }

    public void Unpossess()
    {
        ((IPossessable)this).GhostPlayer.SetActive(true);

        _inputManager.PossessCatActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessCatActions.Invisibility.performed -= _startInvisibility;
        _inputManager.PossessCatActions.Invisibility.canceled -= _stoptInvisibility;

        _inputManager.GhostActions.Enable();
        _inputManager.PossessCatActions.Disable();

        _collider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay());
    }

    private System.Collections.IEnumerator EnableColliderAfterDelay()
    {
        yield return waitForSeconds;
        _collider.enabled = true;
    }

    private void StartInvisibility()
    {
        Logging.Log("Invisibility" + _invisibilityAbility);
    }

    private void StopInvisibility()
    {
        Logging.Log("Stop Invisibility" + _invisibilityAbility);
    }
}