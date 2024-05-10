using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem _playerInput;
    internal InputSystem.PlayerActions PlayerActions { get; private set; }

    private ServiceLocator _serviceLocator;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        _playerInput = new InputSystem();
        PlayerActions = _playerInput.Player;
    }

    private void OnEnable() => _playerInput.Enable();

    private void OnDisable() => _playerInput?.Disable();
}