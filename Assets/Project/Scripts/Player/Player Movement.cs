using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    private InputManager _inputManager;
    private Coroutine _moveCoroutine;

    private int _move = 0;
    private int _run = 0;
    private int _jump = 0;

    private Action<InputAction.CallbackContext> _startRunAction;
    private Action<InputAction.CallbackContext> _stopRunAction;
    private Action<InputAction.CallbackContext> _jumpAction;
    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, false);
    }

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startRunAction = _ => StartRun();
        _inputManager.PlayerActions.Run.started += _startRunAction;

        _stopRunAction = _ => StopRun();
        _inputManager.PlayerActions.Run.canceled += _stopRunAction;

        _jumpAction = _ => Jump();
        _inputManager.PlayerActions.Jump.performed += _jumpAction;

        _startMoveAction = ctx =>
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            if (this != null)
                _moveCoroutine = StartCoroutine(ContinuousMove(ctx.ReadValue<Vector2>()));
        };
        _inputManager.PlayerActions.Move.started += _startMoveAction;

        _stopMoveAction = _ =>
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        };
        _inputManager.PlayerActions.Move.canceled += _stopMoveAction;
    }

    private void OnDisable()
    {
        _inputManager.PlayerActions.Run.started -= _startRunAction;
        _inputManager.PlayerActions.Run.canceled -= _stopRunAction;
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
            Logging.Log("Moving");
            yield return null;
        }
    }

    internal void StartRun()
    {
        _run++;
        Logging.Log("Start Running");
    }

    internal void StopRun()
    {
        Logging.Log("Stop Running");
    }

    internal void Jump()
    {
        _jump++;
        Logging.Log("Jumping");
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