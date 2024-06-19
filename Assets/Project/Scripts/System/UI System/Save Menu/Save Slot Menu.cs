using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotMenu : MonoBehaviour
{
    [SerializeField] private ConfirmationPopupMenu _confirmationPopupMenu;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject _gameObject;

    private SaveSlot[] _saveSlot;
    private ServiceLocator _serviceLocator;
    private DataPersistenceManager _dataPersistenceManager;

    private bool _isGameLoading = false;

    private void Awake()
    {
        _saveSlot = GetComponentsInChildren<SaveSlot>();
        _serviceLocator = ServiceLocator.Instance;
        _dataPersistenceManager = _serviceLocator.GetService<DataPersistenceManager>();
    }

    public void ActivatedMenu(bool _isGameLoading)
    {
        this._isGameLoading = _isGameLoading;

        Dictionary<string, GameData> _profilesGameData = _dataPersistenceManager.GetAllProfilesGameData();

        foreach (SaveSlot saveSlot in _saveSlot)
        {
            _profilesGameData.TryGetValue(saveSlot.GetProfileID(), out GameData _profileData);
            saveSlot.SetData(_profileData);

            if (_profileData == null && _isGameLoading)
                saveSlot.SetButtonInteractable(false);
            else
                saveSlot.SetButtonInteractable(true);
        }
    }

    public void OnSaveSlotClicked(SaveSlot _saveslot)
    {
        DisableMenuButtons();

        if (_isGameLoading)
        {
            _dataPersistenceManager.ChangeSelectedProfile(_saveslot.GetProfileID());
            SaveGameAndLoadScene();
        }
        else if (_saveslot.HasData)
        {
            _confirmationPopupMenu.ActivateMenu(
                "Do you want to overwrite this save?",
                () =>
                {
                    _dataPersistenceManager.ChangeSelectedProfile(_saveslot.GetProfileID());
                    _dataPersistenceManager.NewGame();
                    SaveGameAndLoadScene();
                },
                () => ActivatedMenu(_isGameLoading));

            if (eventSystem != null)
                eventSystem.firstSelectedGameObject = _gameObject;
        }
        else
        {
            _dataPersistenceManager.ChangeSelectedProfile(_saveslot.GetProfileID());
            _dataPersistenceManager.NewGame();
            SaveGameAndLoadScene();
        }
    }

    public void OnClearButtonClicked(SaveSlot _saveslot)
    {
        DisableMenuButtons();

        _confirmationPopupMenu.ActivateMenu(
            "Do you want to clear this save?",
            () =>
            {
                _dataPersistenceManager.DeleteProfileData(_saveslot.GetProfileID());
                ActivatedMenu(_isGameLoading);
            },
            () => ActivatedMenu(_isGameLoading)
            );

        eventSystem.firstSelectedGameObject = _gameObject;
    }

    private void SaveGameAndLoadScene()
    {
        _dataPersistenceManager.SaveGame();
        _serviceLocator.GetService<UISystem>().StartLoadingProcess();
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in _saveSlot)
            saveSlot.SetButtonInteractable(false);
    }
}