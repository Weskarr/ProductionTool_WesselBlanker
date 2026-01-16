using System;
using UnityEngine;


public abstract class NodeToolBase : MonoBehaviour
{
    [Header("Outside Checking")]
    [SerializeField] private RectTransform _insideRectTransform;

    public RectTransform InsideRectTransform => _insideRectTransform;

    #region Input Setup Functions

    // Protected
    public bool IsRegistered => _isRegistered;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered;

    protected virtual void RegisterInputHandlerRelated()
    {
        // Safety check.
        if (_isRegistered)
            return;
        _isRegistered = true;
    }

    protected virtual void UnregisterInputHandlerRelated()
    {
        // Safety check.
        if (!_isRegistered)
            return;
        _isRegistered = false;
    }

    #endregion

    #region Input Listening Functions

    public void StartListeningToInputHandler()
    {
        _inputHandler.OnLeftMouseStarted += OutsideCheck;
        _inputHandler.OnEnterStarted += OutsideCheck;

        _inputHandler.OnRightMouseStarted += StopAndReset;
        _inputHandler.OnMiddleMouseStarted += StopAndReset;
    }

    public void StopListeningToInputHandler()
    {
        _inputHandler.OnLeftMouseStarted -= OutsideCheck;
        _inputHandler.OnEnterStarted -= OutsideCheck;

        _inputHandler.OnRightMouseStarted -= StopAndReset;
        _inputHandler.OnMiddleMouseStarted -= StopAndReset;
    }

    #endregion

    #region OutsideCheck Functions

    private void OutsideCheck()
    {
        Vector2 position = _inputHandler.GetMousePosition;
        if (!RectTransformUtility.RectangleContainsScreenPoint(_insideRectTransform, position))
            StopAndReset();
    }

    protected virtual void StopAndReset() { }

    #endregion
}
