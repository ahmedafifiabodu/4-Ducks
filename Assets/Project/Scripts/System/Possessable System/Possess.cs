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

            _playerType.gameObject.SetActive(false);
            _possessableScript.GetComponent<IPossessable>().Possess();
        }
    }
}