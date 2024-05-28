using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostDash : MonoBehaviour
{
    #region Parameters
    private InputManager _inputManager;
    private PlayerMovement _playerMovement;
    private Action<InputAction.CallbackContext> _dashAction;

    private Rigidbody rb;
    private RaycastHit ishit;
    [SerializeField] private LayerMask detectWall;

    [Header("Dashing Properties")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDistance = 10f;
    //[SerializeField] private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashTime;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        if (_inputManager != null)
        {
            _dashAction = context => Dash();
            _inputManager.GhostActions.Dash.performed += _dashAction;
        }
    }

    private void OnDisable()
    {
        if (_inputManager != null)
        {
            _inputManager.GhostActions.Dash.performed -= _dashAction;
        }
    }

    private void Dash()
    {
        if (canDash && !isDashing)
        {
            Logging.Log("Dashing");
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        if (_playerMovement != null)
        {
             _playerMovement.enabled = false;
           // _playerMovement.isMoving = false;
        }

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

        if (_playerMovement != null)
        {
            _playerMovement.enabled = true;
          // _playerMovement.isMoving = true;
        }

        canDash = true;
    }

    private void SetWallTrigger(Collider wallCollider, bool isTrigger)
    {
        if (wallCollider != null)
        {
            wallCollider.isTrigger = isTrigger;
        }
    }
}
