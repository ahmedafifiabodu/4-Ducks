using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem _input;
    internal InputSystem.CatActions CatActions { get; private set; }
    internal InputSystem.GhostActions GhostActions { get; private set; }
    internal InputSystem.PossessTurretActions PossessTurretActions { get; private set; }
    internal InputSystem.PossessMovableObjectActions PossessMovableObjectActions { get; private set; }
    internal InputSystem.PossessCatActions PossessCatActions { get; private set; }

    private ServiceLocator _serviceLocator;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        _input = new InputSystem();

        CatActions = _input.Cat;
        GhostActions = _input.Ghost;
        PossessTurretActions = _input.PossessTurret;
        PossessMovableObjectActions = _input.PossessMovableObject;
        PossessCatActions = _input.PossessCat;
    }

    private void OnEnable() => _input.Enable();

    private void OnDisable() => _input?.Disable();
}