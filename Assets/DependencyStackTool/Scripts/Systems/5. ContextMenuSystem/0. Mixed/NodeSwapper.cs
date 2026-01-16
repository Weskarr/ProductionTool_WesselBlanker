using System;
using UnityEngine;

public class NodeSwapper : NodeToolBase
{
    [Header("Other Systems")]
    [SerializeField] private ContextMenu _contextMenu;

    // Data
    private bool _isUsingTool = false;
    private Transform _nodeSwapFrom;
    private Transform _nodeSwapToo;

    // Events
    public event Action<Transform, Transform> OnNodeSwappedTriggered;

    #region OnEnable Functions

    private void OnEnable()
    {
        RegisterInputHandlerRelated();
    }

    protected override void RegisterInputHandlerRelated()
    {
        base.RegisterInputHandlerRelated();

        // Safely register input handler related things.
        _contextMenu.OnSwapNodeTriggered += context => HandleInitialSwapping(context);
        _contextMenu.OnUsingToolResultTriggered += context => HandleSecondairSwapping(context);
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
        _contextMenu.OnSwapNodeTriggered -= context => HandleInitialSwapping(context);
        _contextMenu.OnUsingToolResultTriggered -= context => HandleSecondairSwapping(context);
    }

    #endregion

    #region Swapping Functions

    private void HandleInitialSwapping(Transform transform)
    {
        InsideRectTransform.gameObject.SetActive(true);
        _nodeSwapFrom = transform;
        _isUsingTool = true;
    }

    private void HandleSecondairSwapping(Transform transform)
    {
        if (!_isUsingTool)
            return;

        _nodeSwapToo = transform;
        OnNodeSwappedTriggered?.Invoke(_nodeSwapFrom, _nodeSwapToo);
        StopAndReset();
    }

    protected override void StopAndReset()
    {
        InsideRectTransform.gameObject.SetActive(false);
        _isUsingTool = false;
        _nodeSwapToo = null;
        _nodeSwapFrom = null;
    }

    #endregion
}
