using UnityEngine;

public class Possess : Interactable
{
    [SerializeField] private MonoBehaviour _possessableScript;

    protected override void Interact(PlayerType _playerType)
    {
        if (_playerType.IsPlayerGhost)
        {
            if (UseEvents)
            {
                if (gameObject.TryGetComponent<InteractableEvents>(out var _events))
                    _events.onInteract.Invoke();
            }

            _possessableScript.GetComponent<IPossessable>().GhostPlayer = _playerType.gameObject;
            _possessableScript.GetComponent<IPossessable>().Possess();
        }
    }
}