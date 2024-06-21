using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour, IDataPersistence
{
    [SerializeField] private List<Level> levels = new();

    private int currentLevel = 1;
    private List<string> levelScenes;

    private InputManager inputManager;
    private DataPersistenceManager dataPersistenceManager;

    internal int CurrentLevel => currentLevel;

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);

    private void Start()
    {
        inputManager = ServiceLocator.Instance.GetService<InputManager>();
        dataPersistenceManager = ServiceLocator.Instance.GetService<DataPersistenceManager>();
    }

    public void StartLevel(int _targetLevel = -1)
    {
        if (_targetLevel != -1)
            currentLevel = _targetLevel;
        else
            currentLevel++;

        // Get all scenes in the current level
        levelScenes = GetLevelScenes(currentLevel);

        if (levelScenes == null || levelScenes.Count == 0)
        {
            Logging.LogError("No scenes found for level " + currentLevel);
            currentLevel--;

            return;
        }

        // Check if the current level is the first one and disable PauseActions if so
        if (currentLevel == 1 && inputManager != null)
            inputManager.PauseActions.Disable();

        // Load the first scene in Single mode
        SceneManager.LoadScene(levelScenes[0], LoadSceneMode.Single);

        // Load the rest of the scenes in Additive mode
        for (int i = 1; i < levelScenes.Count; i++)
            SceneManager.LoadScene(levelScenes[i], LoadSceneMode.Additive);
    }

    private List<string> GetLevelScenes(int levelNumber)
    {
        foreach (Level level in levels)
            if (level.levelNumber == levelNumber)
                return level.scenes;

        return null;
    }

    internal void SetCurrentLevel(int level) => currentLevel = level - 1;

    public void LoadGame(GameData _gameData)
    {
    }

    public void SaveGame(GameData _gameData)
    {
        _gameData.SetTotalLevels(levels.Count);

        // Adjusted to add currentLevel - 1 to _levelsCompleted
        int levelToMarkAsCompleted = currentLevel - 1;
        if (levelToMarkAsCompleted > 1 && !_gameData._levelsCompleted.Contains(levelToMarkAsCompleted))
        {
            _gameData._levelsCompleted.Add(levelToMarkAsCompleted);
            dataPersistenceManager.SaveGame();
        }
    }
}