using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Save Slot Config")]
    [SerializeField] private string _profileID;

    [Header("UI Config")]
    [SerializeField] private GameObject _noDataContent;

    [SerializeField] private GameObject _hasDataContent;

    [SerializeField] private TMPro.TextMeshProUGUI _percentageComplete;
    [Header("Clear Button")]
    [SerializeField] private Button _clearButton;

    private Button _saveSlotbutton;

    public bool HasData { get; private set; } = false;

    private void Awake() => _saveSlotbutton = GetComponent<Button>();

    public void SetData(GameData gameData)
    {
        if (gameData == null)
        {
            HasData = false;

            _noDataContent.SetActive(true);
            _hasDataContent.SetActive(false);
            _clearButton.interactable = false;

            return;
        }

        HasData = true;

        _noDataContent.SetActive(false);
        _hasDataContent.SetActive(true);
        _clearButton.interactable = true;

        _percentageComplete.text = gameData.PercentageComplete() + "% Complete";
    
    }

    public string GetProfileID() => _profileID;

    public void SetButtonInteractable(bool _interactable)
    {
        _saveSlotbutton.interactable = _interactable;
        _clearButton.interactable = _interactable;
    }
}