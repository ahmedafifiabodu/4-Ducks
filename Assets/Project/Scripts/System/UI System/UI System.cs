using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    [Header("Interaction UI Elements")]
    [SerializeField] private GameObject _prompt;

    [SerializeField] private TextMeshProUGUI _promptText;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Slider progressBar;

    [Header("Pause")]
    [SerializeField] private Canvas _pauseCanvas;

    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _controlCanvas;
    [SerializeField] private Canvas _musicCanvas;

    [Header("Debugging")]
    [SerializeField] internal TextMeshProUGUI _deathCountText;

    private ServiceLocator _serviceLocator;
    private InputManager inputManager;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        inputManager = _serviceLocator.GetService<InputManager>();
    }

    private void OnEnable()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        inputManager.PauseActions.MenuOpenClose.started += _ => TogglePauseMenu();
    }

    private void OnDisable()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        inputManager.PauseActions.MenuOpenClose.started -= _ => TogglePauseMenu();
    }

    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded; // Clean up by ensuring we're unsubscribed from the SceneManager.sceneLoaded event

    #region Interaction UI

    internal void UpdatePromptText(string _promptMessage)
    {
        _prompt.SetActive(true);
        _promptText.text = _promptMessage;
    }

    internal void DisablePromptText() => _prompt.SetActive(false);

    #endregion Interaction UI

    #region Loading UI

    internal void StartLoadingProcess()
    {
        loadingScreen.SetActive(true); // Show the loading screen
        progressBar.value = 0; // Reset the progress bar

        // Animate the progress bar to full over 2 seconds
        DOTween.To(() => progressBar.value, x => progressBar.value = x, 1, 2).OnComplete(LoadNextScene);
    }

    internal void StartMainMenuLoadingProcess()
    {
        loadingScreen.SetActive(true); // Show the loading screen
        progressBar.value = 0; // Reset the progress bar

        // Animate the progress bar to full over 2 seconds
        DOTween.To(() => progressBar.value, x => progressBar.value = x, 1, 2).OnComplete(LoadMainMenuScene);
    }

    private void LoadNextScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _serviceLocator.GetService<SceneManagement>().StartLevel();
    }

    private void LoadMainMenuScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _serviceLocator.GetService<SceneManagement>().StartLevel(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe to prevent this method from being called if another scene loads later
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Now that the scene has loaded, perform your DOTween animation or other actions
        PerformPostLoadAnimation();
    }

    private void PerformPostLoadAnimation()
    {
        // Ensure loadingScreen has a CanvasGroup component attached
        if (!loadingScreen.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            Logging.LogError("loadingScreen does not have a CanvasGroup component.");
            return;
        }

        // Add a 1-second delay before starting the fade-out animation over 5 seconds
        canvasGroup.DOFade(0, 3).SetDelay(1).OnComplete(() => loadingScreen.SetActive(false));
    }

    #endregion Loading UI

    #region Pause

    internal void TogglePauseMenu()
    {
        _pauseCanvas.enabled = !_pauseCanvas.enabled;

        if (inputManager != null)
        {
            if (_pauseCanvas.enabled)
            {
                // Pause menu is now active, disable all inputs except for PauseActions
                inputManager.DisableAllInputsExceptPause();
                Time.timeScale = 0;
            }
            else
            {
                // Pause menu is now inactive, re-enable all inputs
                inputManager.EnableAllInputs();
                Time.timeScale = 1;
            }
        }
    }

    public void SetTime(float _time) => Time.timeScale = _time;

    public void Mainmenu()
    {
        _pauseCanvas.enabled = false;
        _controlCanvas.enabled = false;
        _settingsCanvas.enabled = false;
        _musicCanvas.enabled = false;

        ServiceLocator.Instance.GetService<SceneManagement>().StartFirstLevel();
    }

    public void QuitGame() => Application.Quit();

    #endregion Pause
}