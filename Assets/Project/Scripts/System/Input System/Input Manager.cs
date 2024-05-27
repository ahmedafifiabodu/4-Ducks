using UnityEngine;

public class InputManager : MonoBehaviour
{
    internal InputSystem.CatActions CatActions { get; private set; }
    internal InputSystem.GhostActions GhostActions { get; private set; }
    internal InputSystem.PossessTurretActions PossessTurretActions { get; private set; }
    internal InputSystem.PossessMovableObjectActions PosssessMovableObjectActions { get; private set; }
    internal InputSystem.PossessCatActions PossessCatActions { get; private set; }

    private InputSystem _input;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);

        _input = new InputSystem();

        CatActions = _input.Cat;
        GhostActions = _input.Ghost;
        PossessTurretActions = _input.PossessTurret;
        PosssessMovableObjectActions = _input.PossessMovableObject;
        PossessCatActions = _input.PossessCat;
    }

    private void OnEnable() => _input.Enable();

    private void OnDisable() => _input?.Disable();
}