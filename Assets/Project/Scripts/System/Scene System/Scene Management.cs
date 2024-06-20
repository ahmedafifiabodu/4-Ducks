using System.Collections.Generic;
using System.Linq;
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

    public void LoadGame(GameData _gameData)
    {
        if (_gameData._levelsCompleted.Count > 0)
        {
            Logging.Log("_gameData._levelsCompleted.Count: " + _gameData._levelsCompleted.Count);

            // Find the highest completed level
            int lastCompletedLevel = _gameData._levelsCompleted.Max();

            // Set the current level to the next level after the last completed level
            // Ensure that it does not exceed the total number of levels
            currentLevel = Mathf.Min(lastCompletedLevel + 1, levels.Count);
        }
        else
        {
            // If no levels have been completed, start from the first level
            currentLevel = 1;
        }

        // Optionally, start the level automatically
        StartLevel(currentLevel);
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