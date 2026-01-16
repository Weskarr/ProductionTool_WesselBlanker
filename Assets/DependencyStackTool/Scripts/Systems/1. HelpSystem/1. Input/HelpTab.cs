using System;
using UnityEngine;
using UnityEngine.UI;

public class HelpTab : MonoBehaviour, ITab
{
    [Header("Tab Toggling")]
    [SerializeField] private RectTransform _tabTransform;

    [Header("Outside Checking")]
    [SerializeField] private RectTransform _insideTab;
    [SerializeField] private RectTransform _insideTabBar;

    [Header("Input UI")]
    [SerializeField] private Button _closeTabButton;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;

    // Events
    public event Action OnCloseTabTriggered;


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
