
using UnityEngine;
using System;
using UnityEngine.UI;

public class StylisationTab : MonoBehaviour, ITab
{
    [Header("Tab Toggling")]
    [SerializeField] private RectTransform _tabTransform;

    [Header("Outside Checking")]
    [SerializeField] private RectTransform _insideTab;
    [SerializeField] private RectTransform _insideTabBar;

    [Header("Input UI")]
    [SerializeField] private Button _closeTabButton;
    [SerializeField] private Toggle _nodesNameWidthToggle;
    [SerializeField] private Slider _NodesExtraWidthSlider;
    [SerializeField] private Slider _NodesExtraHeightSlider;
    [SerializeField] private Slider _NodesLineWidthSlider;
    [SerializeField] private Slider _NodesHorizontalSpacingSlider;
    [SerializeField] private Slider _NodesVerticalSpacingSlider;
    [SerializeField] private Slider _NodeStackPaddingSlider;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;

    // Events
    public event Action OnCloseTabTriggered;
    public event Action<bool> OnNodesNameWidthChanged;
    public event Action<int> OnNodesExtraWidthChanged;
    public event Action<int> OnNodesExtraHeightChanged;
    public event Action<int> OnNodesLineWidthChanged;
    public event Action<int> OnNodesHorizontalSpacingChanged;
    public event Action<int> OnNodesVerticalSpacingChanged;
    public event Action<int> OnNodeStackPaddingChanged;


    // ----------------- Functions -----------------


    #region Getter Functions

    public bool GetTabState() => _tabTransform.gameObject.activeSelf;

    #endregion

    #region Setter Functions

    public void SetVisualsWithData(NodeStackStyleData style)
    {
        Debug.Log("Passage Two");
        Debug.Log($"Toggle Was: {_nodesNameWidthToggle.isOn}");
        Debug.Log($"Toggle Needs: {style.NodesNameWidth}");
        _nodesNameWidthToggle.SetIsOnWithoutNotify(style.NodesNameWidth);
        _NodesExtraWidthSlider.SetValueWithoutNotify(style.NodesExtraWidth);
        _NodesExtraHeightSlider.SetValueWithoutNotify(style.NodesExtraHeight);
        _NodesLineWidthSlider.SetValueWithoutNotify(style.NodesLineWidth);
        _NodesHorizontalSpacingSlider.SetValueWithoutNotify(style.NodesHorizontalSpacing);
        _NodesVerticalSpacingSlider.SetValueWithoutNotify(style.NodesVerticalSpacing);
        _NodeStackPaddingSlider.SetValueWithoutNotify(style.NodeStackPadding);
        Debug.Log($"Toggle Became: {_nodesNameWidthToggle.isOn}");
    }


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

    #region Changed Functions

    public void NodesNameWidthChanged(bool value) => OnNodesNameWidthChanged?.Invoke(value);
    public void NodesExtraWidthChanged(Single value) => OnNodesExtraWidthChanged?.Invoke((int)value);
    public void NodesExtraHeightChanged(Single value) => OnNodesExtraHeightChanged?.Invoke((int)value);
    public void NodesLineWidthChanged(Single value) => OnNodesLineWidthChanged?.Invoke((int)value);
    public void NodesHorizontalSpacingChanged(Single value) => OnNodesHorizontalSpacingChanged?.Invoke((int)value);
    public void NodesVerticalSpacingChanged(Single value) => OnNodesVerticalSpacingChanged?.Invoke((int)value);
    public void NodeStackPaddingChanged(Single value) => OnNodeStackPaddingChanged?.Invoke((int)value);

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
