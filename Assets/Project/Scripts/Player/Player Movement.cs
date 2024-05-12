using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Parameters 
    private InputManager _inputManager;
    private Coroutine _moveCoroutine;

    private int _move = 0;
    private int _run = 0;
    private int _jump = 0;

    private Action<InputAction.CallbackContext> _jumpAction;
    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    public Rigidbody rb;
    [SerializeField] private float Speed = 5f;

    //Animation 
    private Animator _animator;
    int RunAnimationId;
    Vector2 _currentAnimation;
    Vector2 _AnimationVelocity;

    #endregion

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
                for (float i = _currentAnimation.y; i > 0; i--)
                {
                    _currentAnimation = Vector2.SmoothDamp(_currentAnimation, Vector2.zero, ref _AnimationVelocity, 0.1f);
                    Logging.Log(_currentAnimation.y);
                    _animator.SetFloat(RunAnimationId, _currentAnimation.y);

                }
                Logging.Log(_currentAnimation.y);
                //_animator.SetFloat(RunAnimationId, 0f);
                rb.velocity = Vector3.zero;
            }

        };
        _inputManager.PlayerActions.Move.canceled += _stopMoveAction;

        //Animation
        _animator = GetComponent<Animator>();
        RunAnimationId = Animator.StringToHash(GameConstant.Animation.IsRunning);
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
            _currentAnimation = Vector2.SmoothDamp(_currentAnimation, _input, ref _AnimationVelocity, 0.1f);
            rb.velocity = new Vector3(_currentAnimation.x * Speed, 0f, _currentAnimation.y * Speed);
            Logging.Log("Moving");

            _animator.SetFloat(RunAnimationId, _currentAnimation.y);
            yield return null;
        }
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