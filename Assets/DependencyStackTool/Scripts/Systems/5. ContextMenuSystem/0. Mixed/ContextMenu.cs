
// [Summary] (By Wessel)
//
// This is a prototype script for a context menu.
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using SaveAndLoadSystem;

public class ContextMenu : MonoBehaviour
{
    [Header("Other Systems")]
    [SerializeField] private SaveDirectReset _saveDirectReset;

    [Header("References")]
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _contextContent;
    [SerializeField] private RectTransform _visualContent;

    [Header("Contexts Appearance")]
    [SerializeField] private GameObject _contextsParent;
    [SerializeField] private RectTransform _destroySingleContext;
    [SerializeField] private RectTransform _destroySingleContextFiller;
    [SerializeField] private RectTransform _destroyBranchContext;
    [SerializeField] private RectTransform _destroyBranchContextFiller;
    [SerializeField] private RectTransform _swapNodeContext;
    [SerializeField] private RectTransform _swapNodeContextFiller;
    [SerializeField] private RectTransform _reparentNodeContext;
    [SerializeField] private RectTransform _reparentNodeContextFiller;
    [SerializeField] private RectTransform[] _contextsToShowDefault;
    [SerializeField] private RectTransform[] _contextsToShowRoot;

    [Header("Context Buttons")]
    [SerializeField] private Button _editNameButton;
    [SerializeField] private Button _swapNodeButton;
    [SerializeField] private Button _addSubNodeButton;
    [SerializeField] private Button _insertNodeButton;
    [SerializeField] private Button _destroySingleButton;
    [SerializeField] private Button _destroyBranchButton;

    // Events
    public event Action<UINode> OnEditNameTriggered;
    public event Action<Transform> OnSwapNodeTriggered;
    public event Action<Transform> OnReparentNodeTriggered;
    public event Action<Transform> OnAddSubNodeTriggered;
    public event Action<Transform> OnInsertNodeTriggered;
    public event Action<Transform> OnDestroySingleTriggered;
    public event Action<Transform> OnDestroyBranchTriggered;

    // For Extension Tools
    public event Action<Transform> OnUsingToolResultTriggered;
    public event Action OnUsingToolAbortTriggered;
    private bool _isUsingTool = false;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;

    // Currents
    private UINode _currentUINode;


    // ----------------- Functions -----------------


    #region Getter Functions

    private UINode GetUINodeUnderMouse(Vector2 mousePosition)
    {
        if (EventSystem.current == null)
            return null;

        var data = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count == 0)
            return null;

        GameObject topHit = results[0].gameObject;

        if (!topHit.CompareTag("Node"))
            return null;

        return topHit.GetComponent<UINode>();
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
        _inputHandler.OnLeftMouseStarted += HandleLeftClick;
        _inputHandler.OnRightMouseStarted += HandleHide;
        _inputHandler.OnMiddleMouseStarted += HandleHide;
        _inputHandler.OnScrollWheelStarted += HandleHide;

        _saveDirectReset.OnSaveReset += ResetContextMenu;
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
        _inputHandler.OnLeftMouseStarted -= HandleLeftClick;
        _inputHandler.OnRightMouseStarted -= HandleHide;
        _inputHandler.OnMiddleMouseStarted -= HandleHide;
        _inputHandler.OnScrollWheelStarted -= HandleHide;

        _saveDirectReset.OnSaveReset -= ResetContextMenu;
    }

    #endregion

    #region Buttons Functions

    public void EditNameTrigger()
    {
        if (_currentUINode != null)
            OnEditNameTriggered?.Invoke(_currentUINode);

        HideMenu();
    }

    public void SwapNodeTrigger()
    {
        if (_currentUINode != null)
            OnSwapNodeTriggered?.Invoke(_currentUINode.LayoutNodeTransform);

        _isUsingTool = true;

        HideMenu();
    }

    public void ReparentNodeTrigger()
    {
        if (_currentUINode != null)
            OnReparentNodeTriggered?.Invoke(_currentUINode.LayoutNodeTransform);

        _isUsingTool = true;

        HideMenu();
    }

    public void AddSubNodeTrigger()
    {
        if (_currentUINode != null)
            OnAddSubNodeTriggered?.Invoke(_currentUINode.LayoutNodeTransform);

        HideMenu();
    }

    public void InsertNodeTrigger()
    {
        Debug.Log($"InsertNodeTrigger: {_currentUINode.LayoutNodeTransform}");

        if (_currentUINode != null)
            OnInsertNodeTriggered?.Invoke(_currentUINode.LayoutNodeTransform);

        HideMenu();
    }

    public void DestroySingleTrigger()
    {
        if (_currentUINode != null)
            OnDestroySingleTriggered?.Invoke(_currentUINode.LayoutNodeTransform);

        HideMenu();
    }

    public void DestroyBranchTrigger()
    {
        if (_currentUINode.LayoutNodeData.id == 0)
            return;

        if (_currentUINode != null)
            OnDestroyBranchTriggered?.Invoke(_currentUINode.LayoutNodeTransform);

        HideMenu();
    }

    #endregion

    #region Handle Functions

    private void HandleLeftClick()
    {
        Vector2 mousePos = _inputHandler.GetMousePosition;
        UINode nodeUnderMouse = GetUINodeUnderMouse(mousePos);

        if (_isUsingTool)
        {
            Transform transform = null;
            if (nodeUnderMouse != null)
                transform = nodeUnderMouse.LayoutNodeTransform;
            OnUsingToolResultTriggered?.Invoke(transform);
            _isUsingTool = false;
            return;
        }

        if (_currentUINode != null)
        {
            // Inside Context Panel, ignore!
            if (RectTransformUtility.RectangleContainsScreenPoint(_contextContent, mousePos))
                return;

            // Outside Any Node
            if (nodeUnderMouse == null)
            {
                HideMenu();
                return;
            }

            // Switch Node or Update Current Node Position
            ShowMenu(nodeUnderMouse, mousePos);
        }
        else
        {
            // No Node Under Mouse.
            if (nodeUnderMouse == null)
                return;

            // Select Node
            ShowMenu(nodeUnderMouse, mousePos);
        }


    }

    private void HandleHide()
    {
        if (_currentUINode == null)
            return;

        HideMenu();
    }

    #endregion

    #region Menu Functions

    private void ShowMenu(UINode node, Vector2 screenPosition)
    {
        _currentUINode = node;
        _contextContent.gameObject.SetActive(true);

        // Convert screen position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            screenPosition,
            null,
            out Vector2 localPos
        );

        _contextContent.anchoredPosition = localPos;

        AllowedToBeDestroyedCheck();

        ClampMenuToCanvas();
    }

    private void HideMenu()
    {
        _contextContent.gameObject.SetActive(false);
        _currentUINode = null;
    }

    private void ClampMenuToCanvas()
    {
        Rect canvasBounds = _canvasRect.rect;
        Rect menuBounds = _contextContent.rect;

        Vector2 pos = _contextContent.anchoredPosition;

        float halfWidth = menuBounds.width * _contextContent.pivot.x;
        float halfHeight = menuBounds.height * _contextContent.pivot.y;

        float minX = canvasBounds.xMin + halfWidth;
        float maxX = canvasBounds.xMax - (menuBounds.width - halfWidth);
        float minY = canvasBounds.yMin + halfHeight;
        float maxY = canvasBounds.yMax - (menuBounds.height - halfHeight);

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _contextContent.anchoredPosition = pos;
    }

    private void AllowedToBeDestroyedCheck()
    {
        if (_currentUINode == null)
            return;

        if (_currentUINode.LayoutNodeData.id == 0)
        {
            foreach (Transform child in _contextsParent.transform)
                child.gameObject.SetActive(false);

            foreach (RectTransform rect in _contextsToShowRoot)
                rect.gameObject.SetActive(true);

            // It's okay to do this only if it has exactly one child.
            int childCount = _currentUINode.LayoutNodeTransform.childCount;
            if (childCount == 1)
            {
                _destroySingleContext.gameObject.SetActive(true);
                _destroySingleContextFiller.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in _contextsParent.transform)
                child.gameObject.SetActive(false);

            foreach (RectTransform rect in _contextsToShowDefault)
                rect.gameObject.SetActive(true);

            // It's not a branch if it has no children; no need then.
            int childCount = _currentUINode.LayoutNodeTransform.childCount;
            if (childCount == 0)
            {
                _destroyBranchContext.gameObject.SetActive(false);
                _destroyBranchContextFiller.gameObject.SetActive(false);
            }
        }

        // No need for swapping if there's only one node.
        int nodesCount = _visualContent.childCount;
        if (nodesCount <= 1)
        {
            _swapNodeContext.gameObject.SetActive(false);
            _swapNodeContextFiller.gameObject.SetActive(false);
        }

        // No need for parenting if there's only two nodes.
        if (nodesCount <= 2)
        {
            _reparentNodeContext.gameObject.SetActive(false);
            _reparentNodeContextFiller.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Reset Functions

    private void ResetContextMenu(NodeStackData nodeStackData)
    {
        HideMenu();
        _currentUINode = null;
        _isUsingTool = false;
        OnUsingToolAbortTriggered?.Invoke();
    }

    #endregion
}
