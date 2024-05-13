using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingBall : MonoBehaviour
{
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startThrowAction;
    private Action<InputAction.CallbackContext> _endThrowAction;

    [SerializeField] private GameObject _ballObj;
    [SerializeField] private float _baseVelocity = 10f;
    [SerializeField] private float _velocityMultiplier = 1.5f;
    private float _currentVelocity;


    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startThrowAction = ctx => StartCoroutine(StartThrow());
        _endThrowAction = ctx => Throw();

        _inputManager.PlayerActions.Throw.started += _startThrowAction;
        _inputManager.PlayerActions.Throw.canceled += _endThrowAction;
    }

    private void OnDisable()
    {
        _inputManager.PlayerActions.Throw.started -= _startThrowAction;
        _inputManager.PlayerActions.Throw.canceled -= _endThrowAction;
    }

    private IEnumerator StartThrow()
    {
        _currentVelocity = _baseVelocity;

        while (true)
        {
            _currentVelocity += _velocityMultiplier * Time.deltaTime;
            yield return null;
        }
    }

    private void Throw()
    {
        GameObject ball = Instantiate(_ballObj, transform.position, transform.rotation);

        //the initial velocity of the ball
        float velocityX = _currentVelocity * Mathf.Cos(45);
        float velocityY = _currentVelocity * Mathf.Sin(45);

        //the initial velocity of the ball
        Vector3 initialVelocity = new Vector3(velocityX, velocityY, 0);
        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
        ballRigidbody.velocity = initialVelocity;

        Logging.Log("Throwing with velocity: " + _currentVelocity);
    }
}
