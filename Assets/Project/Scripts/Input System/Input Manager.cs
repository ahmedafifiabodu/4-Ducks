using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem _playerInput;
    internal InputSystem.PlayerActions PlayerActions { get; private set; }

    private ServiceLocator _serviceLocator;
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this);

        _playerInput = new InputSystem();
        PlayerActions = _playerInput.Player;
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _serviceLocator.GetService<AudioManager>().StopAllAudio();
    }

    private void Start()
    {
        _playerMovement = _serviceLocator.GetService<PlayerMovement>();

        PlayerActions.Run.started += _ => _playerMovement.StartRun();
        PlayerActions.Run.canceled += _ => _playerMovement.StopRun();

        PlayerActions.Jump.performed += _ => _playerMovement.Jump();
    }

    private void FixedUpdate()
    {
        _playerMovement.Move(PlayerActions.Move.ReadValue<Vector2>());
    }
}