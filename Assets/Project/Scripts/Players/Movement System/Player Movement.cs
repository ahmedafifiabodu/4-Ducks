using FMOD.Studio;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IDataPersistence
{
    #region Parameters

    [Header("Animation")]
    [SerializeField] private float smooth = 5;

    [Header("Movement")]
    [SerializeField] private float Speed = 8f;

    [SerializeField] private bool isCat;

    private Action<InputAction.CallbackContext> _startMoveAction;
    private Action<InputAction.CallbackContext> _stopMoveAction;

    private InputManager _inputManager;

    private Rigidbody rb;

    private Animator _animator;

    //Animation
    private int RunAnimationId;

    private int RunAnimationIdY;
    private float p_anim;
    private float p_animVerical;

    private Vector2 input;
    private bool isMoving;

    //Audio
    private EventInstance PlayerFootSteps;

    private AudioSystemFMOD AudioSystem;
    private FMODEvents FmodSystemn;

    #endregion Parameters

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.Animation.HorizontalMove);
        //RunAnimationIdY = Animator.StringToHash(GameConstant.Animation.VerticalMove);
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
            _inputManager.PlayerActions.Move.started += _startMoveAction;
            _inputManager.PlayerActions.Move.canceled += _stopMoveAction;
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
            _inputManager.PlayerActions.Move.started -= _startMoveAction;
            _inputManager.PlayerActions.Move.canceled -= _stopMoveAction;
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
                input = _inputManager.PlayerActions.Move.ReadValue<Vector2>().normalized;
            else
                input = _inputManager.GhostActions.Move.ReadValue<Vector2>().normalized;

            Move();
        }
    }

    private void Move()
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        Vector3 newVelocity = transform.forward * Speed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;

        PLAYBACK_STATE playState;


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