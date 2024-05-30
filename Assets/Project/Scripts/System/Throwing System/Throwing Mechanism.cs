using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowingMechanism : MonoBehaviour
{
    // Serialized fields that can be set in the Unity editor
    [Header("Throwing Mechanism")]
    [SerializeField] private GameObject _ballPrefab; // Prefab for the ball

    [SerializeField] protected float _baseVelocity = 2f; // Base velocity for the throw
    [SerializeField] protected float _velocityMultiplier = 2f; // Velocity multiplier for the throw

    [Header("Trajectory Line Renderer")]
    [SerializeField] protected LineRenderer trajectoryLineRenderer; // Line renderer for the trajectory

    [SerializeField] protected int numPoints = 10; // Number of points in the trajectory
    [SerializeField] protected float timeBetweenPoints = 0.1f; // Time between points in the trajectory
    [SerializeField] protected float pointIncreaseInterval = 0.2f; // Interval to increase the points in the trajectory

    private ObjectPool _objectPool; // Reference to the ObjectPool
    protected InputManager _inputManager; // Reference to the InputManager
    private Coroutine _throwCoroutine; // Coroutine for the throw

    protected Action<InputAction.CallbackContext> _startThrowAction; // Action to start the throw
    protected Action<InputAction.CallbackContext> _endThrowAction; // Action to end the throw

    protected Vector3 initialVelocity; // Initial velocity for the throw
    private float _holdTime = 0f; // Time the throw button is held
    protected float _currentVelocity; // Current velocity for the throw
    internal bool _checkingPlayerInput = false; // Flag to check if the player input is being checked
    private bool _isFireKeyPressed = false; // Flag to check if the fire key is being pressed

    protected Vector3 startingVelocity; // Starting velocity for the throw

    // Called when the object is enabled
    protected virtual void OnEnable()
    {
        // Get the InputManager and ObjectPool from the ServiceLocator
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        // Initialize the actions
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

    // Called every frame
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

    // Called when the object is disabled
    protected virtual void OnDisable()
    {
        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        if (_throwCoroutine != null)
            StopCoroutine(_throwCoroutine);
    }

    // Start the throw
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

    // Draw the trajectory
    protected virtual void DrawTrajectory(int numP)
    {
    }

    // Throw the ball
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