using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PossessableCat : MonoBehaviour, IPossessable
{
    private Ghost _ghost;
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _unpossess;

    private void OnEnable()
    {
        _unpossess = _ => Unpossess();

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.MovableObjectActions.Disable();
    }

    private void OnDisable()
    {
        _inputManager.MovableObjectActions.Disable();
    }

    public void Possess()
    {
        _ghost.gameObject.SetActive(false);

        _inputManager.GhostActions.Disable();
        _inputManager.MovableObjectActions.Enable();
    }

    public void Unpossess()
    {
        _ghost.gameObject.SetActive(true);

        _inputManager.GhostActions.Enable();
        _inputManager.MovableObjectActions.Disable();

        _inputManager.MovableObjectActions.Interact.performed -= _unpossess;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ghost _ghost))
        {
            this._ghost = _ghost;
            _inputManager.MovableObjectActions.Interact.performed += _unpossess;
        }
    }
}