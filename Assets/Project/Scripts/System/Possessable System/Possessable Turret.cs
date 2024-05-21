using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessableTurret : MonoBehaviour, IPossessable
{
    [SerializeField] private float rotationSpeed = 5f; // Adjust this value to control the speed of rotation
    [SerializeField] private ThrowingBall _throwingBall;
    [SerializeField] private Vector2 _angleLimitForRotation;

    private Vector2 _inputVector;

    private Ghost _ghost;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _unpossess;

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
    }

    private void OnEnable()
    {
        _unpossess = _ => Unpossess();

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.TurretActions.Disable();
    }

    private void OnDisable()
    {
        _inputManager.TurretActions.Disable();
    }

    public void Possess()
    {
        _ghost.gameObject.SetActive(false);

        _inputManager.GhostActions.Disable();
        _inputManager.TurretActions.Enable();
    }

    public void Unpossess()
    {
        _ghost.gameObject.SetActive(true);

        _inputManager.TurretActions.Interact.performed -= _unpossess;
        _inputManager.TurretActions.Look.performed -= ctx => { _inputVector = Vector2.zero; };
        _inputManager.TurretActions.Look.canceled -= ctx => { _inputVector = Vector2.zero; };

        _inputManager.GhostActions.Enable();
        _inputManager.TurretActions.Disable();
    }

    private Vector3 currentRotation;

    private void Update()
    {
        if (_inputVector != Vector2.zero)
        {
            // Calculate the rotation amount based on the input vector and the rotation speed
            Vector3 rotationAmount = rotationSpeed * Time.deltaTime * new Vector3(_inputVector.y, _inputVector.x, 0);

            // Add the rotation amount to the current rotation
            currentRotation += rotationAmount;

            // Clamp the rotation angles
            currentRotation.x = Mathf.Clamp(currentRotation.x, _angleLimitForRotation.x, _angleLimitForRotation.y);
            currentRotation.y = Mathf.Clamp(currentRotation.y, _angleLimitForRotation.x, _angleLimitForRotation.y);

            // Apply the rotation to the turret
            transform.rotation = Quaternion.Euler(currentRotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ghost _ghost))
        {
            this._ghost = _ghost;

            _inputManager.TurretActions.Interact.performed += _unpossess;
            _inputManager.TurretActions.Look.performed += ctx => { _inputVector = ctx.ReadValue<Vector2>(); };
            _inputManager.TurretActions.Look.canceled += ctx => { _inputVector = Vector2.zero; };
        }
    }
}