using KinematicCharacterController;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNewMovmentSystemCat : MonoBehaviour, ICharacterController
{
    #region Parameters

    [Header("Movement")]
    [SerializeField] private float _maxMovementSpeed = 10f;

    [SerializeField] private float _speedAcceleration = 5f;
    [SerializeField] private float _speedDeceleration = 10f;

    [Header("Air Movement")]
    [SerializeField] private float _maxAirMovementSpeed = 10f;

    [SerializeField] private float _airAccelerationSpeed = 5f;
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

    private Animator _animator;
    private KinematicCharacterMotor _motor;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;
    private Action<InputAction.CallbackContext> _jumpAction;

    private int RunAnimationID;
    private int JumpAnimationID;
    private int OnGroundAnimationID;
    private int IsFallingAnimationID;

    private bool isMoving;
    private Vector2 _inputVector;
    private Vector3 _moveInputVector;

    private bool _jumpRequested = false;
    private bool _jumpConsumed = false;
    private float _timeSinceJumpRequested = Mathf.Infinity;
    private float _timeSinceLastAbleToJump = 0f;

    private bool _doubleJumpAllowed = false;
    private bool _doubleJumpConsumed = false;

    #endregion Parameters

    private void Awake()
    {
        _motor = GetComponent<KinematicCharacterMotor>();
        _animator = GetComponent<Animator>();

        RunAnimationID = Animator.StringToHash(GameConstant.Animation.HorizontalMove);
        JumpAnimationID = Animator.StringToHash(GameConstant.AnimationTest.CatJumping);
        OnGroundAnimationID = Animator.StringToHash(GameConstant.AnimationTest.CatOnGround);
        IsFallingAnimationID = Animator.StringToHash(GameConstant.AnimationTest.CatFalling);
    }

    private void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startMoveAction = _ =>
        {
            isMoving = true;
        };

        _stopMoveAction = _ =>
        {
            isMoving = false;
            _moveInputVector = Vector3.zero;
        };

        _jumpAction = _ =>
        {
            _jumpRequested = true;
            _timeSinceJumpRequested = 0f;
        };

        _inputManager.PlayerActions.Move.started += _startMoveAction;
        _inputManager.PlayerActions.Move.canceled += _stopMoveAction;
        _inputManager.PlayerActions.Jump.started += _jumpAction;
    }

    private void Start() => _motor.CharacterController = this;

    private void OnDisable()
    {
        _inputManager.PlayerActions.Move.started -= _startMoveAction;
        _inputManager.PlayerActions.Move.canceled -= _stopMoveAction;
    }

    private void Update()
    {
        if (isMoving)
        {
            _inputVector = _inputManager.PlayerActions.Move.ReadValue<Vector2>();
            _moveInputVector = new Vector3(_inputVector.x, 0, _inputVector.y);
        }
        else
        {
            _inputVector = Vector2.Lerp(_inputVector, Vector2.zero, Time.deltaTime * 10);

            if (_inputVector.magnitude < 0.01f)
                _inputVector = Vector2.zero;
        }

        _animator.SetFloat(RunAnimationID, _inputVector.magnitude);
    }

    #region ICharacterController

    public void AfterCharacterUpdate(float deltaTime)
    {
        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
            _jumpRequested = false;

        if (_motor.GroundingStatus.IsStableOnGround)
        {
            _jumpConsumed = false;
            _doubleJumpAllowed = true;
            _doubleJumpConsumed = false;
            _timeSinceLastAbleToJump = 0f;

            _animator.SetBool(OnGroundAnimationID, true);
            _animator.SetBool(IsFallingAnimationID, false);
            _animator.SetBool(JumpAnimationID, false);
        }
        else
        {
            _timeSinceLastAbleToJump += deltaTime;
        }

        _timeSinceLastAbleToJump += deltaTime;
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
        if (_moveInputVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_moveInputVector, Vector3.up);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, 10 * deltaTime);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        Vector3 targetMovementVelocity;

        if (_motor.GroundingStatus.IsStableOnGround)
        {
            Vector3 inputDirection = new Vector3(_moveInputVector.x, 0, _moveInputVector.z).normalized;
            targetMovementVelocity = inputDirection * _maxMovementSpeed;

            float speedChange = isMoving ? _speedAcceleration : _speedDeceleration;

            currentVelocity = Vector3.MoveTowards(currentVelocity, targetMovementVelocity, speedChange * deltaTime);

            _animator.SetBool(OnGroundAnimationID, true);
            _animator.SetBool(IsFallingAnimationID, false);
            _animator.SetBool(JumpAnimationID, false);
        }
        else
        {
            if (_moveInputVector.sqrMagnitude > 0f)
            {
                targetMovementVelocity = _moveInputVector * _maxAirMovementSpeed;

                // Prevent climbing on un-stable slopes with air movement
                if (_motor.GroundingStatus.FoundAnyGround)
                {
                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(_motor.CharacterUp, _motor.GroundingStatus.GroundNormal), _motor.CharacterUp).normalized;
                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                }

                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                currentVelocity += _airAccelerationSpeed * deltaTime * velocityDiff;
            }

            // Gravity
            if (currentVelocity.y > 0)
                currentVelocity += _upwardsGravityMultiplier * deltaTime * Gravity;
            else
                currentVelocity += _downwardsGravityMultiplier * deltaTime * Gravity;

            // _drag
            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
        }

        _timeSinceJumpRequested += deltaTime;

        if (_jumpRequested)
        {
            if (!_jumpConsumed && (_motor.GroundingStatus.IsStableOnGround || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
            {
                Vector3 jumpDirection = _motor.CharacterUp;

                if (_motor.GroundingStatus.FoundAnyGround && !_motor.GroundingStatus.IsStableOnGround)
                    jumpDirection = _motor.GroundingStatus.GroundNormal;

                _motor.ForceUnground(0.1f);

                currentVelocity += (jumpDirection * _jumpForce) - Vector3.Project(currentVelocity, _motor.CharacterUp);

                _jumpRequested = false;
                _jumpConsumed = true;

                _animator.SetBool(JumpAnimationID, true); // Add this line
                _animator.SetBool(IsFallingAnimationID, false); // Add this line
            }

            if (!_jumpConsumed && !_doubleJumpConsumed && _doubleJumpAllowed && AllowDoubleJump)
            {
                Vector3 jumpDirection = _motor.CharacterUp;

                _motor.ForceUnground(0.1f);

                currentVelocity += (jumpDirection * _doubleJumpForce) - Vector3.Project(currentVelocity, _motor.CharacterUp);

                _jumpRequested = false;
                _doubleJumpConsumed = true;

                _animator.SetBool(JumpAnimationID, true); // Add this line
                _animator.SetBool(IsFallingAnimationID, false); // Add this line
            }
        }
        else if (!_motor.GroundingStatus.IsStableOnGround)
        {
            _animator.SetBool(IsFallingAnimationID, true);
        }
    }

    #endregion ICharacterController
}