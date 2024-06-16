using FMOD.Studio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatController : PlayerController, IMove, IJump, IStep
{
    #region Parameters

    private Action<InputAction.CallbackContext> _jumpAction;

    private Vector2 input;

    private PlayerState CatState;

    [SerializeField] private ParticleSystem _movingEffect;

    [Header("Movement")]
    [SerializeField] private float Speed = 20f;

    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 4f;

    [SerializeField] private float jumpDuration = 0.1f;
    private int jumpCount = 0;

    [Header("Physics")]
    [SerializeField] private float gravity = 30f;

    [SerializeField] private float maxVelocity = 20f;
    [SerializeField] private float maxForce = 20f;
    private float VelocityNode;

    [Header("CatAnimation")]
    [SerializeField] private float smooth = 5f;

    [SerializeField] private float animationPlayTransition = 0.001f;
    private int JumpAnimationId;
    private int RunAnimationId;
    private float p_anim;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistanceNormal = 0.1f;

    [Header("Steps")]
    [SerializeField] private float startRay = 0.1f;

    [SerializeField] private float rayLength = 1f;
    [SerializeField] private float stepSmooth = 20f;
    [SerializeField] private float stepHeight = 1f;
    [SerializeField] private float maxClimbHeight = 0.5f;

    #endregion Parameters

    [Header("Audio")]
    private EventInstance PlayerFootSteps;

    protected override void Awake()
    {
        base.Awake();
        JumpAnimationId = Animator.StringToHash(GameConstant.CatAnimation.IsJumping);
        RunAnimationId = Animator.StringToHash(GameConstant.CatAnimation.HorizontalMove);
    }

    protected override void Start()
    {
        _jumpAction = _ => Jump();

        base.Start();
        PlayerFootSteps = AudioSystem.CreateEventInstance(FmodSystem.PlayerSteps);

        if (_inputManager == null)
            _inputManager = ServiceLocator.Instance.GetService<InputManager>();
    }

    private void Update()
    {
        input = _inputManager.CatActions.Move.ReadValue<Vector2>().normalized;
        if (input.magnitude > 0.1f)
        {
            Move(input);
        }
        if (_inputManager.CatActions.Jump.triggered)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        LimitVelocity();
    }

    protected override void OnMovePerformed(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>().normalized;
        CatState = PlayerState.moving;

        PlayerFootSteps.getPlaybackState(out PLAYBACK_STATE playbackstate);
        PlayerFootSteps.start();
        MovingEffect();
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        StartCoroutine(StopMoveSmoothly());
        rb.velocity = Vector3.zero;

        PlayerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
        StoppingEffect();
    }

    protected override void OnJumpPerformed(InputAction.CallbackContext context)
    {
        CatState = PlayerState.jumping;
    }

    protected override void OnDashPerformed(InputAction.CallbackContext context)
    {
        // Empty implementation to satisfy the interface requirement
    }

    protected override void OnAscendPerformed(InputAction.CallbackContext context)
    {
        // Empty implementation to satisfy the interface requirement
    }

    protected override void OnAscendCanceled(InputAction.CallbackContext context)
    {
        // Empty implementation to satisfy the interface requirement
    }

    public void Move(Vector2 input)
    {
        if (CatState != PlayerState.moving)
            return;
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (ShouldStep(moveDirection))
            {
                Step(moveDirection);
            }
            else
            {
                Vector3 newVelocity = moveDirection * Speed;
                newVelocity.y = rb.velocity.y;
                rb.velocity = newVelocity;
            }
        }
        Animate(input);
    }

    public bool ShouldStep(Vector3 moveDirection)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = moveDirection;
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength))
        {
            float heightDifference = hit.point.y - transform.position.y;
            Vector3 stepRayOrigin = rayOrigin + Vector3.up * stepHeight;

            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
            {
                if (!Physics.Raycast(stepRayOrigin, rayDirection, out hit, rayLength))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Step(Vector3 moveDirection)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = moveDirection;
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength))
        {
            float heightDifference = hit.point.y - transform.position.y;

            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
            {
                Vector3 normal = hit.normal;
                Vector3 projectedDirection = Vector3.ProjectOnPlane(rayDirection, normal);

                Vector3 stepUpPosition = transform.position + projectedDirection.normalized * Speed * Time.deltaTime;
                stepUpPosition.y = hit.point.y + stepHeight;

                rb.MovePosition(Vector3.Lerp(rb.position, stepUpPosition, stepSmooth * Time.deltaTime));
            }
            else
            {
                rb.AddForce(Vector3.up * gravity);
            }
        }
        else
        {
            rb.AddForce(Vector3.up * gravity);
        }
    }

    public void Jump()
    {
        if (CatState != PlayerState.jumping)
            return;
        CatState = PlayerState.moving;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistanceNormal)) // && jumpTimer <= 0)
        {
            if (hit.distance < groundCheckDistanceNormal)
            {
                Debug.Log(jumpCount);
                CatState = PlayerState.moving;
                jumpCount = 0;
            }
        }
        if (jumpCount < 2)
        {
            VelocityNode = (2 * gravity * jumpHeight) / jumpDuration;
            rb.velocity = new Vector3(rb.velocity.x, VelocityNode, rb.velocity.z);
            _animator.CrossFade(JumpAnimationId, animationPlayTransition);
            jumpCount++;
        }
    }

    public void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    public void LimitVelocity()
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

    private void Animate(Vector2 input)
    {
        if (p_anim < input.x || p_anim >= input.x)
        {
            p_anim += Time.deltaTime * smooth;
        }

        p_anim = Mathf.Clamp(p_anim, 0.0f, 0.25f);
        _animator.SetFloat(RunAnimationId, p_anim);
    }

    private void MovingEffect()
    {
        _movingEffect.Play();
    }
    private void StoppingEffect()
    {
        _movingEffect.Stop();
    }
    private IEnumerator StopMoveSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(1, 0, elapsedTime / duration);
            _animator.SetFloat(RunAnimationId, value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _animator.SetFloat(RunAnimationId, 0);
        p_anim = 0;
    }

    public override void LoadGame(GameData _gameData)
    {
        //transform.position = _gameData._playerPosition;
    }

    public override void SaveGame(GameData _gameData)
    {
        //_gameData._playerPosition = transform.position;
    }
}