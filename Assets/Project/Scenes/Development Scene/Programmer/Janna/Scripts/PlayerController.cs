using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerController : MonoBehaviour, IDataPersistence
{
    protected InputManager _inputManager;
    protected Rigidbody rb;
    protected Camera mainCamera;
    protected Animator _animator;
    [SerializeField] private bool isCat;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
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

    public abstract void LoadGame(GameData _gameData);
    public abstract void SaveGame(GameData _gameData);
}
