using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 1f;

    [Header("Physics")]
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float maxVelocity = 10f;
    [SerializeField] private float maxForce = 20f;

    private float VelocityNode;

    private int jumpCount = 0;

    private InputManager _inputManager;
    private Action<InputAction.CallbackContext> _jumpAction;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _jumpAction = _ => Jump();
        _inputManager.PlayerActions.Jump.performed += _jumpAction;
    }

    private void OnDisable() => _inputManager.PlayerActions.Jump.performed -= _jumpAction;

    private void Jump()
    {
        if (jumpCount < 2)
        {
            VelocityNode = (2 * gravity * jumpHeight) / jumpDuration;
            rb.velocity = new Vector3(rb.velocity.x, VelocityNode, rb.velocity.z);
            jumpCount++;
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        LimitVelocity();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
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
