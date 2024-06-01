using Febucci.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("Dialog Canvas")]
    [SerializeField] private Canvas _dialogCanvas;

    [Header("Dialog Elements")]
    [SerializeField] private Image _iconImage;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _dialogText;

    [Header("Typewriter")]
    [SerializeField] private TypewriterByCharacter _typewriter;

    [SerializeField] private List<DialogText> _dialogs;

    private InputManager _inputManager;

    private readonly Queue<string> _dialog = new();

    private bool _conversationEnded = false;
    private bool _isTextFullyShown = true;
    private int _currentDialogIndex = 0;
    private int _currentCharacterIndex = 0;

    private void Start()
    {
        _dialogCanvas.gameObject.SetActive(false);

        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.DialogActions.NextDialog.performed += _ => NextDialog();

        foreach (var dialog in _dialogs)
            dialog.IsDialogEnded = false;
    }

    public void DisplayNextDialog(int dialogIndex)
    {
        if (_dialog.Count == 0)
        {
            if (!_conversationEnded)
            {
                // Check if the current dialog has ended
                if (_dialogs[dialogIndex].IsDialogEnded)
                {
                    // Load the after dialog
                    LoadAfterDialog(dialogIndex);
                }
                else
                {
                    // Start conversation
                    StartConversation(dialogIndex);
                }
            }
            else
            {
                // End conversation
                EndConversation();

                return;
            }
        }
    }

    private void StartConversation(int _dialogIndex)
    {
        // Set the current dialog index
        _currentDialogIndex = _dialogIndex;

        // Disable the cat actions
        _inputManager.CatActions.Disable();

        // Disable the ghost actions
        _inputManager.GhostActions.Disable();

        // Enable the dialog actions
        _inputManager.DialogActions.Enable();

        // If the panel is not active, activate it
        if (!_dialogCanvas.gameObject.activeSelf)
            _dialogCanvas.gameObject.SetActive(true);

        // Update the NPC image
        if (_dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._image != null)
            _iconImage.sprite = _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._image.sprite;

        // Update the NPC name
        _nameText.text = _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._name;

        // Add the dialog to the queue
        foreach (string dialog in _dialogs[_currentDialogIndex].Dialog[_currentCharacterIndex]._text)
            _dialog.Enqueue(dialog);

        // Display the first dialog
        NextDialog();
    }

    private void LoadAfterDialog(int _dialogIndex)
    {
        // Set the current dialog index
        _currentDialogIndex = _dialogIndex;

        // Disable the cat actions
        _inputManager.CatActions.Disable();

        // Disable the ghost actions
        _inputManager.GhostActions.Disable();

        // Enable the dialog actions
        _inputManager.DialogActions.Enable();

        // If the panel is not active, activate it
        if (!_dialogCanvas.gameObject.activeSelf)
            _dialogCanvas.gameObject.SetActive(true);

        // Update the NPC image
        if (_dialogs[_currentDialogIndex].AfterDialog[_currentCharacterIndex]._image != null)
            _iconImage.sprite = _dialogs[_currentDialogIndex].AfterDialog[_currentCharacterIndex]._image.sprite;

        // Update the NPC name
        _nameText.text = _dialogs[_currentDialogIndex].AfterDialog[_currentCharacterIndex]._name;

        // Add the dialog to the queue
        foreach (string dialog in _dialogs[_currentDialogIndex].AfterDialog[_currentCharacterIndex]._text)
            _dialog.Enqueue(dialog);

        // Display the first dialog
        NextDialog();
    }

    private void NextDialog()
    {
        // If there is something in the dialog queue
        if (_dialog.Count > 0)
        {
            if (!_isTextFullyShown && !_typewriter.TextAnimator.allLettersShown)
            {
                // Skip the typewriter effect
                _typewriter.SkipTypewriter();

                // Set _isTextFullyShown to true
                _isTextFullyShown = true;
            }
            else
            {
                // Display the next dialog
                _dialogText.text = _dialog.Dequeue();

                // Set _isTextFullyShown to false
                _isTextFullyShown = false;
            }
        }
        else
        {
            // Check if the current dialog has ended
            if (_dialogs[_currentDialogIndex].IsDialogEnded)
            {
                // If there are no more dialogs in the queue
                if (_dialogs[_currentDialogIndex].AfterDialog.Count - 1 == _currentCharacterIndex)
                {
                    // Set _conversationEnded to true
                    _conversationEnded = true;

                    // Set the current dialog as ended
                    _dialogs[_currentDialogIndex].IsDialogEnded = true;

                    // End the conversation
                    EndConversation();

                    return;
                }

                // Increase the current character index
                _currentCharacterIndex++;

                // Update the Dialog Data
                LoadAfterDialog(_currentDialogIndex);
            }
            else
            {
                // If there are no more dialogs in the queue
                if (_dialogs[_currentDialogIndex].Dialog.Count - 1 == _currentCharacterIndex)
                {
                    // Set _conversationEnded to true
                    _conversationEnded = true;

                    // Set the current dialog as ended
                    _dialogs[_currentDialogIndex].IsDialogEnded = true;

                    // End the conversation
                    EndConversation();

                    return;
                }

                // Increase the current character index
                _currentCharacterIndex++;

                // Update the Dialog Data
                StartConversation(_currentDialogIndex);
            }
        }
    }

    private void EndConversation()
    {
        Logging.Log("End Conversation");

        // Enable the cat actions
        _inputManager.CatActions.Enable();

        // Enable the ghost actions
        _inputManager.GhostActions.Enable();

        // Disable the dialog actions
        _inputManager.DialogActions.Disable();

        // Clear the dialog queue
        _dialog.Clear();

        // Set _isTextFullyShown to false
        _isTextFullyShown = true;

        // Clear the NPC image
        _iconImage.sprite = null;

        // Clear the NPC name
        _nameText.text = string.Empty;

        // Clear the dialog text
        _dialogText.text = string.Empty;

        // Set _conversationEnded to false
        _conversationEnded = false;

        // Reset the current character index
        _currentCharacterIndex = 0;

        // Deactivate the dialog panel
        if (_dialogCanvas.gameObject.activeSelf)
            _dialogCanvas.gameObject.SetActive(false);
    }
}