using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class GhostController : PlayerController, IMove, IDash, IStep, IAscend
{
    #region Parameters

    [SerializeField] private bool canFly;
    private Vector2 input;
    private bool isAscending = false;
    private bool canDash = true;
    private bool isDashing = false;

    private PlayerState GhostState;

    [Header("VFX")]
    [SerializeField] private VisualEffect dustVFX;

    [Header("BroadCasting")]
    // reference of two events
    [SerializeField] private UnityEvent _onAscend;

    [SerializeField] private UnityEvent _onDescend;

    [Header("Movement")]
    [SerializeField] private float Speed = 10f;

    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 100f;

    [SerializeField] private float dashDistance = 10f;
    private float dashTime;

    [Header("Ascending")]
    [SerializeField] private float ascendingSpeed = 40f;

    [SerializeField] private float gravity = -9.81f;

    private readonly float ghostDistance = 0.1f;
    private float distanceToGround;
    private RaycastHit reachedDistance;

    [Header("Steps")]
    [SerializeField] private float startRay = 0.5f;

    [SerializeField] private float rayLength = 1f;
    [SerializeField] private float stepSmooth = 20f;
    [SerializeField] private float stepHeight = 1f;
    [SerializeField] private float maxClimbHeight = 0.5f;
    [SerializeField] private LayerMask detectWall;

    private readonly int RunAnimationId;
    private RaycastHit ishit;

    #endregion Parameters

    [SerializeField] private EventReference _ghostDash;
    private AudioSystemFMOD _audioSystem;

    public AudioSystemFMOD AudioSystem => _audioSystem;

    protected override void Awake() => base.Awake();

    protected override void Start()
    {
        base.Start();
        _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();
    }

    private void Update()
    {
        input = _inputManager.GhostActions.Move.ReadValue<Vector2>().normalized;
        if (input.magnitude > 0.1f)
            Move(input);

        if (canFly)
        {
            if (isAscending)
            {
                Ascend();
                _onAscend?.Invoke();
            }
            else
            {
                ApplyGravity();
                _onDescend?.Invoke();
            }
        }
        else
        {
            ApplyGravity();
            _onDescend?.Invoke();
        }
    }

    protected override void OnMovePerformed(InputAction.CallbackContext context)
    {
        GhostState = PlayerState.moving;
        input = context.ReadValue<Vector2>().normalized;

        if (dustVFX != null)
        {
            dustVFX.Play();
        }
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        rb.velocity = Vector3.zero;

        if (dustVFX != null)
        {
            dustVFX.Stop();
        }
    }

    protected override void OnDashPerformed(InputAction.CallbackContext context)
    {
        GhostState = PlayerState.Dashing;
        Dash();
    }

    protected override void OnAscendPerformed(InputAction.CallbackContext context)
    {
        if (canFly)
        {
            isAscending = context.ReadValue<float>() > 0.1f;
            isAscending = true;
            GhostState = PlayerState.Ascending;
        }
    }

    protected override void OnAscendCanceled(InputAction.CallbackContext context)
    {
        if (canFly)
        {
            isAscending = context.ReadValue<float>() > 0.1f;
            isAscending = false;
            GhostState = PlayerState.moving;
        }
    }

    protected override void OnJumpPerformed(InputAction.CallbackContext context)
    {
    }

    public void Move(Vector2 input)
    {
        if (GhostState != PlayerState.moving)
            return;

        Vector3 cameraForward = _camera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 cameraRight = _camera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (ShouldStep(moveDirection))
                Step(moveDirection);
            else
            {
                Vector3 newVelocity = moveDirection * Speed;
                newVelocity.y = rb.velocity.y;
                rb.velocity = newVelocity;
            }
        }
        else
            _animator.SetFloat(RunAnimationId, 0);
    }

    public bool ShouldStep(Vector3 moveDirection)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = moveDirection;

        Logging.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            float heightDifference = hit.point.y - transform.position.y;
            Vector3 stepRayOrigin = rayOrigin + Vector3.up * stepHeight;

            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
                if (!Physics.Raycast(stepRayOrigin, rayDirection, out _, rayLength))
                    return true;
        }

        return false;
    }

    public void Step(Vector3 moveDirection)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = moveDirection;

        Logging.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            float heightDifference = hit.point.y - transform.position.y;

            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
            {
                Vector3 normal = hit.normal;
                Vector3 projectedDirection = Vector3.ProjectOnPlane(rayDirection, normal);

                Vector3 stepUpPosition = transform.position + Speed * Time.deltaTime * projectedDirection.normalized;
                stepUpPosition.y = hit.point.y + stepHeight;

                rb.MovePosition(Vector3.Lerp(rb.position, stepUpPosition, stepSmooth * Time.deltaTime));
            }
            else
                rb.AddForce(Vector3.up * gravity);
        }
        else
            rb.AddForce(Vector3.up * gravity);
    }

    public void Dash()
    {
        if (GhostState != PlayerState.Dashing)
            return;

        GhostState = PlayerState.Dashing;

        if (AudioSystem != null)
            AudioSystem.PlayerShooting(_ghostDash, this.gameObject.transform.position);

        if (canDash && !isDashing)
            StartCoroutine(PerformDash());
    }

    public IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (Physics.Raycast(transform.position, transform.forward, out ishit, dashDistance, detectWall))
            SetWallTrigger(ishit.collider, true);

        rb.velocity = transform.forward * dashSpeed;
        dashTime = dashDistance / dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.velocity = Vector3.zero;
        isDashing = false;

        if (ishit.collider != null)
            SetWallTrigger(ishit.collider, false);

        canDash = true;
        GhostState = PlayerState.moving;
    }

    public void SetWallTrigger(Collider wallCollider, bool isTrigger)
    {
        if (wallCollider != null)
            wallCollider.isTrigger = isTrigger;
    }

    public void Ascend()
    {
        if (GhostState != PlayerState.Ascending)
            return;

        rb.velocity = new Vector3(rb.velocity.x, ghostDistance * ascendingSpeed, rb.velocity.z);
    }

    public void ApplyGravity()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out reachedDistance))
        {
            distanceToGround = reachedDistance.distance;

            if (distanceToGround > ghostDistance)
                rb.useGravity = true;
            else
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }
        }
    }
}