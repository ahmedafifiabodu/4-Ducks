using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    #region Parameters

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private int jumpCount = 0;

    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _jumpAction;

    private Rigidbody rb;

    #endregion Parameters

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
            float normalizedTime = Mathf.Clamp01(jumpCurve.keys[jumpCurve.length - 1].time);
            float evaluatedJumpForce = jumpCurve.Evaluate(Time.time / normalizedTime) * jumpForce;

            rb.AddForce(Vector3.up * evaluatedJumpForce, ForceMode.Impulse);
            jumpCount++;
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