using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour, IPossessable
{
    [SerializeField] private float _timer = 5f;

    private InputManager _inputManager;
    private WaitForSeconds _wait;

    private void Start()
    {
        _wait = new WaitForSeconds(_timer);
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
    }

    private void OnEnable() => StartCoroutine(DeactivateAfterDelay());

    private IEnumerator DeactivateAfterDelay()
    {
        yield return _wait;

        ObjectPool.SharedInstance.ReturnToPool(2, gameObject);
    }

    public void Possess()
    {
        _inputManager.GhostActions.Disable();
        _inputManager.TurretActions.Enable();
    }

    public void Unpossess()
    {
        _inputManager.GhostActions.Enable();
        _inputManager.TurretActions.Disable();
    }
}