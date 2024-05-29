using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessableTurret : MonoBehaviour, IPossessable
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private Vector2 _angleLimitForRotation;
    [SerializeField] private TurretThrowing _turretThrowing;

    [Header("Unpossess")]
    [SerializeField] private float _delayAfterUnpossess = 3f;

    [SerializeField] private Collider _collider;

    private bool _isPossessed = false;
    private Vector2 _inputVector;
    private Vector3 currentRotation;

    private InputManager _inputManager;
    private Action<InputAction.CallbackContext> _unpossess;

    private WaitForSeconds waitForSeconds;

    GameObject IPossessable.GhostPlayer { get; set; }

    private void Awake()
    {
        if (_turretThrowing == null)
            _turretThrowing = GetComponentInChildren<TurretThrowing>();

        if (_collider == null)
            _collider = GetComponent<Collider>();

        waitForSeconds = new WaitForSeconds(_delayAfterUnpossess);

        _unpossess = _ => Unpossess();
    }

    private void Start() => _inputManager = ServiceLocator.Instance.GetService<InputManager>();

    private void OnDisable()
    {
        if (_turretThrowing != null)
            _turretThrowing.enabled = false;

        if (_inputManager != null)
            _inputManager.PossessTurretActions.Disable();
    }

    public void Possess()
    {
        _isPossessed = true;
        _turretThrowing.enabled = true;
        ((IPossessable)this).GhostPlayer.SetActive(false);

        _inputManager.PossessTurretActions.Unpossess.performed += _unpossess;
        _inputManager.PossessTurretActions.Look.performed += ctx => { _inputVector = ctx.ReadValue<Vector2>(); };
        _inputManager.PossessTurretActions.Look.canceled += _ => { _inputVector = Vector2.zero; };

        _inputManager.GhostActions.Disable();
        _inputManager.PossessTurretActions.Enable();
    }

    public void Unpossess()
    {
        _isPossessed = false;
        _turretThrowing.enabled = false;
        ((IPossessable)this).GhostPlayer.SetActive(true);

        _inputManager.PossessTurretActions.Unpossess.performed -= _unpossess;
        _inputManager.PossessTurretActions.Look.performed -= ctx => { _inputVector = ctx.ReadValue<Vector2>(); };
        _inputManager.PossessTurretActions.Look.canceled -= _ => { _inputVector = Vector2.zero; };

        _inputManager.GhostActions.Enable();
        _inputManager.PossessTurretActions.Disable();

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
        if (_isPossessed && _inputVector != Vector2.zero)
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
}