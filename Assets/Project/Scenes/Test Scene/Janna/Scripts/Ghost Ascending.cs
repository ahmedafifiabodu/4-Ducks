using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostAscending : MonoBehaviour
{
    private InputManager _inputManager;
    private Action<InputAction.CallbackContext> _startAscendAction;
    private Action<InputAction.CallbackContext> _stopAscendAction;

    private Rigidbody rb;
    private RaycastHit reachedDistance;
    private float distanceToGround;

    private bool isAscending = false;
    private float ghostDistance = 1.2f;
    [SerializeField] private float ascendingSpeed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startAscendAction = context => StartAscend();
        _stopAscendAction = context => StopAscend();

        _inputManager.GhostActions.Ascend.started += _startAscendAction;
        _inputManager.GhostActions.Ascend.canceled += _stopAscendAction;
    }

    private void OnDisable()
    {
        _inputManager.GhostActions.Ascend.started -= _startAscendAction;
        _inputManager.GhostActions.Ascend.canceled -= _stopAscendAction;
    }

    private void Update()
    {
        if (isAscending)
        {
            Ascend();
        }
        else
        {
            ApplyGravity();
        }
    }

    private void StartAscend()
    {
        Logging.Log("Ascend started");
        isAscending = true;
    }

    private void StopAscend()
    {
        Logging.Log("Ascend stopped");
        isAscending = false;
    }

    private void Ascend()
    {
        rb.velocity = new Vector3(rb.velocity.x, ghostDistance * ascendingSpeed, rb.velocity.z);
    }

    private void ApplyGravity()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out reachedDistance))
        {
            distanceToGround = reachedDistance.distance;

            if (distanceToGround > ghostDistance + 0.1f)
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
}