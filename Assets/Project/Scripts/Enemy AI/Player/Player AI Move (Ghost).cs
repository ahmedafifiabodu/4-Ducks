using KinematicCharacterController;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewMovmentSystemGhost : MonoBehaviour, ICharacterController
{
    #region Parameters

    [Header("Air Movement")]
    [SerializeField] private float _airAccelerationSpeed = 5f;

    [SerializeField] private float _airDecelerationSpeed = 10f;
    [SerializeField] private float _maxAirMoveSpeed = 10f;
    [SerializeField] private float _drag = 0.1f;

    [Header("Floating")]
    [SerializeField] private float _floatingHeight = 1f;

    [Header("Misc")]
    [SerializeField] private Vector3 Gravity = new(0, 0, 0);

    [SerializeField] private Transform MeshRoot;

    private Animator _animator;
    private KinematicCharacterMotor _motor;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;
    private Action<InputAction.CallbackContext> _startFlyAction;
    private Action<InputAction.CallbackContext> _stopFlyAction;

    private bool _isMoving;
    private bool _isFlying;
    private int _runAnimationId;

    private Vector2 _inputVector;
    private Vector3 _moveInputVector;

    #endregion Parameters

    private void Awake()
    {
        _motor = GetComponent<KinematicCharacterMotor>();
        _animator = GetComponent<Animator>();

        _runAnimationId = Animator.StringToHash(GameConstant.Animation.HorizontalMove);
    }

    private void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startMoveAction = _ =>
        {
            _isMoving = true;
        };

        _stopMoveAction = _ =>
        {
            _isMoving = false;
            _moveInputVector = Vector3.zero;
        };

        _startFlyAction = _ =>
        {
            _isFlying = true;
        };

        _stopFlyAction = _ =>
        {
            _isFlying = false;
        };

        _inputManager.GhostActions.Move.started += _startMoveAction;
        _inputManager.GhostActions.Move.canceled += _stopMoveAction;
        _inputManager.GhostActions.Fly.started += _startFlyAction;
        _inputManager.GhostActions.Fly.canceled += _stopFlyAction;
    }

    private void Start() => _motor.CharacterController = this;

    private void OnDisable()
    {
        _inputManager.GhostActions.Move.started -= _startMoveAction;
        _inputManager.GhostActions.Move.canceled -= _stopMoveAction;
        _inputManager.GhostActions.Fly.started -= _startFlyAction;
        _inputManager.GhostActions.Fly.canceled -= _stopFlyAction;
    }

    private void Update()
    {
        if (_isMoving)
        {
            _inputVector = _inputManager.GhostActions.Move.ReadValue<Vector2>();
            _moveInputVector = new Vector3(_inputVector.x, 0, _inputVector.y);
        }
        else
        {
            _inputVector = Vector2.Lerp(_inputVector, Vector2.zero, Time.deltaTime * 10);

            if (_inputVector.magnitude < 0.01f)
                _inputVector = Vector2.zero;
        }

        _animator.SetFloat(_runAnimationId, _inputVector.magnitude);
    }

    #region ICharacterController

    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        if (_isFlying)
        {
            Vector3 pos = transform.position;
            pos.y += _floatingHeight * deltaTime;
            _motor.SetPosition(pos);
        }

        // Calculate the distance to the ground
        if (Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out RaycastHit hit))
        {
            float distanceToGround = hit.distance;
            float desiredHeight = _floatingHeight;

            Vector3 pos = transform.position;

            if (distanceToGround < desiredHeight)
            {
                // If the player is below the desired height, move the player up
                pos.y = Mathf.MoveTowards(pos.y, pos.y + (desiredHeight - distanceToGround), deltaTime * 10f);
                _motor.SetPosition(pos);
            }
            else if (distanceToGround > desiredHeight)
            {
                // If the player is above the desired height, move the player down
                pos.y = Mathf.MoveTowards(pos.y, pos.y - (distanceToGround - desiredHeight), deltaTime * 2f);
                _motor.SetPosition(pos);
            }
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_moveInputVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveInputVector, Vector3.up);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, 10 * deltaTime);
        }
    }

    public void UpdateVelocity(ref Vector3 _currentVelocity, float _deltaTime)
    {
        Vector3 _targetMovementVelocity;

        if (_moveInputVector.sqrMagnitude > 0f)
        {
            _targetMovementVelocity = _moveInputVector * _maxAirMoveSpeed;

            Vector3 velocityDiff = Vector3.ProjectOnPlane(_targetMovementVelocity - _currentVelocity, Gravity);
            _currentVelocity += _airAccelerationSpeed * _deltaTime * velocityDiff;
        }
        else
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, _airDecelerationSpeed * _deltaTime);

        // Gravity
        _currentVelocity += _deltaTime * Gravity;

        // Air resistance
        _currentVelocity *= (1f - _drag * _deltaTime);
    }

    #endregion ICharacterController
}