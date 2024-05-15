using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingMechanism : MonoBehaviour
{
    #region Parameters

    [SerializeField] private GameObject _ballObj;
    [SerializeField] private float _baseVelocity = 10f;
    [SerializeField] protected float _velocityMultiplier = 1.5f;

    [SerializeField] protected bool IsCat;
    [SerializeField] protected bool IsGhost;

    protected float _currentVelocity;

    private InputManager _inputManager;
    private Coroutine _throwCoroutine;

    private Action<InputAction.CallbackContext> _startThrowAction;
    private Action<InputAction.CallbackContext> _endThrowAction;

    #endregion Parameters

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startThrowAction = _ => _throwCoroutine = StartCoroutine(StartThrow());
        _endThrowAction = _ =>
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
        else if (IsGhost)
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
        GameObject bullet = ObjectPool.SharedInstance.GetPooledObject(1);

        if (bullet != null)
        {
            bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            bullet.SetActive(true);

            Vector3 initialVelocity = transform.up * _currentVelocity + transform.forward * _currentVelocity;
            Rigidbody ballRigidbody = bullet.GetComponent<Rigidbody>();
            ballRigidbody.velocity = initialVelocity;
        }

        /*//the initial velocity of the ball
        Vector3 initialVelocity = new Vector3(0, _currentVelocity, _currentVelocity);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.velocity = initialVelocity;*/
    }
}