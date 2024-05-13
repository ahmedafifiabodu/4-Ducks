using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    public delegate void PlayerEnterEvent(Transform _player);

    public delegate void PlayerExitEvent(Vector3 _lastKnownPosition);

    public event PlayerEnterEvent OnPlayerEnter;

    public event PlayerExitEvent OnPlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter?.Invoke(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExit?.Invoke(other.transform.position);
        }
    }
}