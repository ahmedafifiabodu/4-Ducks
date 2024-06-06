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
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.2f; 
    [SerializeField] private float rayLength = 1f;
    [SerializeField] private float startRay = 0.1f;
    private Vector2 input;
    private bool isMoving;

    [Header("Audio")]
    private EventInstance PlayerFootSteps;

    private AudioSystemFMOD AudioSystem;
    private FMODEvents FmodSystem;

    #endregion Parameters

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        RunAnimationId = Animator.StringToHash(GameConstant.CatAnimation.HorizontalMove);
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
        FmodSystem = ServiceLocator.Instance.GetService<FMODEvents>();

        PlayerFootSteps = AudioSystem.CreateEventInstance(FmodSystem.PlayerSteps);
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
        Vector3 rayOrigin = transform.position + Vector3.up * startRay;
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.blue);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            if (hit.point.y - transform.position.y > 0.1f && hit.point.y - transform.position.y < stepHeight)
            {
                Vector3 stepUpPosition = new Vector3(transform.position.x, hit.point.y + stepHeight, transform.position.z);
                rb.MovePosition(Vector3.Lerp(rb.position, stepUpPosition, stepSmooth));
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        Vector3 newVelocity = transform.forward * Speed;
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;

        Animate(input);
    }

    private void Animate(Vector2 input)
    {
        if (p_anim < input.x || p_anim >= input.x)
        {
            p_anim += Time.deltaTime * smooth;
        }

        p_anim = Mathf.Clamp(p_anim, 0.0f, 0.25f);
        _animator.SetFloat(RunAnimationId, p_anim);
    }

    private System.Collections.IEnumerator StopMoveSmoothly()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            float value = Mathf.Lerp(1, 0, elapsedTime / duration);
            _animator.SetFloat(RunAnimationId, value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _animator.SetFloat(RunAnimationId, 0);
        p_anim = 0;
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
