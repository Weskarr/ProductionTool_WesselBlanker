
using UnityEngine;
using System;

public class NodeNamer : NodeToolBase
{
    [Header("Other Systems")]
    [SerializeField] private ContextMenu _contextMenu;

    [Header("Input UI")]
    [SerializeField] private TextInputBox _nodeTextInputBox;

    // Data
    private UINode _currentNode;

    // Events
    public event Action<UINode> OnNodeNameEditedTriggered;


    // ----------------- Functions -----------------


    #region Setter Functions

    private void SetNodeTypedText(string text) => _nodeTextInputBox.SetTypedText(text);

    #endregion

    #region OnEnable Functions

    private void OnEnable()
    {
        RegisterInputHandlerRelated();
    }

    protected override void RegisterInputHandlerRelated()
    {
        base.RegisterInputHandlerRelated();

        // Safely register input handler related things.
        _contextMenu.OnEditNameTriggered += HandleEditingNode;
        _contextMenu.OnUsingToolAbortTriggered += HandleAbort;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        UnregisterInputHandlerRelated();
    }

    protected override void UnregisterInputHandlerRelated()
    {
        base.UnregisterInputHandlerRelated();

        // Safely unregister input handler related things.
        _contextMenu.OnEditNameTriggered -= HandleEditingNode;
        _contextMenu.OnUsingToolAbortTriggered += HandleAbort;
    }

    #endregion

    #region Editing Functions

    private void HandleEditingNode(UINode node)
    {
        _currentNode = node;
        _currentNode.LayoutNodeData = node.LayoutNodeData;
        SetNodeTypedText(_currentNode.LayoutNodeData.name);
        InsideRectTransform.gameObject.SetActive(true);
        StartListeningToInputHandler();
        _nodeTextInputBox.FocusEnabler();
    }

    private void HandleAbort()
    {
        StopAndReset();
    }

    public bool IsDirtyCheck()
    {
        if (_nodeTextInputBox.GetTypedText() != _currentNode.LayoutNodeData.name)
            return true;
        return false;
    }

    protected override void StopAndReset()
    {
        if (_currentNode == null)
            return;

        if (IsDirtyCheck())
        {
            _currentNode.LayoutNodeData.name = _nodeTextInputBox.GetTypedText();
            OnNodeNameEditedTriggered?.Invoke(_currentNode);
        }

        _currentNode = null;
        InsideRectTransform.gameObject.SetActive(false);
        StopListeningToInputHandler();
    }

    #endregion

}
