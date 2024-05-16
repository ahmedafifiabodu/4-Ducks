using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessableTurret : MonoBehaviour, IPossessable
{
    private Ghost _ghost;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _unpossess;
    private Action<InputAction.CallbackContext> _shoot;

    private void OnEnable()
    {
        _unpossess = _ => Unpossess();
        _shoot = _ => Shoot();

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.TurretActions.Fire.performed += _shoot;
        _inputManager.TurretActions.Disable();
    }

    private void OnDisable()
    {
        _inputManager.TurretActions.Fire.performed -= _shoot;
        _inputManager.TurretActions.Disable();
    }

    public void Possess()
    {
        _ghost.gameObject.SetActive(false);

        _inputManager.GhostActions.Disable();
        _inputManager.TurretActions.Enable();
    }

    public void Unpossess()
    {
        _ghost.gameObject.SetActive(true);

        _inputManager.GhostActions.Enable();
        _inputManager.TurretActions.Disable();

        _inputManager.TurretActions.Interact.performed -= _unpossess;
    }

    private void Shoot()
    {
        Logging.Log("Move");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ghost _ghost))
        {
            this._ghost = _ghost;
            _inputManager.TurretActions.Interact.performed += _unpossess;
        }
    }
}