
using UnityEngine;

// Bad: ToolMaster -> Here
using NodeStackSystem;

public class MovementController : MonoBehaviour
{
    // Bad: ToolMaster -> Here
    [Header("Managers & Other Subsystems")]
    [SerializeField] private PreferencesManager _preferencesManager;
    [SerializeField] private NodeStackGenerator _nodeStackGenerator;
    
    [Header("References")]
    [SerializeField] private RectTransform _controllerViewportRect;
    [SerializeField] private RectTransform _panContainerRect;
    [SerializeField] private RectTransform _zoomContainerRect;
    [SerializeField] private RectTransform _visualContentRect;

    // Panning settings, from preferences.
    private float _panSpeed = 1f;
    private bool _isPanning;

    // Zooming settings, from preferences.
    private float _zoomSpeed = 0.0505f;
    private float _zoomMin = 0.3f;
    private float _zoomMax = 2f;
    private float _zoomPercentage = 0f;
    private bool _isInverted = false;
    private bool _clampViewportInsideContent = false;

    // Input Handler
    private InputHandler _inputHandler => InputHandler.Instance;
    private bool _isRegistered = false;


    // ----------------- Functions -----------------


    #region Setter Functions

    public void SetZoomPercentage(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);
        float targetScale = Mathf.Lerp(_zoomMin, _zoomMax, percentage);

        ApplyZoom(_zoomContainerRect.localScale.x, targetScale);
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
        _inputHandler.OnMiddleMouseStarted += HandleStartedPanning;
        _inputHandler.OnMiddleMouseCanceled += HandleCanceledPanning;
        _inputHandler.OnMouseDeltaPerformed += HandlePerformedPanning;
        _inputHandler.OnScrollWheelDeltaPerformed += HandleZooming;
        _preferencesManager.OnPreferencesChanged += HandleSettingsChanged;
        _nodeStackGenerator.OnGeneratedNodeStack += HandleChangedContentSize;
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
        _inputHandler.OnMiddleMouseStarted -= HandleStartedPanning;
        _inputHandler.OnMiddleMouseCanceled -= HandleCanceledPanning;
        _inputHandler.OnMouseDeltaPerformed -= HandlePerformedPanning;
        _inputHandler.OnScrollWheelDeltaPerformed -= HandleZooming;
        _nodeStackGenerator.OnGeneratedNodeStack -= HandleChangedContentSize;
    }

    #endregion

    #region Zooming Functions

    private void HandleZooming(Vector2 scrollDelta)
    {
        if (_isInverted)
            scrollDelta *= -1;

        float scroll = scrollDelta.y;
        if (Mathf.Approximately(scroll, 0f)) 
            return;

        float oldScale = _zoomContainerRect.localScale.x;
        float newScale = Mathf.Clamp(oldScale + scroll * _zoomSpeed, _zoomMin, _zoomMax);

        if (Mathf.Approximately(oldScale, newScale)) 
            return;

        ApplyZoom(oldScale, newScale);
    }

    private void ApplyZoom(float oldScale, float newScale)
    {
        Vector2 oldPanPos = _panContainerRect.anchoredPosition;

        // Compute normalized pan percentage relative to content size
        Vector2 contentSize = _visualContentRect.rect.size;
        Vector2 normPan = new Vector2(
            (oldPanPos.x) / (contentSize.x * oldScale),
            (oldPanPos.y) / (contentSize.y * oldScale)
        );

        // Apply new scale
        _zoomContainerRect.localScale = Vector3.one * newScale;

        // Update zoom percentage
        _zoomPercentage = (newScale - _zoomMin) / (_zoomMax - _zoomMin);

        // Apply normalized pan to new scale
        Vector2 newPanPos = new Vector2(
            normPan.x * contentSize.x * newScale,
            normPan.y * contentSize.y * newScale
        );

        _panContainerRect.anchoredPosition = newPanPos;

        ClampViewInsideContent();
    }

    #endregion

    #region Panning Functions

    private void HandleStartedPanning() => _isPanning = true;
    private void HandleCanceledPanning() => _isPanning = false;
    private void HandlePerformedPanning(Vector2 delta)
    {
        if (!_isPanning) return;

        _panContainerRect.anchoredPosition += delta * _panSpeed;
        ClampViewInsideContent();
    }

    #endregion

    #region Changed Functions

    private void HandleSettingsChanged(PreferencesData preferencesData)
    {
        _isInverted = preferencesData.invertZooming;
        _zoomSpeed = 0.1f * (1 + preferencesData.zoomingSpeed) / 20f;
        _panSpeed = (1f + preferencesData.panningSpeed) / 10f;

        ClampViewInsideContent();
    }

    private void HandleChangedContentSize()
    {
        if (!IsViewCenterInsideContent())
            CenterOnContentRaw();
    }

    #endregion

    #region View Functions

    private void ClampViewInsideContent()
    {
        if (_visualContentRect == null || _controllerViewportRect == null) return;

        Vector2 contentSize = _visualContentRect.rect.size * _zoomContainerRect.localScale.x;
        Vector2 viewSize = _controllerViewportRect.rect.size;

        float halfContentX = contentSize.x * 0.5f;
        float halfContentY = contentSize.y * 0.5f;

        float halfViewX = viewSize.x * 0.5f;
        float halfViewY = viewSize.y * 0.5f;

        Vector2 pos = _panContainerRect.anchoredPosition;

        if (_clampViewportInsideContent)
        {
            // Mode B: viewport fully inside content
            if (halfContentX <= halfViewX)
                pos.x = 0f;
            else
                pos.x = Mathf.Clamp(pos.x,
                    -(halfContentX - halfViewX),
                     (halfContentX - halfViewX));

            if (halfContentY <= halfViewY)
                pos.y = 0f;
            else
                pos.y = Mathf.Clamp(pos.y,
                    -(halfContentY - halfViewY),
                     (halfContentY - halfViewY));
        }
        else
        {
            // Mode A: center can reach content corners
            pos.x = Mathf.Clamp(pos.x, -halfContentX, halfContentX);
            pos.y = Mathf.Clamp(pos.y, -halfContentY, halfContentY);
        }

        _panContainerRect.anchoredPosition = pos;
    }

    public bool IsViewCenterInsideContent()
    {
        if (_visualContentRect == null || _zoomContainerRect == null)
            return false;

        Vector2 contentSize = _visualContentRect.rect.size * _zoomContainerRect.localScale.x;

        float halfContentX = contentSize.x * 0.5f;
        float halfContentY = contentSize.y * 0.5f;

        Vector2 panCenter = _panContainerRect.anchoredPosition;

        bool insideX = panCenter.x >= -halfContentX && panCenter.x <= halfContentX;
        bool insideY = panCenter.y >= -halfContentY && panCenter.y <= halfContentY;

        return insideX && insideY;
    }

    public void CenterOnContentRaw()
    {
        if (_panContainerRect == null || _zoomContainerRect == null || _visualContentRect == null)
            return;

        Vector2 contentSize = _visualContentRect.rect.size;
        float scale = _zoomContainerRect.localScale.x;

        // Explicit center of content in its own local space
        Vector2 contentCenterLocal = new Vector2(
            (0.5f - _visualContentRect.pivot.x) * contentSize.x,
            (0.5f - _visualContentRect.pivot.y) * contentSize.y
        );

        // Apply zoom scale
        contentCenterLocal *= scale;

        // Pan so content center aligns with panContainer pivot
        Vector2 panPos = -contentCenterLocal;

        _panContainerRect.anchoredPosition = panPos;
    }

    #endregion

}
