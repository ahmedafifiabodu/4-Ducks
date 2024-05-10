using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    private InputManager _inputManager;
    private Coroutine _moveCoroutine;

    private int _move = 0;
    private int _run = 0;
    private int _jump = 0;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this);
    }

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.PlayerActions.Run.started += _ => StartRun();
        _inputManager.PlayerActions.Run.canceled += _ => StopRun();

        _inputManager.PlayerActions.Jump.performed += _ => Jump();

        _inputManager.PlayerActions.Move.started += ctx =>
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(ContinuousMove(ctx.ReadValue<Vector2>()));
        };

        _inputManager.PlayerActions.Move.canceled += _ =>
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        };
    }

    private void OnDestroy()
    {
        _inputManager.PlayerActions.Run.started -= _ => StartRun();
        _inputManager.PlayerActions.Run.canceled -= _ => StopRun();

        _inputManager.PlayerActions.Jump.performed -= _ => Jump();

        _inputManager.PlayerActions.Move.started -= ctx =>
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(ContinuousMove(ctx.ReadValue<Vector2>()));
        };

        _inputManager.PlayerActions.Move.canceled -= _ =>
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        };
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