using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class GhostController : PlayerController, IMove, IStep, IDash, IAscend
{
    #region Parameters
    private Vector2 input;
    private bool isMoving;
    private bool isAscending = false;
    private bool canDash = true;
    private bool isDashing = false;

    private GhostState currentState;

    [Header("Movement")]
    [SerializeField] private float Speed = 15f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dashDistance = 5f;
    private float dashTime;

    [Header("Ascending")]
    [SerializeField] private float ascendingSpeed =20f;
    private float ghostDistance = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    private float distanceToGround;
    private RaycastHit reachedDistance;

    [Header("Steps")]
    [SerializeField] private float startRay = 0.2f;
    [SerializeField] private float rayLength = 2f;
    [SerializeField] private float stepSmooth = 15f;
    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float maxClimbHeight = 0.3f;

    private RaycastHit ishit;
    [SerializeField] private LayerMask detectWall;

    #endregion
    protected override void Awake()
    {
        base.Awake();
        currentState = GhostState.Idle;
    }

    private void Update()
    {
        input = _inputManager.GhostActions.Move.ReadValue<Vector2>().normalized;

        switch (currentState)
        {
            case GhostState.Idle:
                if (isMoving)
                {
                    currentState = GhostState.Moving;
                }
                break;

            case GhostState.Moving:
                if (input != Vector2.zero)
                {
                    Move(input);
                    Step();
                }
                if (isAscending)
                {
                    currentState = GhostState.Ascending;
                }
                if (isDashing)
                {
                    currentState = GhostState.Dashing;
                }
                break;

            case GhostState.Ascending:
                Ascend();
                if (!isAscending)
                {
                    currentState = GhostState.Moving;
                }
                break;

            case GhostState.Dashing:
                Dash();
                currentState = GhostState.Moving;
                break;
        }

        if (!isAscending && currentState != GhostState.Dashing)
        {
            ApplyGravity();
        }
    }

    protected override void OnMovePerformed(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>().normalized;
        isMoving = true;
        currentState = GhostState.Moving;
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
        rb.velocity = Vector3.zero;
        currentState = GhostState.Idle;
    }

    protected override void OnDashPerformed(InputAction.CallbackContext context)
    {
        isDashing = true;
        Dash();
        currentState = GhostState.Dashing;
    }

    protected override void OnAscendPerformed(InputAction.CallbackContext context)
    {
        isAscending = context.ReadValue<float>() > 0.1f;
        isAscending = true;
        currentState = GhostState.Ascending;
    }

    protected override void OnAscendCanceled(InputAction.CallbackContext context)
    {
        isAscending = context.ReadValue<float>() > 0.1f;
        isAscending = false;
        currentState = GhostState.Moving;
    }

    protected override void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // Empty implementation to satisfy the abstract method requirement
    }

    public void Move(Vector2 input)
    {
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
    public void Dash()
    {
        if (canDash && !isDashing)
        {
            Logging.Log("Dashing");
            StartCoroutine(PerformDash());
        }
    }

    public IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        isMoving = true;

        if (Physics.Raycast(transform.position, transform.forward, out ishit, dashDistance, detectWall))
        {
            SetWallTrigger(ishit.collider, true);
        }

        rb.velocity = transform.forward * dashSpeed;
        dashTime = dashDistance / dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.velocity = Vector3.zero;
        isDashing = false;

        if (ishit.collider != null)
        {
            SetWallTrigger(ishit.collider, false);

            canDash = true;
        }
    }
    public void SetWallTrigger(Collider wallCollider, bool isTrigger)
    {
        if (wallCollider != null)
        {
            wallCollider.isTrigger = isTrigger;
        }
    }



public void Ascend()
    {
        rb.velocity = new Vector3(rb.velocity.x, ghostDistance * ascendingSpeed, rb.velocity.z);
    }
    public void StartAscend()
    {
        Logging.Log("Ascend started");
        isAscending = true;
    }

    public void StopAscend()
    {
        Logging.Log("Ascend stopped");
        isAscending = false;
    }

    public void ApplyGravity()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out reachedDistance))
        {
            distanceToGround = reachedDistance.distance;

            if (distanceToGround > ghostDistance)
            {
                rb.useGravity = true;
            }
            else
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            }
        }
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

public enum GhostState
{
    Idle,
    Moving,
    Dashing,
    Ascending
}
