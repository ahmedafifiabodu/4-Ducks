using TMPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    [Header("Interaction UI Elements")]
    [SerializeField] private GameObject _prompt;

    [SerializeField] private TextMeshProUGUI _promptText;

    [Header("Debugging")]
    [SerializeField] internal TextMeshProUGUI _deathCountText;

    private ServiceLocator _serviceLocator;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);
    }

    #region Interaction UI

    internal void UpdatePromptText(string _promptMessage)
    {
        _prompt.SetActive(true);
        _promptText.text = _promptMessage;
    }

    internal void DisablePromptText() => _prompt.SetActive(false);

    #endregion Interaction UI
}