using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovableObject : MonoBehaviour, IPossessable
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
            _inputManager.PosssessMovableObjectActions.Disable();
    }

    public void Possess()
    {
        _isPossessed = true;
        ((IPossessable)this).GhostPlayer.SetActive(false);

        _inputManager.PosssessMovableObjectActions.Unpossess.performed += _unpossess;
        _inputManager.PosssessMovableObjectActions.Move.performed += _move;
        _inputManager.PosssessMovableObjectActions.Move.canceled += _ => _isMoving = false;

        _inputManager.GhostActions.Disable();
        _inputManager.PosssessMovableObjectActions.Enable();
    }

    public void Unpossess()
    {
        _isPossessed = false;
        ((IPossessable)this).GhostPlayer.SetActive(true);

        _inputManager.PosssessMovableObjectActions.Unpossess.performed -= _unpossess;
        _inputManager.PosssessMovableObjectActions.Move.performed -= _move;
        _inputManager.PosssessMovableObjectActions.Move.canceled -= _ => _isMoving = false;

        _inputManager.GhostActions.Enable();
        _inputManager.PosssessMovableObjectActions.Disable();

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
            Vector2 inputVector = _inputManager.PosssessMovableObjectActions.Move.ReadValue<Vector2>();
            Vector3 movement = _speed * Time.deltaTime * new Vector3(inputVector.x, 0, inputVector.y);
            transform.Translate(movement, Space.World);
        }
    }
}