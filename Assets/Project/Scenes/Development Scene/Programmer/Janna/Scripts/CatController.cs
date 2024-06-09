using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatController : PlayerController, IMove, IJump, IStep
{
    #region Parameters
    
    private Vector2 input;
    private bool isMoving;
    private bool isJumping;
    private bool isStepping;

    private CatState currentState;

    [Header("Movement")]
    [SerializeField] private float Speed = 15f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpDuration = 0.5f;
    private int jumpCount = 0;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float maxForce = 15f;
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
    [SerializeField] private float startRay = 0.2f;
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private float stepSmooth = 15f;
    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float maxClimbHeight = 0.3f;

    #endregion
    protected override void Awake()
    {
        base.Awake();
        JumpAnimationId = Animator.StringToHash(GameConstant.CatAnimation.IsJumping);
        RunAnimationId = Animator.StringToHash(GameConstant.CatAnimation.HorizontalMove);
        currentState = CatState.Idle;
    }

    private void Update()
    {
        input = _inputManager.CatActions.Move.ReadValue<Vector2>().normalized;

        switch (currentState)
        {
            case CatState.Idle:
                if (isMoving && input != Vector2.zero)
                {
                    currentState = CatState.Moving;
                    Move(input);
                    Step();
                }
                break;
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
        isMoving = true;
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
        StartCoroutine(StopMoveSmoothly());
        rb.velocity = Vector3.zero;
    }

    protected override void OnJumpPerformed(InputAction.CallbackContext context)
    {
        currentState = CatState.Jumping;
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
        currentState = CatState.Moving;
        isMoving = true;
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

            Vector3 newVelocity = moveDirection * Speed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }

        Animate(input);
    }

    public void Jump()
    {
        isJumping = true;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistanceNormal))
        {
            if (hit.distance < groundCheckDistanceNormal)
            {
                jumpCount = 0;
            }
        }
        if (jumpCount < 2)
        {
            VelocityNode = (2 * gravity * jumpHeight) / jumpDuration;
            rb.velocity = new Vector3(rb.velocity.x, VelocityNode, rb.velocity.z);
            _animator.CrossFade(JumpAnimationId, animationPlayTransition);
            jumpCount++;
            Logging.Log("Jumping");
        }
        currentState = CatState.Jumping;
    }

    public void ApplyGravity()
    {
        isStepping = false;
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

    public void Step()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = transform.forward;
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, rayLength);

        bool stepDetected = false;

        foreach (RaycastHit hit in hits)
        {
            float heightDifference = hit.point.y - transform.position.y;
            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
            {
                Vector3 stepUpPosition = new Vector3(transform.position.x, hit.point.y + stepHeight, transform.position.z);
                rb.MovePosition(Vector3.Lerp(rb.position, stepUpPosition, stepSmooth));
                stepDetected = true;
                break;
            }
        }

        if (!stepDetected)
        {
            rb.AddForce(Vector3.up * gravity);
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

    private IEnumerator StopMoveSmoothly()
    {
        isMoving = false;
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
        transform.position = _gameData._playerPosition;
    }

    public override void SaveGame(GameData _gameData)
    {
        _gameData._playerPosition = transform.position;
    }
}
public enum CatState
{
    Idle,
    Moving,
    Jumping
}