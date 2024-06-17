using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isCat;

    protected Rigidbody rb;
    protected Animator _animator;
    protected Camera _camera;

    protected AudioSystemFMOD AudioSystem;
    protected FMODEvents FmodSystem;

    protected InputManager _inputManager;
    protected ServiceLocator _serviceLocator;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _serviceLocator = ServiceLocator.Instance;
        _inputManager = _serviceLocator.GetService<InputManager>();
    }

    protected virtual void Start()
    {
        AudioSystem = _serviceLocator.GetService<AudioSystemFMOD>();
        FmodSystem = _serviceLocator.GetService<FMODEvents>();
        _camera = _serviceLocator.GetService<CameraInstance>().Camera;
    }

    protected virtual void OnEnable()
    {
        if (isCat)
        {
            _inputManager.CatActions.Move.started += OnMovePerformed;
            _inputManager.CatActions.Move.canceled += OnMoveCanceled;
        }
        else
        {
            _inputManager.GhostActions.Move.started += OnMovePerformed;
            _inputManager.GhostActions.Move.canceled += OnMoveCanceled;
        }
        _inputManager.CatActions.Jump.performed += OnJumpPerformed;
        _inputManager.GhostActions.Dash.performed += OnDashPerformed;
        _inputManager.GhostActions.Ascend.started += OnAscendPerformed;
        _inputManager.GhostActions.Ascend.canceled += OnAscendCanceled;
    }

    protected virtual void OnDisable()
    {
        if (isCat)
        {
            _inputManager.CatActions.Move.started -= OnMovePerformed;
            _inputManager.CatActions.Move.canceled -= OnMoveCanceled;
        }
        else
        {
            _inputManager.GhostActions.Move.started -= OnMovePerformed;
            _inputManager.GhostActions.Move.canceled -= OnMoveCanceled;
        }
        _inputManager.CatActions.Jump.performed -= OnJumpPerformed;
        _inputManager.GhostActions.Dash.performed -= OnDashPerformed;
        _inputManager.GhostActions.Ascend.started -= OnAscendPerformed;
        _inputManager.GhostActions.Ascend.canceled -= OnAscendCanceled;
    }

    protected abstract void OnMovePerformed(InputAction.CallbackContext context);

    protected abstract void OnMoveCanceled(InputAction.CallbackContext context);

    protected abstract void OnJumpPerformed(InputAction.CallbackContext context);

    protected abstract void OnDashPerformed(InputAction.CallbackContext context);

    protected abstract void OnAscendPerformed(InputAction.CallbackContext context);

    protected abstract void OnAscendCanceled(InputAction.CallbackContext context);
}

public enum PlayerState
{
    moving, jumping, Dashing, Ascending
}