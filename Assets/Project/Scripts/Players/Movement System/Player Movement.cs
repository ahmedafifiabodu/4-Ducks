using FMOD.Studio;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Parameters

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    private InputManager _inputManager;
    private Rigidbody rb;
    private Animator _animator;
    [SerializeField] private bool isCat;

    [Header("CatAnimation")]
    [SerializeField] private float smooth = 5;

    private int RunAnimationId;
    private int RunAnimationIdY;
    private float p_anim;
    private float p_animVerical;

    [Header("Movement")]
    [SerializeField] private float Speed = 4f;

    [SerializeField] private float rotationSpeed = 0.5f;
    private Vector2 input;
    private bool isMoving;

    [Header("Audio")]
    private EventInstance PlayerFootSteps;

    private AudioSystemFMOD AudioSystem;
    private FMODEvents FmodSystemn;

    #endregion Parameters

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.CatAnimation.HorizontalMove);
        //RunAnimationIdY = Animator.StringToHash(GameConstant.CatAnimation.VerticalMove);
    }

    private void OnEnable()
    {
        _startMoveAction = _ =>
        {
            if (isCat)
            {
                PlayerFootSteps.getPlaybackState(out PLAYBACK_STATE playbackstate);
                PlayerFootSteps.start();
            }

            isMoving = true;
        };

        _stopMoveAction = _ =>
        {
            isMoving = false;
            StartCoroutine(StopMoveSmoothly());
            rb.velocity = Vector3.zero;
            PlayerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
        };

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        if (isCat)
        {
            _inputManager.CatActions.Move.started += _startMoveAction;
            _inputManager.CatActions.Move.canceled += _stopMoveAction;
        }
        else
        {
            _inputManager.GhostActions.Move.started += _startMoveAction;
            _inputManager.GhostActions.Move.canceled += _stopMoveAction;
        }
    }

    private void Start()
    {
        AudioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();
        FmodSystemn = ServiceLocator.Instance.GetService<FMODEvents>();

        PlayerFootSteps = AudioSystem.CreateEventInstance(FmodSystemn.PlayerSteps);
    }

    private void OnDisable()
    {
        if (isCat)
        {
            _inputManager.CatActions.Move.started -= _startMoveAction;
            _inputManager.CatActions.Move.canceled -= _stopMoveAction;
        }
        else
        {
            _inputManager.GhostActions.Move.started -= _startMoveAction;
            _inputManager.GhostActions.Move.canceled -= _stopMoveAction;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            if (isCat)
                input = _inputManager.CatActions.Move.ReadValue<Vector2>().normalized;
            else
                input = _inputManager.GhostActions.Move.ReadValue<Vector2>().normalized;

            Move();
        }
    }

    private void Move()
    {
        Vector3 forward = transform.forward;
        //Vector3 right = transform.right;
        Vector3 forwardMovement = forward.normalized * input.y;
        //Vector3 rotationDirection = right.normalized * input.x;

        // Vector3 inputDirection = (right * input.x + forward * input.y).normalized;

        if (MathF.Abs(input.x) != 0)
        {
            transform.forward = Quaternion.Euler(0, input.x, 0) * transform.forward;
            /*            Quaternion targetRotation = transform.forward * Quaternion.Euler(0, input.x, 0);
                            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);*/
        }

        // Move in the input direction
        Vector3 newVelocity = forwardMovement * Speed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;

        Animate(input);
    }

    private void Animate(Vector2 input)
    {
        /*if (Math.Abs(p_anim - input.x) < 0.1f)
            p_anim = input.x;*/

        if (p_anim < input.x || p_anim >= input.x)
        {
            p_anim += Time.deltaTime * smooth;
        }

        /*if (Math.Abs(p_animVerical - input.y) < 0.1f)
            p_animVerical = input.y;

        if (p_animVerical < input.y || p_animVerical >= input.y)
        {
            p_animVerical += Time.deltaTime * smooth;
            Logging.Log("Forward ?? ");
        }*/

        // Clamp p_anim to be between 0 and 0.5
        p_anim = Mathf.Clamp(p_anim, 0.0f, 0.25f);
        //p_animVerical = Mathf.Clamp(p_animVerical, 0.0f, 0.5f);

        _animator.SetFloat(RunAnimationId, p_anim);
        //_animator.SetFloat(RunAnimationIdY, p_animVerical);
    }

    private System.Collections.IEnumerator StopMoveSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(1, 0, elapsedTime / duration);
            _animator.SetFloat(RunAnimationId, value);
            //_animator.SetFloat(RunAnimationIdY, value);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _animator.SetFloat(RunAnimationId, 0);
        //_animator.SetFloat(RunAnimationIdY, 0);
        p_anim = 0;
        //p_animVerical = 0;
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