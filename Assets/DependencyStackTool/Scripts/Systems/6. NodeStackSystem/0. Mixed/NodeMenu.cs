
using UnityEngine;
using System;

public class NodeMenu : MonoBehaviour
{
    [Header("Other Systems")]
    [SerializeField] private ContextMenu _contextMenu;

    [Header("Outside Checking")]
    [SerializeField] private RectTransform _insideRectTransform;

    [Header("Input UI")]
    [SerializeField] private TextInputBox _nodeTextInputBox;

    // Data
    private UINode _currentNode;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;

    // Events
    public event Action<UINode> OnNodeEditedTriggered;


    // ----------------- Functions -----------------


    #region Setter Functions

    private void SetNodeTypedText(string text) => _nodeTextInputBox.SetTypedText(text);

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
        _contextMenu.OnEditNodeTriggered += context => HandleEditingNode(context);
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
        _contextMenu.OnEditNodeTriggered -= context => HandleEditingNode(context);
    }

    #endregion

    #region Input Listening Functions

    private void StartListeningToInputHandler()
    {
        _inputHandler.OnLeftMouseStarted += OutsideCheck;
        _inputHandler.OnRightMouseStarted += CloseNodeMenuTrigger;
        _inputHandler.OnMiddleMouseStarted += CloseNodeMenuTrigger;
        _inputHandler.OnEnterStarted += OutsideCheck;
    }

    private void StopListeningToInputHandler()
    {
        _inputHandler.OnLeftMouseStarted -= OutsideCheck;
        _inputHandler.OnRightMouseStarted -= CloseNodeMenuTrigger;
        _inputHandler.OnMiddleMouseStarted -= CloseNodeMenuTrigger;
        _inputHandler.OnEnterStarted -= OutsideCheck;
    }

    #endregion

    #region Editing Functions

    private void HandleEditingNode(UINode node)
    {
        _currentNode = node;
        _currentNode.LayoutNodeData = node.LayoutNodeData;
        SetNodeTypedText(_currentNode.LayoutNodeData.name);
        _insideRectTransform.gameObject.SetActive(true);
        StartListeningToInputHandler();
        _nodeTextInputBox.FocusTriggered();
    }

    public void CloseNodeMenuTrigger()
    {
        if (_currentNode == null)
            return;

        if (IsDirtyCheck())
        {
            _currentNode.LayoutNodeData.name = _nodeTextInputBox.GetTypedText();
            OnNodeEditedTriggered?.Invoke(_currentNode);
        }

        _currentNode = null;
        _insideRectTransform.gameObject.SetActive(false);
        StopListeningToInputHandler();
    }

    public bool IsDirtyCheck()
    {
        if (_nodeTextInputBox.GetTypedText() != _currentNode.LayoutNodeData.name)
            return true;
        return false;
    }

    #endregion

    // Reworking below with EventSystem!

    #region OutsideCheck Functions

    private void OutsideCheck()
    {
        Vector2 position = _inputHandler.GetMousePosition;
        if (!RectTransformUtility.RectangleContainsScreenPoint(_insideRectTransform, position))
            CloseNodeMenuTrigger();
    }

    #endregion

}
