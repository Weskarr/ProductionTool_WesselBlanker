
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SaveAndLoadTab : MonoBehaviour
{
    [Header("Tab Toggling")]
    [SerializeField] private RectTransform _tabTransform;

    [Header("Outside Checking")]
    [SerializeField] private RectTransform _insideTab;
    [SerializeField] private RectTransform _insideTabBar;

    [Header("Input Name")]
    [SerializeField] private TextInputBox _textInputBox;

    [Header("Input UI")]
    [SerializeField] private Button _closeTabButton;
    [SerializeField] private Button _saveButton;

    [Header("Saves Visuals")]
    [SerializeField] private RectTransform _visualSavesContainer;
    [SerializeField] private SaveVisual _saveVisualPrefab;
    private HashSet<SaveVisual> _registeredSaves = new();

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;

    // Events
    public event Action OnCloseTabTriggered;
    public event Action<string> OnSaveTriggered;
    public event Action<string> OnLoadTriggeredRelay;
    public event Action<string> OnDeleteTriggeredRelay;


    // ----------------- Functions -----------------


    #region Getter Functions

    public bool GetTabState() => _tabTransform.gameObject.activeSelf;

    #endregion

    #region OnEnable Functions

    private void OnEnable()
    {
        RegisterInputHandlerRelated();
    }

    private void RegisterInputHandlerRelated()
    {
        // Safety check.
        if (_isRegistered)
            return;
        _isRegistered = true;

        // Safely register input handler related things.
        _inputHandler.OnAnyMouseStarted += OutsideCheck;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        UnregisterInputHandlerRelated();
    }

    private void UnregisterInputHandlerRelated()
    {
        // Safety check.
        if (!_isRegistered)
            return;
        _isRegistered = false;

        // Safely unregister input handler related things.
        _inputHandler.OnAnyMouseStarted -= OutsideCheck;
    }

    #endregion

    #region Toggle Functions

    public void ToggleTab(bool direction)
    {
        _tabTransform.gameObject.SetActive(direction);

        if (direction)
        {
            if (!_isRegistered)
                RegisterInputHandlerRelated();
        }
        else
        {
            UnregisterInputHandlerRelated();
        }
    }

    #endregion

    #region Trigger Functions

    public void CloseTabTrigger() => OnCloseTabTriggered?.Invoke();

    public void SaveTrigger()
    {
        string saveName = _textInputBox.GetTypedText();
        if (string.IsNullOrWhiteSpace(saveName))
            return;

        OnSaveTriggered?.Invoke(_textInputBox.GetTypedText());
    }

    #endregion

    #region Register Saves Functions

    public void RegisterSave(SaveVisual save)
    {
        if (_registeredSaves.Contains(save))
            return;

        _registeredSaves.Add(save);

        save.OnLoadTriggered += OnLoadTriggeredRelay;
        save.OnDeleteTriggered += OnDeleteTriggeredRelay;
    }

    public void RegisterAllSaves()
    {
        foreach (var save in _registeredSaves)
            RegisterSave(save);
    }

    #endregion

    #region Unregister Saves Functions

    public void UnregisterSave(SaveVisual save)
    {
        if (!_registeredSaves.Contains(save))
            return;

        _registeredSaves.Remove(save);

        save.OnLoadTriggered -= OnLoadTriggeredRelay;
        save.OnDeleteTriggered -= OnDeleteTriggeredRelay;
    }

    public void UnregisterAllSaves()
    {
        foreach (var save in _registeredSaves)
            UnregisterSave(save);

        _registeredSaves.Clear();
    }

    #endregion

    #region Refresh Saves Functions

    public void RefreshSaveVisuals(HashSet<string> saves)
    {
        foreach (Transform child in _visualSavesContainer)
        {
            Destroy(child.gameObject);
        }
        _registeredSaves.Clear();

        foreach (string savePath in saves)
        {
            SaveVisual newSaveVisual = Instantiate(_saveVisualPrefab, _visualSavesContainer);
            newSaveVisual.SetSavePath(savePath);
            RegisterSave(newSaveVisual);
        }
    }

    #endregion

    // Reworking below with EventSystem!

    #region OutsideCheck Functions

    void OutsideCheck()
    {
        Vector2 position = _inputHandler.GetMousePosition;
        if (!RectTransformUtility.RectangleContainsScreenPoint(_insideTab, position) &&
            !RectTransformUtility.RectangleContainsScreenPoint(_insideTabBar, position))
            CloseTabTrigger();
    }

    #endregion
}
