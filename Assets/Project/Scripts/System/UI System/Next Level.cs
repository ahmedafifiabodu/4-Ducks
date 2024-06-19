using UnityEngine;

public class NextLevel : MonoBehaviour
{
    // Load the next level
    public void LoadNextLevel() => ServiceLocator.Instance.GetService<UISystem>().StartLoadingProcess();

    public void LoadMainMenuLevel() => ServiceLocator.Instance.GetService<UISystem>().StartMainMenuLoadingProcess();
}