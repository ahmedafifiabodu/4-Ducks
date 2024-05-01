using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathCount : MonoBehaviour, IDataPersistence
{
    [Header("Death Count")]
    [SerializeField] private int _deathCount = 0;

    [SerializeField] private TMPro.TextMeshProUGUI _deathCountText = null;

    private void Start() => _deathCountText.text = $"Death Count: {_deathCount}";

    public void IncreaseDeathCount()
    {
        _deathCount++;
        _deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void DecreaseDeathCount()
    {
        _deathCount--;
        _deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void ResetDeathCount()
    {
        _deathCount = 0;
        _deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void LoadGame(GameData _gameData)
    {
        _deathCount = _gameData._deathCount;
        _deathCountText.text = $"Death Count: {_deathCount}";
    }

    public void SaveGame(GameData _gameData) => _gameData._deathCount = _deathCount;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync("0");
        }
    }
}