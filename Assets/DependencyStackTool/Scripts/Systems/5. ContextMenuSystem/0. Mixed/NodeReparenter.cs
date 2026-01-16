using System;
using UnityEngine;

public class NodeReparenter : NodeToolBase
{
    [Header("Other Systems")]
    [SerializeField] private ContextMenu _contextMenu;

    // Data
    private bool _isUsingTool = false;
    private Transform _nodeToReparent = null;
    private Transform _nodeNewParent = null;

    // Events
    public event Action<Transform, Transform> OnNodeReparentedTriggered;

    #region OnEnable Functions

    private void OnEnable()
    {
        RegisterInputHandlerRelated();
    }

    protected override void RegisterInputHandlerRelated()
    {
        base.RegisterInputHandlerRelated();

        // Safely register input handler related things.
        _contextMenu.OnReparentNodeTriggered += HandleInitialReparent;
        _contextMenu.OnUsingToolResultTriggered += HandleSecondairReparent;
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
        _contextMenu.OnReparentNodeTriggered -= HandleInitialReparent;
        _contextMenu.OnUsingToolResultTriggered -= HandleSecondairReparent;
        _contextMenu.OnUsingToolAbortTriggered -= HandleAbort;
    }

    #endregion

    #region Reparenting Functions

    private void HandleInitialReparent(Transform transform)
    {
        InsideRectTransform.gameObject.SetActive(true);
        _nodeToReparent = transform;
        _isUsingTool = true;
    }

    private void HandleSecondairReparent(Transform transform)
    {
        if (!_isUsingTool)
            return;

        _nodeNewParent = transform;
        OnNodeReparentedTriggered?.Invoke(_nodeToReparent, _nodeNewParent);
        StopAndReset();
    }

    private void HandleAbort()
    {
        StopAndReset();
    }

    protected override void StopAndReset()
    {
        InsideRectTransform.gameObject.SetActive(false);
        _isUsingTool = false;
        _nodeToReparent = null;
        _nodeNewParent = null;
    }

    #endregion
}
