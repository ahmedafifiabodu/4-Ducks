using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingMechanism : MonoBehaviour
{
    #region Parameters

    [Header("Throwing Mechanism")]
    [SerializeField] private GameObject _ballPrefab;

    [SerializeField] private float _baseVelocity = 10f;
    [SerializeField] protected float _velocityMultiplier = 1.5f;

    [Header("Player Type")]
    [SerializeField] protected bool IsCat;

    [SerializeField] protected bool IsTurret;

    protected float _currentVelocity;

    private ObjectPool _objectPool;
    private InputManager _inputManager;
    private Coroutine _throwCoroutine;

    private Action<InputAction.CallbackContext> _startThrowAction;
    private Action<InputAction.CallbackContext> _startThrow;
    private Action<InputAction.CallbackContext> _endThrowAction;

    #endregion Parameters

    private bool _checkingPlayerInput = false;
    private float _holdTime = 0f; // Timer to track how long the fire key is held
    private bool _isFireKeyPressed = false; // Flag to check if the fire key is being pressed

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        _startThrowAction = context =>
        {
            if (context.phase == InputActionPhase.Started)
            {
                _isFireKeyPressed = true;
                _holdTime = 0f;
            }
        };

        _endThrowAction = context =>
        {
            _isFireKeyPressed = false;
            Logging.Log("Throw");
            if (_throwCoroutine != null)
            {
                StopCoroutine(_throwCoroutine);
                _throwCoroutine = null;
            }

            if (_holdTime <= 1.0f)
            {
                _currentVelocity = _baseVelocity * 15;
                _checkingPlayerInput = true;
            }
            Throw();
        };

        if (IsCat)
        {
            _inputManager.PlayerActions.Throw.started += _startThrowAction;
            _inputManager.PlayerActions.Throw.canceled += _endThrowAction;
        }
        else if (IsTurret)
        {
            _inputManager.TurretActions.Fire.started += _startThrowAction;
            _inputManager.TurretActions.Fire.canceled += _endThrowAction;

            //_inputManager.TurretActions.FireAndHold.canceled += _startThrow;

            //_inputManager.TurretActions.Disable();
        }
    }

    private void Update()
    {
        if (_isFireKeyPressed)
        {
            _holdTime += Time.deltaTime;
            if (_holdTime > 1f)
            {
                _isFireKeyPressed = false;
                _throwCoroutine = StartCoroutine(StartThrow());
            }
        }
    }


    private void OnDisable()
    {
        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        if (IsCat)
        {
            _inputManager.PlayerActions.Throw.started -= _startThrowAction;
            _inputManager.PlayerActions.Throw.canceled -= _endThrowAction;
        }
        else if (IsTurret)
        {
            _inputManager.TurretActions.Fire.started -= _startThrowAction;
            _inputManager.TurretActions.Fire.canceled -= _endThrowAction;
            //_inputManager.TurretActions.FireAndHold.canceled -= _startThrow;

            _checkingPlayerInput = false;
        }

        if (_throwCoroutine != null)
            StopCoroutine(_throwCoroutine);
    }

    protected virtual IEnumerator StartThrow()
    {
        _currentVelocity = _baseVelocity;
        yield return null;
    }

    protected virtual void Throw()
    {
        GameObject bullet = _objectPool.GetPooledObject(_ballPrefab);

        if (bullet != null)
        {
            bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            bullet.SetActive(true);
            Vector3 initialVelocity;

            if (_checkingPlayerInput)
                initialVelocity = transform.forward * _currentVelocity;
            else
                initialVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;

            Rigidbody ballRigidbody = bullet.GetComponent<Rigidbody>();
            ballRigidbody.velocity = initialVelocity;
            _checkingPlayerInput = false;
        }

        _currentVelocity = 0;
    }
}