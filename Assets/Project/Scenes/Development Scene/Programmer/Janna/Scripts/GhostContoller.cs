using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;

public class GhostController : PlayerController, IMove, IDash, IAscend, IStep
{
    #region Parameters

    private Action<InputAction.CallbackContext> _dashAction;
    private Action<InputAction.CallbackContext> _startAscendAction;
    private Action<InputAction.CallbackContext> _stopAscendAction;

    private Vector2 input;
    private bool isAscending = false;
    private bool canDash = true;
    private bool isDashing = false;

    PlayerState GhostState;

    [Header("BroadCasting")]
    // refernce of two events 

    [Header("Movement")]
    [SerializeField] private float Speed = 15f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 100f;
    [SerializeField] private float dashDistance = 10f;
    private float dashTime;

    [Header("Ascending")]
    [SerializeField] private float ascendingSpeed =40f;
    private float ghostDistance = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    private float distanceToGround;
    private RaycastHit reachedDistance;

    [Header("Steps")]
    [SerializeField] private float startRay = 0.15f;
    [SerializeField] private float rayLength = 1.5f;
    [SerializeField] private float stepSmooth = 7.3f;
    [SerializeField] private float stepHeight = 0.4f;
    [SerializeField] private float maxClimbHeight = 0.25f;

    private int RunAnimationId;
    private RaycastHit ishit;
    [SerializeField] private LayerMask detectWall;

    #endregion
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        _dashAction = context => Dash();
    }
    private void Update()
    {
        input = _inputManager.GhostActions.Move.ReadValue<Vector2>().normalized;
        if (input.magnitude > 0.1f)
        {
            Move(input);
        }
        
        if (isAscending)
        {
            Ascend();
        }
        else
        {
            ApplyGravity();
        }
    }

    protected override void OnMovePerformed(InputAction.CallbackContext context)
    {
        GhostState = PlayerState.moving;
        input = context.ReadValue<Vector2>().normalized;
    }

    protected override void OnMoveCanceled(InputAction.CallbackContext context)
    {
        rb.velocity = Vector3.zero;
    }

    protected override void OnDashPerformed(InputAction.CallbackContext context)
    {
        GhostState = PlayerState.Dashing;
        Dash();
    }

    protected override void OnAscendPerformed(InputAction.CallbackContext context)
    {
        //invoke event start ascending
        isAscending = context.ReadValue<float>() > 0.1f;
        isAscending = true;
        GhostState = PlayerState.Ascending;
    }

    protected override void OnAscendCanceled(InputAction.CallbackContext context)
    {
        isAscending = context.ReadValue<float>() > 0.1f;
        isAscending = false;
        GhostState = PlayerState.moving;
    }

    protected override void OnJumpPerformed(InputAction.CallbackContext context)
    {
    }

    /* public void Move(Vector2 input)
     {
         if (GhostState != PlayerState.moving)
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
 }*/
    public void Move(Vector2 input)
    {
        if (GhostState != PlayerState.moving)
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
       
    }

    public bool ShouldStep(Vector3 moveDirection)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = moveDirection;
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, rayDirection, rayLength);

        foreach (RaycastHit hit in hits)
        {
            float heightDifference = hit.point.y - transform.position.y;
            if (heightDifference > 0.1f && heightDifference < stepHeight && heightDifference < maxClimbHeight)
            {
                return true;
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


    public void Dash()
    {
        if (GhostState != PlayerState.Dashing)
            return;
        GhostState = PlayerState.Dashing;
        if (canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    public IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

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

        }
        canDash = true;
        GhostState = PlayerState.moving;
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
