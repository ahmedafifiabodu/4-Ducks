using UnityEngine;

public class NextLevel : MonoBehaviour
{
    // Load the next level
    public void LoadNextLevel() => ServiceLocator.Instance.GetService<SceneManagement>().StartLevel();

    public void LoadMainMenuLevel() => ServiceLocator.Instance.GetService<SceneManagement>().StartLevel(1);
}