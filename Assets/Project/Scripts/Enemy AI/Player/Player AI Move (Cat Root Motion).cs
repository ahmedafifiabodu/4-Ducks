using KinematicCharacterController;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewMovmentSystemRootMotion : MonoBehaviour, ICharacterController
{
    #region Parameters

    [Header("Stable Movement")]
    [SerializeField] private float MaxStableMoveSpeed = 10f;

    [SerializeField] private float Acceleration = 5f;
    [SerializeField] private float Deceleration = 10f;

    [Header("Air Movement")]
    [SerializeField] private float MaxAirMoveSpeed = 10f;

    [SerializeField] private float AirAccelerationSpeed = 5f;
    [SerializeField] private float Drag = 0.1f;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce = 10f;

    [SerializeField] private float _upwardsGravityMultiplier = 0.5f;
    [SerializeField] private float _downwardsGravityMultiplier = 1.5f;
    [SerializeField] private float JumpPreGroundingGraceTime = 0f;
    [SerializeField] private float JumpPostGroundingGraceTime = 0f;

    [Header("Double Jumping")]
    [SerializeField] private bool AllowDoubleJump = true;

    [SerializeField] private float _doubleJumpForce = 10f;

    [Header("Misc")]
    [SerializeField] private Vector3 Gravity = new(0, -30f, 0);

    [SerializeField] private Transform MeshRoot;
    [SerializeField] private float _animationSmooth = 2;

    private Animator _animator;
    private KinematicCharacterMotor _motor;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;
    private Action<InputAction.CallbackContext> _jumpAction;

    #region Animation Parameters

    private int RunAnimationId;

    private Vector3 _rootMotionPositionDelta;
    private Quaternion _rootMotionRotationDelta;

    #endregion Animation Parameters

    #region Movement Parameters

    private bool isMoving;
    private Vector3 _moveInputVector;

    private float _forwardAxis;
    private float _rightAxis;

    #endregion Movement Parameters

    #region Jumping

    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;

    private bool _doubleJumpAllowed = false;
    private bool _doubleJumpConsumed = false;

    #endregion Jumping

    #endregion Parameters

    private void Awake()
    {
        _motor = GetComponent<KinematicCharacterMotor>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.CatAnimation.HorizontalMove);

        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;
    }

    private void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;

        #region Input Actions

        _startMoveAction = _ =>
        {
            isMoving = true;
        };

        _stopMoveAction = _ =>
        {
            isMoving = false;
            _moveInputVector = Vector3.zero;
            //StartCoroutine(StopMoveSmoothly());
        };

        _jumpAction = _ =>
        {
            _jumpRequested = true;
            _timeSinceJumpRequested = 0f;
        };

        _inputManager.CatActions.Move.started += _startMoveAction;
        _inputManager.CatActions.Move.canceled += _stopMoveAction;
        _inputManager.CatActions.Jump.started += _jumpAction;

        #endregion Input Actions
    }

    private void Start() => _motor.CharacterController = this;

    private void OnDisable()
    {
        _inputManager.CatActions.Move.started -= _startMoveAction;
        _inputManager.CatActions.Move.canceled -= _stopMoveAction;
    }

    private void Update()
    {
        Vector2 inputVector = _inputManager.CatActions.Move.ReadValue<Vector2>();

        _forwardAxis = inputVector.y; // Use the y component for forward/backward movement
        _rightAxis = inputVector.x; // Use the x component for left/right movement

        _animator.SetFloat(RunAnimationId, inputVector.magnitude);
    }

    private void OnAnimatorMove()
    {
        Logging.Log("OnAnimatorMove called, deltaPosition: " + _animator.deltaPosition);
        _rootMotionPositionDelta += _animator.deltaPosition;
        _rootMotionRotationDelta = _animator.deltaRotation * _rootMotionRotationDelta;
    }

    #region ICharacterController

    public void AfterCharacterUpdate(float deltaTime)
    {
        _rootMotionPositionDelta = Vector3.zero;
        _rootMotionRotationDelta = Quaternion.identity;

        #region Jumping

        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
            _jumpRequested = false;

        if (_motor.GroundingStatus.IsStableOnGround)
        {
            _jumpConsumed = false;
            _doubleJumpAllowed = true;
            _doubleJumpConsumed = false;
            _timeSinceLastAbleToJump = 0f;
        }
        else
            _timeSinceLastAbleToJump += deltaTime;

        _timeSinceLastAbleToJump += deltaTime;

        #endregion Jumping
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
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
        currentRotation = _rootMotionRotationDelta * currentRotation;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            if (deltaTime > 0)
            {
                // The final velocity is the velocity from root motion reoriented on the ground plane
                currentVelocity = _rootMotionPositionDelta / deltaTime;
                currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, _motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;
            }
            else
            {
                // Prevent division by zero
                currentVelocity = Vector3.zero;
            }
        }
        else
        {
            if (_forwardAxis > 0f)
            {
                // If we want to move, add an acceleration to the velocity
                Vector3 targetMovementVelocity = _motor.CharacterForward * _forwardAxis * MaxAirMoveSpeed;
                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                currentVelocity += AirAccelerationSpeed * deltaTime * velocityDiff;
            }

            // Gravity
            currentVelocity += Gravity * deltaTime;

            // Drag
            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
        }
    }

    #endregion ICharacterController
}