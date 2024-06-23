using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _continueGame;

    [SerializeField] private Button _newGame;
    [SerializeField] private Button _loadGame;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _continueGameText;
    [SerializeField] private TextMeshProUGUI _loadGameText;

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

        // Set color opacity based on _hasGameStarted
        Color continueGameTextColor = _continueGameText.color;
        Color loadGameTextColor = _loadGameText.color;

        if (_hasGameStarted)
        {
            continueGameTextColor.a = 1f; // 255 in terms of opacity
            loadGameTextColor.a = 1f; // 255 in terms of opacity
        }
        else
        {
            continueGameTextColor.a = 0.35f; // 90% opacity
            loadGameTextColor.a = 0.35f; // 90% opacity
        }

        _continueGameText.color = continueGameTextColor;
        _loadGameText.color = loadGameTextColor;
    }

    public void OnContiuneGameClicked()
    {
        _serviceLocator.GetService<DataPersistenceManager>().SaveGame();
        _serviceLocator.GetService<SceneManagement>().StartLevel();
    }

    public void OnExitClicked() => Application.Quit();
}