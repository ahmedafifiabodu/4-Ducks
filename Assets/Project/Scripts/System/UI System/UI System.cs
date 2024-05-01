using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISystem : MonoBehaviour
{
    [Header("Interaction UI Elements")]
    [SerializeField] private GameObject _prompt;

    [SerializeField] private TextMeshProUGUI _promptText;

    private ServiceLocator _serviceLocator;

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this);
    }

    #region Interaction UI

    internal void UpdatePromptText(string _promptMessage)
    {
        _prompt.SetActive(true);
        _promptText.text = _promptMessage;
    }

    internal void DisablePromptText() => _prompt.SetActive(false);

    #endregion Interaction UI

    #region Main Menu

    /*public void OnNewGameClicked()
    {
        _serviceLocator.GetService<DataPersistenceManager>().NewGame();

        SceneManager.LoadSceneAsync(1);
    }*/

    public void OnContiuneGameClicked()
    {
        _serviceLocator.GetService<DataPersistenceManager>().SaveGame();
        SceneManager.LoadSceneAsync(1);
    }

    public void OnExitClicked() => Application.Quit();

    #endregion Main Menu
}