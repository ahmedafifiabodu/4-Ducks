using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingMechanism : MonoBehaviour
{
    #region Parameters

    [Header("Throwing Mechanism")]
    [SerializeField] private GameObject _ballPrefab;

    [SerializeField] protected float _baseVelocity = 2f;
    [SerializeField] protected float _velocityMultiplier = 2f;

    [Header("Trajectory Line Renderer")]
    [SerializeField] protected LineRenderer trajectoryLineRenderer;

    [SerializeField] protected int numPoints = 10;
    [SerializeField] protected float timeBetweenPoints = 0.1f; // Time between points
    [SerializeField] protected float pointIncreaseInterval = 0.2f;

    private ObjectPool _objectPool;
    protected InputManager _inputManager;
    private Coroutine _throwCoroutine;

    protected Action<InputAction.CallbackContext> _startThrowAction;
    protected Action<InputAction.CallbackContext> _endThrowAction;

    protected Vector3 initialVelocity;
    private float _holdTime = 0f;
    protected float _currentVelocity;
    internal bool _checkingPlayerInput = false;
    private bool _isFireKeyPressed = false; //check if the fire key is being pressed

    protected Vector3 startingVelocity;

    #endregion Parameters

    protected virtual void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        _startThrowAction = context =>
        {
            _isFireKeyPressed = true;
            _holdTime = 0f;
        };

        _endThrowAction = context =>
        {
            _isFireKeyPressed = false;

            if (_throwCoroutine != null)
            {
                StopCoroutine(_throwCoroutine);
                _throwCoroutine = null;
            }

            if (_holdTime <= 1.0f)
            {
                _currentVelocity = _baseVelocity * 5;
                _checkingPlayerInput = true;
            }

            Throw();
        };
    }

    protected virtual void Update()
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

    protected virtual void OnDisable()
    {
        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        if (_throwCoroutine != null)
            StopCoroutine(_throwCoroutine);
    }

    protected virtual IEnumerator StartThrow()
    {
        _currentVelocity = _baseVelocity;

        trajectoryLineRenderer.enabled = true;
        trajectoryLineRenderer.positionCount = numPoints;

        float timer = 0f;

        while (true)
        {
            _currentVelocity += _velocityMultiplier * Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= pointIncreaseInterval && numPoints < 50 && _currentVelocity >= 4.5)
            {
                numPoints++;
                timer = 0f;
            }

            DrawTrajectory(numPoints);
            yield return null;
        }
    }

    protected virtual void DrawTrajectory(int numP)
    {
    }

    protected virtual void Throw()
    {
        GameObject bullet = _objectPool.GetPooledObject(_ballPrefab);

        if (bullet != null)
        {
            bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            bullet.SetActive(true);

            Rigidbody ballRigidbody = bullet.GetComponent<Rigidbody>();
            ballRigidbody.velocity = initialVelocity;
            _checkingPlayerInput = false;
        }

        _currentVelocity = 0;
        trajectoryLineRenderer.enabled = false;
        numPoints = 10;
    }
}