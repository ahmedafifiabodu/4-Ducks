using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private Button skipButton;
    [SerializeField] private GameObject cutsceneParent;
    [SerializeField] private Image cutsceneBackground;
    [SerializeField] private Image cutsceneImage;
    [SerializeField] private List<Sprite> cutsceneFrames;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private bool isFinalCutscene = false;

    private InputManager _inputManager;
    private bool _skipToNextImage = false;

    internal bool IsCutscenePlaying { get; private set; }

    private void Start()
    {
        //_inputManager = InputManager.Instance;
    }

    public void StartCutscene()
    {
        IsCutscenePlaying = true;

        cutsceneParent.SetActive(true);
        //_inputManager._playerInput.Disable();

        skipButton.onClick.AddListener(SkipToNextImage);

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        for (int i = 0; i < cutsceneFrames.Count; i++)
        {
            if (_skipToNextImage)
            {
                _skipToNextImage = false;
                continue;
            }

            cutsceneImage.sprite = cutsceneFrames[i];

            yield return FadeIn();
            if (_skipToNextImage)
            {
                _skipToNextImage = false;
                continue;
            }
            yield return WaitForSecondsOrSkip(2);
            if (_skipToNextImage)
            {
                _skipToNextImage = false;
                continue;
            }
            yield return FadeOut();

            _skipToNextImage = false;
        }

        cutsceneParent.SetActive(false);
        cutsceneParent.SetActive(false);

        if (isFinalCutscene)
            ReturnToMainMenu();

        IsCutscenePlaying = false;
    }

    public void SkipToNextImage() => _skipToNextImage = true;

    private IEnumerator FadeIn()
    {
        float t = 0;
        while (t < transitionTime)
        {
            if (_skipToNextImage) yield break;
            t += Time.deltaTime;
            Color color = cutsceneImage.color;
            color.a = Mathf.Lerp(0, 1, t / transitionTime);
            cutsceneImage.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        while (t < transitionTime)
        {
            if (_skipToNextImage) yield break;
            t += Time.deltaTime;
            Color color = cutsceneImage.color;
            color.a = Mathf.Lerp(1, 0, t / transitionTime);
            cutsceneImage.color = color;
            yield return null;
        }
    }

    private IEnumerator WaitForSecondsOrSkip(float seconds)
    {
        float t = 0;
        while (t < seconds)
        {
            if (_skipToNextImage) yield break;
            t += Time.deltaTime;
            yield return null;
        }
    }

    private void ReturnToMainMenu() => SceneManager.LoadScene(0);
}