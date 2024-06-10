using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveableObject : MonoBehaviour, IPossessable
{
    [Header("Movement")]
    [SerializeField] private float _speed = 5f;

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f;

    [SerializeField] private Collider _collider;

    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _unpossess;
    private Action<InputAction.CallbackContext> _move;

    private WaitForSeconds waitForSeconds;

    private bool _isMoving = false;
    private bool _isPossessed = false;

    GameObject IPossessable.GhostPlayer { get; set; }

    private void Awake()
    {
        _unpossess = _ => Unpossess();
        _move = _ => { _isMoving = true; };

        if (_collider == null)
            _collider = GetComponent<Collider>();

        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);
    }

    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    private void OnDisable()
    {
        if (_inputManager != null)
            _inputManager.PossessMovableObjectActions.Disable();
    }

    public void Possess()
    {
        _isPossessed = true;
        ((IPossessable)this).GhostPlayer.SetActive(false);

        _inputManager.PossessMovableObjectActions.Unpossess.performed += _unpossess;
        _inputManager.PossessMovableObjectActions.Move.performed += _move;
        _inputManager.PossessMovableObjectActions.Move.canceled += _ => _isMoving = false;

        _inputManager.GhostActions.Disable();
        _inputManager.PossessMovableObjectActions.Enable();
    }

    public void Unpossess()
    {
        _isPossessed = false;
        ((IPossessable)this).GhostPlayer.SetActive(true);

        _inputManager.PossessMovableObjectActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessMovableObjectActions.Move.performed -= _move;
        _inputManager.PossessMovableObjectActions.Move.canceled -= _ => _isMoving = false;

        _inputManager.GhostActions.Enable();
        _inputManager.PossessMovableObjectActions.Disable();

        _collider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay());
    }

    private System.Collections.IEnumerator EnableColliderAfterDelay()
    {
        yield return waitForSeconds;
        _collider.enabled = true;
    }

    private void Update()
    {
        if (_isPossessed && _isMoving)
        {
            Vector2 inputVector = _inputManager.PossessMovableObjectActions.Move.ReadValue<Vector2>();
            Vector3 movement = _speed * Time.deltaTime * new Vector3(-inputVector.y, 0, inputVector.x);
            transform.Translate(movement, Space.World);
        }
    }
}