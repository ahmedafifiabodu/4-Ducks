using Fungus;
using System;
using UnityEngine;
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

    [Header("Movement")]
    public Rigidbody rb;
    [SerializeField] private float Speed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    [Header("Jump")]
    private bool isGrounded = true;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private AnimationCurve jumpCurve; 
    [SerializeField] private float jumpDuration = 1f;


    [Header("Animation")]
    private Animator _animator;
    private int RunAnimationId;
    private int RunAnimationIdY;
    float p_anim;
    float p_animVerical;
    [SerializeField]  float smooth = 2;

    #endregion Parameters

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, false);

        //Animation
        _animator = GetComponent<Animator>();


        RunAnimationId = Animator.StringToHash(GameConstant.Animation.IsRunning);
        RunAnimationIdY = Animator.StringToHash(GameConstant.Animation.IsRunningY);
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
            {
                _moveCoroutine = StartCoroutine(ContinuousMove());
                //StartCoroutine(StartRunAnimation());
            }
        };
        _inputManager.PlayerActions.Move.started += _startMoveAction;

        _stopMoveAction = _ =>
        {
            if (_moveCoroutine != null)
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
        _inputManager.PlayerActions.Jump.performed -= _jumpAction;
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

            rb.velocity = transform.forward * Speed;
          

            Animate(input);

            yield return null;
        }
    }

    void Animate(Vector2 input)
    {

        if (Math.Abs(p_anim - input.x) < 0.1f)
            p_anim = input.x;

        if (p_anim < input.x)
            p_anim += Time.deltaTime * smooth;
        else if (p_anim > input.x)
            p_anim -= Time.deltaTime * smooth;

        if(Math.Abs(p_animVerical - input.y) < 0.1f)
            p_animVerical = input.y;

        if(p_animVerical < input.y)
            p_animVerical += Time.deltaTime * smooth;
        else if (p_animVerical > input.y)
            p_animVerical -= Time.deltaTime * smooth;

        _animator.SetFloat(RunAnimationId, p_anim);
        _animator.SetFloat(RunAnimationIdY, p_animVerical);
    }

    //private System.Collections.IEnumerator StartRunAnimation()
    //{
    //    float elapsedTime = 0f;
    //    float duration = 0.2f; // Duration over which to interpolate

    //    while (elapsedTime < duration)
    //    {
    //        float value = Mathf.Lerp(0, 1, elapsedTime / duration);
    //        _animator.SetFloat(RunAnimationId, value);

    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    _animator.SetFloat(RunAnimationId, 1);
    //}

    private System.Collections.IEnumerator StopMoveSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // Duration over which to interpolate

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

    internal void Jump()
    {
        if (isGrounded)
        {
            float normalizedTime = Mathf.Clamp01(jumpCurve.keys[jumpCurve.length - 1].time);
            float evaluatedJumpForce = jumpCurve.Evaluate(Time.time / normalizedTime) * jumpForce;

            rb.AddForce(Vector3.up * evaluatedJumpForce, ForceMode.Impulse);
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