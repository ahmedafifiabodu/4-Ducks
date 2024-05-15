using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Parameters

    [Header("Animation")]
    [SerializeField] private float smooth = 2;

    [Header("Movement")]
    [SerializeField] private float Speed = 5f;

    private InputManager _inputManager;
    private Coroutine _moveCoroutine;

    private Rigidbody rb;
    private Animator _animator;

    private int RunAnimationId;
    private int RunAnimationIdY;
    private float p_anim;
    private float p_animVerical;

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    #endregion Parameters

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.Animation.HorizontalMove);
        RunAnimationIdY = Animator.StringToHash(GameConstant.Animation.VerticalMove);
    }

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startMoveAction = _ =>
        {
            _moveCoroutine = StartCoroutine(ContinuousMove());
        };
        _inputManager.PlayerActions.Move.started += _startMoveAction;

        _stopMoveAction = _ =>
        {
            {
                StopCoroutine(_moveCoroutine);
                StartCoroutine(StopMoveSmoothly());
                rb.velocity = Vector3.zero;
            }
        };
        _inputManager.PlayerActions.Move.canceled += _stopMoveAction;
    }

    private void OnDisable()
    {
        _inputManager.PlayerActions.Move.started -= _startMoveAction;
        _inputManager.PlayerActions.Move.canceled -= _stopMoveAction;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
    }

    private System.Collections.IEnumerator ContinuousMove()
    {
        while (true)
        {
            Vector2 input = _inputManager.PlayerActions.Move.ReadValue<Vector2>().normalized;

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            Vector3 newVelocity = transform.forward * Speed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;

            Animate(input);

            yield return null;
        }
    }

    private void Animate(Vector2 input)
    {
        if (Math.Abs(p_anim - input.x) < 0.1f)
            p_anim = input.x;

        if (p_anim < input.x)
            p_anim += Time.deltaTime * smooth;
        else if (p_anim > input.x)
            p_anim -= Time.deltaTime * smooth;

        if (Math.Abs(p_animVerical - input.y) < 0.1f)
            p_animVerical = input.y;

        if (p_animVerical < input.y)
            p_animVerical += Time.deltaTime * smooth;
        else if (p_animVerical > input.y)
            p_animVerical -= Time.deltaTime * smooth;

        _animator.SetFloat(RunAnimationId, p_anim);
        _animator.SetFloat(RunAnimationIdY, p_animVerical);
    }

    private System.Collections.IEnumerator StopMoveSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(1, 0, elapsedTime / duration);
            _animator.SetFloat(RunAnimationId, value);
            _animator.SetFloat(RunAnimationIdY, value);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _animator.SetFloat(RunAnimationId, 0);
        _animator.SetFloat(RunAnimationIdY, 0);
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