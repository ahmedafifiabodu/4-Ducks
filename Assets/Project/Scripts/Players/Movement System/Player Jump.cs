using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    private InputManager _inputManager;
    private Action<InputAction.CallbackContext> _jumpAction;

    private Rigidbody rb;
    public RaycastHit hit;

    [Header("Jump")]
    // [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 1f;
    private bool canJump = true;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float maxForce = 20f;
    private float VelocityNode;

    [Header("CatAnimation")]
    [SerializeField] private float animationPlayTransition = 0.001f;
    private Animator _animator;
    private int JumpAnimationId;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistanceNormal = 1f;
    private float checkDistance;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        JumpAnimationId = Animator.StringToHash(GameConstant.CatAnimation.IsJumping);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _jumpAction = _ => Jump();
        _inputManager.CatActions.Jump.performed += _jumpAction;
    }

    private void OnDisable() => _inputManager.CatActions.Jump.performed -= _jumpAction;

    private void FixedUpdate()
    {
        ApplyGravity();
        LimitVelocity();
        GroundDetection();
    }

    private void Jump()
    {
        if (canJump)
        {
            VelocityNode = (2 * gravity * jumpHeight) / jumpDuration;
            rb.velocity = new Vector3(rb.velocity.x, VelocityNode, rb.velocity.z);
            _animator.CrossFade(JumpAnimationId, animationPlayTransition);
            canJump = false;
            Logging.Log("Jumping");
        }
    }

    private void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    private void LimitVelocity()
    {
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
        if (rb.velocity.magnitude > maxForce)
        {
            rb.velocity = rb.velocity.normalized * maxForce;
        }
    }

    private void GroundDetection()
    {
        checkDistance = groundCheckDistanceNormal;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance))
        {
            if (hit.distance < checkDistance)
            {
                canJump = true;
            }
        }
    }

    public void LoadGame(GameData _gameData)
    {
        transform.position = _gameData._playerPosition;
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData._playerPosition = transform.position;
    }
}
