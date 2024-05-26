using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovableObject : MonoBehaviour, IPossessable
{
    [SerializeField] private float _speed = 5f;

    private Ghost _ghost;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _unpossess;
    private Action<InputAction.CallbackContext> _move;

    private bool _isMoving = false;

    private void OnEnable()
    {
        _unpossess = _ => Unpossess();
        _move = _ => Move();

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.MovableObjectActions.Move.performed += _move;
        _inputManager.MovableObjectActions.Move.canceled += _ => _isMoving = false;
        _inputManager.MovableObjectActions.Disable();
    }

    private void OnDisable()
    {
        _inputManager.MovableObjectActions.Move.performed -= _move;
        _inputManager.MovableObjectActions.Move.canceled -= _ => _isMoving = false;
        _inputManager.MovableObjectActions.Disable();
    }

    public void Possess()
    {
        _ghost.gameObject.SetActive(false);

        _inputManager.GhostActions.Disable();
        _inputManager.MovableObjectActions.Enable();
    }

    public void Unpossess()
    {
        _ghost.gameObject.SetActive(true);

        _inputManager.GhostActions.Enable();
        _inputManager.MovableObjectActions.Disable();

        _inputManager.MovableObjectActions.Interact.performed -= _unpossess;
    }

    private void Move() => _isMoving = true;

    private void Update()
    {
        if (_isMoving)
        {
            Vector2 inputVector = _inputManager.MovableObjectActions.Move.ReadValue<Vector2>();
            Vector3 movement = _speed * Time.deltaTime * new Vector3(inputVector.x, 0, inputVector.y);
            transform.Translate(movement, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ghost _ghost))
        {
            this._ghost = _ghost;
            _inputManager.MovableObjectActions.Interact.performed += _unpossess;
        }
    }
}