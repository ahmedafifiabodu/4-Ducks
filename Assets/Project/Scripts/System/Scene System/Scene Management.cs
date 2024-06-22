using System.Collections;
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

    public delegate void LevelLoadingDelegate(bool isLoading);

    public event LevelLoadingDelegate OnLevelLoading;

    private AsyncOperation loadOperation = null;
    public float LoadProgress => (loadOperation != null) ? loadOperation.progress : 1f;

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

        levelScenes = GetLevelScenes(currentLevel);

        if (levelScenes == null || levelScenes.Count == 0)
        {
            Logging.LogError("No scenes found for level " + currentLevel);

            currentLevel--;
            OnLevelLoading?.Invoke(false);
            return;
        }

        if (currentLevel == 1 && inputManager != null)
            inputManager.PauseActions.Disable();

        OnLevelLoading?.Invoke(true);

        // Assuming UISystem is accessible via a reference or ServiceLocator
        UISystem uiSystem = ServiceLocator.Instance.GetService<UISystem>();
        if (uiSystem != null)
        {
            uiSystem.SimulateLoadingProgress(0.9f, 5f); // Simulate progress to 0.9 over 5 seconds
        }

        // Start the actual scene loading after a delay
        StartCoroutine(StartActualLoading(levelScenes));
    }

    private List<string> GetLevelScenes(int levelNumber)
    {
        foreach (Level level in levels)
            if (level.levelNumber == levelNumber)
                return level.scenes;

        return null;
    }

    internal void SetCurrentLevel(int level) => currentLevel = level - 1;

    private IEnumerator StartActualLoading(List<string> scenes)
    {
        yield return new WaitForSeconds(5f); // Wait for the simulated loading to complete

        if (scenes.Count > 0)
        {
            loadOperation = SceneManager.LoadSceneAsync(scenes[0], LoadSceneMode.Single);
            loadOperation.allowSceneActivation = false;

            for (int i = 1; i < scenes.Count; i++)
            {
                SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);
            }

            // Wait until the load progress reaches 0.9
            while (loadOperation.progress < 0.9f)
            {
                yield return null;
            }

            // Notify UISystem that loading ends before allowing scene activation
            OnLevelLoading?.Invoke(false); // This line should already be in place

            // Allow the scene to activate
            loadOperation.allowSceneActivation = true;
        }
    }

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