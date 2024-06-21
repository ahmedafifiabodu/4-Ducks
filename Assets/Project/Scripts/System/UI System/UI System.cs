using System.Collections;
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
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Pause")]
    [SerializeField] private Canvas _pauseCanvas;

    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _controlCanvas;
    [SerializeField] private Canvas _musicCanvas;

    [Header("Debugging")]
    [SerializeField] internal TextMeshProUGUI _deathCountText;

    private ServiceLocator _serviceLocator;
    private InputManager inputManager;
    private SceneManagement sceneManagement;

    private void Awake()
    {
        _serviceLocator = ServiceLocator.Instance;
        _serviceLocator.RegisterService(this, true);

        inputManager = _serviceLocator.GetService<InputManager>();
        sceneManagement = _serviceLocator.GetService<SceneManagement>();
    }

    private void OnEnable()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        inputManager.PauseActions.MenuOpenClose.started += _ => TogglePauseMenu();
        sceneManagement.OnLevelLoading += HandleLevelLoading;
    }

    private void OnDisable()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        if (sceneManagement == null)
            sceneManagement = _serviceLocator.GetService<SceneManagement>();

        inputManager.PauseActions.MenuOpenClose.started -= _ => TogglePauseMenu();
        sceneManagement.OnLevelLoading -= HandleLevelLoading;
    }

    private void OnDestroy()
    {
        if (inputManager == null)
            inputManager = _serviceLocator.GetService<InputManager>();

        if (sceneManagement == null)
            sceneManagement = _serviceLocator.GetService<SceneManagement>();

        inputManager.PauseActions.MenuOpenClose.started -= _ => TogglePauseMenu();
        sceneManagement.OnLevelLoading -= HandleLevelLoading;
    }

    #region Interaction UI

    internal void UpdatePromptText(string _promptMessage)
    {
        _prompt.SetActive(true);
        _promptText.text = _promptMessage;
    }

    internal void DisablePromptText() => _prompt.SetActive(false);

    #endregion Interaction UI

    #region Loading UI

    private void HandleLevelLoading(bool isLoading, float targetProgress, float duration)
    {
        if (isLoading)
        {
            // Enable the loading screen
            loadingScreen.SetActive(true);
            progressBar.value = 0; // Assuming you want to reset the progress bar

            inputManager.DisableAllInputsExceptPause();
            StartCoroutine(SimulateProgressCoroutine(targetProgress, duration));
        }
        else
        {
            StartCoroutine(FadeOutLoadingScreen()); // Start fading out the loading screen
        }
    }

    private IEnumerator FadeOutLoadingScreen()
    {
        float delayBeforeFadeStarts = 2.0f; // Delay in seconds before fade starts

        yield return new WaitForSeconds(delayBeforeFadeStarts); // Wait for the specified delay

        float duration = 1.0f; // Duration in seconds for the fade-out effect
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - (elapsedTime / duration));
            _canvasGroup.alpha = alpha;

            yield return null;
        }

        loadingScreen.SetActive(false);

        _canvasGroup.alpha = 1.0f;
        inputManager.EnableAllInputs();
    }

    private IEnumerator SimulateProgressCoroutine(float targetProgress, float duration)
    {
        float startTime = Time.time;
        float startProgress = progressBar.value;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float elapsed = Time.time - startTime;
            progressBar.value = Mathf.Lerp(startProgress, targetProgress, elapsed / duration);

            yield return null;
        }

        progressBar.value = targetProgress;
    }

    #endregion Loading UI

    #region Pause

    internal void TogglePauseMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return; // Do nothing if it's the main menu

        _pauseCanvas.enabled = !_pauseCanvas.enabled;

        if (inputManager != null)
        {
            if (_pauseCanvas.enabled)
            {
                // Pause menu is now active, disable all inputs except for PauseActions
                inputManager.DisableAllInputsExceptPause();
            }
            else
            {
                // Pause menu is now inactive, re-enable all inputs
                inputManager.EnableAllInputs();
            }
        }
    }

    internal void StartLevel(int level = -1)
    {
        _pauseCanvas.enabled = false;
        _controlCanvas.enabled = false;
        _settingsCanvas.enabled = false;
        _musicCanvas.enabled = false;

        ServiceLocator.Instance.GetService<SceneManagement>().StartLevel(level);
    }

    public void QuitGame() => Application.Quit();

    #endregion Pause
}