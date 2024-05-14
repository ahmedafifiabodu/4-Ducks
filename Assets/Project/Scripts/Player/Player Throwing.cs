using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrowing : MonoBehaviour
{
    private InputManager _inputManager;

    private Action<InputAction.CallbackContext> _startThrowAction;

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _startThrowAction = ctx => StartThrow();

        _inputManager.PlayerActions.Throw.started += _startThrowAction;
    }

    private void OnDisable()
    {
        _inputManager.PlayerActions.Throw.started -= _startThrowAction;
    }

    private void StartThrow()
    {
        Logging.Log("Throwing");
    }
}