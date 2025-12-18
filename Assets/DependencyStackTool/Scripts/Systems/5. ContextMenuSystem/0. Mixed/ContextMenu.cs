
// [Summary] (By Wessel)
//
// This is a prototype script for a context menu.
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ContextMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _menuPanelRect;
    [SerializeField] private RectTransform _destroySectionRect;

    [Header("Buttons")]
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _addButton;
    [SerializeField] private Button _destroyButton;

    // Currents
    private UINode _currentUINode;

    // Events
    public event Action<Transform> OnAddSubNodeTriggered;
    public event Action<UINode> OnEditNodeTriggered;
    public event Action<Transform> OnDestroyNodeTriggered;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;


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
    }

    #endregion

    #region Buttons Functions

    public void AddSubNodeTrigger()
    {
        if (_currentUINode != null)
            OnAddSubNodeTriggered?.Invoke(_currentUINode.LayoutNodeTransform);
        HideMenu();
    }

    public void EditNodeTrigger()
    {
        if (_currentUINode != null)
            OnEditNodeTriggered?.Invoke(_currentUINode);
        HideMenu();
    }

    public void DestroyNodeTrigger()
    {
        if (_currentUINode.LayoutNodeData.id == 0)
            return;

        if (_currentUINode != null)
            OnDestroyNodeTriggered?.Invoke(_currentUINode.LayoutNodeTransform);
        HideMenu();
    }

    #endregion

    #region Handle Functions

    private void HandleLeftClick()
    {
        Vector2 mousePos = _inputHandler.GetMousePosition;
        UINode nodeUnderMouse = GetUINodeUnderMouse(mousePos);

        if (_currentUINode != null)
        {
            // Inside Context Panel, ignore!
            if (RectTransformUtility.RectangleContainsScreenPoint(_menuPanelRect, mousePos))
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
        _menuPanelRect.gameObject.SetActive(true);

        // Convert screen position to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            screenPosition,
            null,
            out Vector2 localPos
        );

        _menuPanelRect.anchoredPosition = localPos;

        AllowedToBeDestroyedCheck();

        ClampMenuToCanvas();
    }

    private void HideMenu()
    {
        _menuPanelRect.gameObject.SetActive(false);
        _currentUINode = null;
    }

    private void ClampMenuToCanvas()
    {
        Rect canvasBounds = _canvasRect.rect;
        Rect menuBounds = _menuPanelRect.rect;

        Vector2 pos = _menuPanelRect.anchoredPosition;

        float halfWidth = menuBounds.width * _menuPanelRect.pivot.x;
        float halfHeight = menuBounds.height * _menuPanelRect.pivot.y;

        float minX = canvasBounds.xMin + halfWidth;
        float maxX = canvasBounds.xMax - (menuBounds.width - halfWidth);
        float minY = canvasBounds.yMin + halfHeight;
        float maxY = canvasBounds.yMax - (menuBounds.height - halfHeight);

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        _menuPanelRect.anchoredPosition = pos;
    }

    private void AllowedToBeDestroyedCheck()
    {
        if (_currentUINode.LayoutNodeData.id == 0)
            _destroySectionRect.gameObject.SetActive(false);
        else
            _destroySectionRect.gameObject.SetActive(true);
    }

    #endregion

}
