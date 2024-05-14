using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingMechanism : MonoBehaviour
{
    private InputManager _inputManager;
    private Coroutine _throwCoroutine;

    private Action<InputAction.CallbackContext> _startThrowAction;
    private Action<InputAction.CallbackContext> _endThrowAction;

    [SerializeField] private GameObject _ballObj;
    [SerializeField] private float _baseVelocity = 10f;
    [SerializeField] protected float _velocityMultiplier = 1.5f;
    protected float _currentVelocity;

    [SerializeField] protected bool IsCat;
    [SerializeField] protected bool IsGhost;

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startThrowAction = ctx => _throwCoroutine = StartCoroutine(StartThrow());
        _endThrowAction = ctx =>
        {
            if (_throwCoroutine != null)
            {
                StopCoroutine(_throwCoroutine);
                Throw();
            }

        };

        if (IsCat)
        {
            _inputManager.PlayerActions.Throw.started += _startThrowAction;
            _inputManager.PlayerActions.Throw.canceled += _endThrowAction;
        }
        else if(IsGhost)
        {
            _inputManager.GhostActions.Throw.started += _startThrowAction;
            _inputManager.GhostActions.Throw.canceled += _endThrowAction;
        }

    }


    private void OnDisable()
    {
        if (IsCat)
        {
            _inputManager.PlayerActions.Throw.started -= _startThrowAction;
            _inputManager.PlayerActions.Throw.canceled -= _endThrowAction;
        }
        else if (IsGhost)
        {
            _inputManager.GhostActions.Throw.started -= _startThrowAction;
            _inputManager.GhostActions.Throw.canceled -= _endThrowAction;
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
        GameObject ball = Instantiate(_ballObj, transform.position, transform.rotation);

        //the initial velocity of the ball
        Vector3 initialVelocity = new Vector3(0, _currentVelocity, _currentVelocity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.velocity = initialVelocity;

        Logging.Log("Throwing with velocity: " + _currentVelocity);
    }

}
