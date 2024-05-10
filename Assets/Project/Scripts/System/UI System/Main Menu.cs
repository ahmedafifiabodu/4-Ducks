using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _continueGame;
    [SerializeField] private Button _newGame;
    [SerializeField] private Button _loadGame;

    private ServiceLocator _serviceLocator;

    private void Start()
    {
        _serviceLocator = ServiceLocator.Instance;
        DisableButtonsDependingOnData();
    }

    private void DisableButtonsDependingOnData()
    {
        bool _hasGameStarted = _serviceLocator.GetService<DataPersistenceManager>().HasGameStarted();

        _continueGame.interactable = _hasGameStarted;
        _loadGame.interactable = _hasGameStarted;
    }

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
}