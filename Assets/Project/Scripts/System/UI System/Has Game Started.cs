using UnityEngine;
using UnityEngine.UI;

public class HasGameStarted : MonoBehaviour
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

    public void DisableButtonsDependingOnData()
    {
        bool _hasGameStarted = _serviceLocator.GetService<DataPersistenceManager>().HasGameStarted();

        _continueGame.interactable = _hasGameStarted;
        _loadGame.interactable = _hasGameStarted;
    }
}