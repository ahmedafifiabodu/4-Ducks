using Fungus;
using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    private InputManager _inputManager;
    private Coroutine _moveCoroutine;

    private int _move = 0;
    private int _run = 0;
    private int _jump = 0;

    private Action<InputAction.CallbackContext> _jumpAction;
    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    [Header("Movement")]
    public Rigidbody rb;
    [SerializeField] private float Speed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    [Header("Jump")]
    private bool isGrounded =true;
    [SerializeField] private float jumpForce = 5f;


    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, false);
    }

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _jumpAction = _ => Jump();
        _inputManager.PlayerActions.Jump.performed += _jumpAction;

        _startMoveAction = ctx =>
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            if (this != null)
                _moveCoroutine = StartCoroutine(ContinuousMove(ctx.ReadValue<Vector2>().normalized));
        };
        _inputManager.PlayerActions.Move.started += _startMoveAction;

        _stopMoveAction = _ =>
        {
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                rb.velocity = Vector3.zero;
            }

            };
        _inputManager.PlayerActions.Move.canceled += _stopMoveAction;
    }

    private void OnDisable()
    {

        _inputManager.PlayerActions.Jump.performed -= _jumpAction;
        _inputManager.PlayerActions.Move.started -= _startMoveAction;
        _inputManager.PlayerActions.Move.canceled -= _stopMoveAction;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
    }

    private System.Collections.IEnumerator ContinuousMove(Vector2 _input)
    {
        while (true)
        {
            rb.velocity = new Vector3(_input.x * Speed, 0f, _input.y * Speed);
            Logging.Log("Moving");
            yield return null;
        }
    }
    internal void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        Logging.Log("Jumping");
    }
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }


    public void LoadGame(GameData _gameData)
    {
        _move = _gameData._move;
        _run = _gameData._run;
        _jump = _gameData._jump;

        transform.position = _gameData._playerPosition;
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData._move = _move;
        _gameData._run = _run;
        _gameData._jump = _jump;

        _gameData._playerPosition = transform.position;
    }
}