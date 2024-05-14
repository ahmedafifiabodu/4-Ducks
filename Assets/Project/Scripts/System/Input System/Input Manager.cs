using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem _input;
    internal InputSystem.PlayerActions PlayerActions { get; private set; }
    internal InputSystem.GhostActions GhostActions { get; private set; }

    private ServiceLocator _serviceLocator;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        _input = new InputSystem();


        PlayerActions = _input.Player;
        GhostActions = _input.Ghost;
    }

    private void OnEnable() => _input.Enable();

    private void OnDisable() => _input?.Disable();
}