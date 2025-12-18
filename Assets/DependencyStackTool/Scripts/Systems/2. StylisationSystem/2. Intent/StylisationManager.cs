
using UnityEngine;
using System;

// Bad: ToolMaster -> Here
using SaveAndLoadSystem;
using NodeStackSystem;

public class StylisationManager : MonoBehaviour
{
    // Bad: ToolMaster -> Here
    [Header("Other Managers")]
    [SerializeField] private SaveLoader _saveLoader;
    [SerializeField] private NodeStackManager _nodeStackManager;

    [Header("UI Input")]
    [SerializeField] private StylisationTab _stylisationTab;

    // Events
    public event Action OnNodeStackStyleChange;


    // ----------------- Functions -----------------


    #region OnEnable Functions

    private void OnEnable()
    {
        _saveLoader.OnSaveLoadedSuccesfully += context => HandleResyncVisualsWithDataRelay(context.NodeStackStyle);
        _stylisationTab.OnCloseTabTriggered += HandleCloseTabTriggered;
        _stylisationTab.OnNodesNameWidthChanged += HandleNodesNameWidthChanged;
        _stylisationTab.OnNodesExtraWidthChanged += HandleNodesExtraWidthChanged;
        _stylisationTab.OnNodesExtraHeightChanged += HandleNodesExtraHeightChanged;
        _stylisationTab.OnNodesLineWidthChanged += HandleNodesLineWidthChanged;
        _stylisationTab.OnNodesHorizontalSpacingChanged += HandleNodesHorizontalSpacingChanged;
        _stylisationTab.OnNodesVerticalSpacingChanged += HandleNodesVerticalSpacingChanged;
        _stylisationTab.OnNodeStackPaddingChanged += HandleNodeStackPaddingChanged;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        _stylisationTab.OnCloseTabTriggered -= HandleCloseTabTriggered;
        _stylisationTab.OnNodesNameWidthChanged -= HandleNodesNameWidthChanged;
        _stylisationTab.OnNodesExtraWidthChanged -= HandleNodesExtraWidthChanged;
        _stylisationTab.OnNodesExtraHeightChanged -= HandleNodesExtraHeightChanged;
        _stylisationTab.OnNodesLineWidthChanged -= HandleNodesLineWidthChanged;
        _stylisationTab.OnNodesHorizontalSpacingChanged -= HandleNodesHorizontalSpacingChanged;
        _stylisationTab.OnNodesVerticalSpacingChanged -= HandleNodesVerticalSpacingChanged;
        _stylisationTab.OnNodeStackPaddingChanged -= HandleNodeStackPaddingChanged;
    }

    #endregion

    #region Relay Functions

    public void ToggleTabRelay(bool direction) => _stylisationTab.ToggleTab(direction);
    public bool GetTabStateRelay() => _stylisationTab.GetTabState();
    private void HandleResyncVisualsWithDataRelay(NodeStackStyleData style) => _stylisationTab.SetVisualsWithData(style);

    #endregion

    #region Handle Functions


    private void HandleCloseTabTriggered()
    {
        //Debug.Log("Close Tab Triggered");
        // Implement logic to handle tab closing

        ToggleTabRelay(false);
    }
    private void HandleNodesNameWidthChanged(bool isEnabled)
    {
        //Debug.Log($"Nodes Name Width changed to: {isEnabled}");
        // Implement logic to handle nodes name width preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodesNameWidth = isEnabled;
        OnNodeStackStyleChange?.Invoke();
    }
    private void HandleNodesExtraWidthChanged(int width)
    {
        //Debug.Log($"Nodes Extra Width changed to: {width}");
        // Implement logic to handle nodes extra width preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodesExtraWidth = width;
        OnNodeStackStyleChange?.Invoke();
    }
    private void HandleNodesExtraHeightChanged(int height)
    {
        //Debug.Log($"Nodes Extra Height changed to: {height}");
        // Implement logic to handle nodes extra height preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodesExtraHeight = height;
        OnNodeStackStyleChange?.Invoke();
    }
    private void HandleNodesLineWidthChanged(int lineWidth)
    {
        //Debug.Log($"Nodes Line Width changed to: {lineWidth}");
        // Implement logic to handle nodes line width preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodesLineWidth = lineWidth;
        OnNodeStackStyleChange?.Invoke();
    }
    private void HandleNodesHorizontalSpacingChanged(int horizontalSpacing)
    {
        //Debug.Log($"Nodes Horizontal Spacing changed to: {horizontalSpacing}");
        // Implement logic to handle nodes horizontal spacing preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodesHorizontalSpacing = horizontalSpacing;
        OnNodeStackStyleChange?.Invoke();
    }
    private void HandleNodesVerticalSpacingChanged(int verticalSpacing)
    {
        //Debug.Log($"Nodes Vertical Spacing changed to: {verticalSpacing}");
        // Implement logic to handle nodes vertical spacing preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodesVerticalSpacing = verticalSpacing;
        OnNodeStackStyleChange?.Invoke();
    }
    private void HandleNodeStackPaddingChanged(int padding)
    {
        //Debug.Log($"Node Stack Padding changed to: {padding}");
        // Implement logic to handle node stack padding preference change

        _nodeStackManager.GetCurrentNodeStackStyle().NodeStackPadding = padding;
        OnNodeStackStyleChange?.Invoke();
    }

    #endregion

}
