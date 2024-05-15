using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turret : MonoBehaviour, IPossessable
{
    [SerializeField] private float _timer = 5f;

    private InputManager _inputManager;
    private WaitForSeconds _wait;
    private Ghost _ghost;

    private Action<InputAction.CallbackContext> _Unpossess;

    private void Start()
    {
        _wait = new WaitForSeconds(_timer);
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();

        _Unpossess = _ => Unpossess();
    }

    private void OnEnable() => StartCoroutine(DeactivateAfterDelay());

    private IEnumerator DeactivateAfterDelay()
    {
        yield return _wait;

        //ObjectPool.SharedInstance.ReturnToPool(2, gameObject);
    }

    public void Possess()
    {
        Logging.Log("Possess");

        _ghost.gameObject.SetActive(false);

        _inputManager.GhostActions.Disable();
        _inputManager.TurretActions.Enable();
    }

    public void Unpossess()
    {
        Logging.Log("Unpossess");

        _ghost.gameObject.SetActive(true);

        _inputManager.GhostActions.Enable();
        _inputManager.TurretActions.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ghost _ghost))
        {
            this._ghost = _ghost;
            _inputManager.TurretActions.Interact.performed += _Unpossess;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _inputManager.TurretActions.Interact.performed -= _Unpossess;
    }
}