using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveableObject : MonoBehaviour, IPossessable
{
    [Header("Movement")]
    [SerializeField] private float _speed = 5f; // Speed of the object

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f; // Delay after unpossessing the object

    [SerializeField] private Collider _collider; // Collider of the object
    [SerializeField] private Rigidbody _rigidbody; // Rigidbody of the object for physics-based movement

    private InputManager _inputManager; // Input manager

    // Actions for unpossessing and moving the object
    private Action<InputAction.CallbackContext> _unpossess;

    private Action<InputAction.CallbackContext> _move;

    private WaitForSeconds waitForSeconds; // Wait for seconds

    // Flags to check if the object is moving or possessed
    private bool _isMoving = false;

    private bool _isPossessed = false;

    GameObject IPossessable.GhostPlayer { get; set; } // Ghost player

    private void Awake()
    {
        // Initialize actions
        _unpossess = _ => Unpossess();
        _move = _ => { _isMoving = true; };

        // Get collider and rigidbody if they are not assigned
        if (_collider == null)
            _collider = GetComponent<Collider>();
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);
    }

    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>(); // Get input manager

    private void OnDisable()
    {
        // Disable input actions when the object is disabled
        if (_inputManager != null)
            _inputManager.PossessMovableObjectActions.Disable();
    }

    public void Possess()
    {
        // Possess the object
        _isPossessed = true;
        ((IPossessable)this).GhostPlayer.SetActive(false);

        // Subscribe to input actions
        _inputManager.PossessMovableObjectActions.Unpossess.performed += _unpossess;
        _inputManager.PossessMovableObjectActions.Move.performed += _move;
        _inputManager.PossessMovableObjectActions.Move.canceled += _ => _isMoving = false;

        // Enable and disable appropriate input actions
        _inputManager.GhostActions.Disable();
        _inputManager.PossessMovableObjectActions.Enable();
    }

    public void Unpossess()
    {
        // Unpossess the object
        _isPossessed = false;
        ((IPossessable)this).GhostPlayer.SetActive(true);

        // Unsubscribe from input actions
        _inputManager.PossessMovableObjectActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessMovableObjectActions.Move.performed -= _move;
        _inputManager.PossessMovableObjectActions.Move.canceled -= _ => _isMoving = false;

        // Enable and disable appropriate input actions
        _inputManager.GhostActions.Enable();
        _inputManager.PossessMovableObjectActions.Disable();

        // Disable the collider and enable it after a delay
        _collider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay());
    }

    private System.Collections.IEnumerator EnableColliderAfterDelay()
    {
        // Wait for the specified delay and then enable the collider
        yield return waitForSeconds;
        _collider.enabled = true;
    }

    private void Update()
    {
        // Move the object if it is possessed and moving
        if (_isPossessed && _isMoving)
        {
            Vector2 inputVector = _inputManager.PossessMovableObjectActions.Move.ReadValue<Vector2>();
            // Convert the 2D input vector into a 3D vector, considering input for horizontal and vertical movement
            Vector3 movement = _speed * Time.deltaTime * new Vector3(inputVector.x, 0, inputVector.y);
            // Apply the movement vector to the current position
            _rigidbody.MovePosition(_rigidbody.position + movement);
        }
    }
}