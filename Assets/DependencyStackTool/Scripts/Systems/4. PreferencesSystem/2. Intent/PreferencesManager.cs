
using UnityEngine;
using System;

public class PreferencesManager : MonoBehaviour
{
    [Header("UI Input")]
    [SerializeField] private PreferencesTab _preferencesTab;

    // Data
    private PreferencesData _currentPreferencesData = new();
    private bool _isDirty = false;

    // Events
    public event Action<PreferencesData> OnPreferencesChanged;


    // ----------------- Functions -----------------


    #region OnEnable Functions

    private void OnEnable()
    {
        _preferencesTab.OnCloseTabTriggered += HandleCloseTabTriggered;
        _preferencesTab.OnInvertZoomingChanged += HandleInvertZoomingChanged;
        _preferencesTab.OnZoomingSpeedChanged += HandleZoomingSpeedChanged;
        _preferencesTab.OnPanningSpeedChanged += HandlePanningSpeedChanged;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        _preferencesTab.OnCloseTabTriggered -= HandleCloseTabTriggered;
        _preferencesTab.OnInvertZoomingChanged -= HandleInvertZoomingChanged;
        _preferencesTab.OnZoomingSpeedChanged -= HandleZoomingSpeedChanged;
        _preferencesTab.OnPanningSpeedChanged -= HandlePanningSpeedChanged;
    }

    #endregion

    #region Relay Functions

    public void ToggleTabRelay(bool direction) => _preferencesTab.ToggleTab(direction);

    public bool GetTabStateRelay() => _preferencesTab.GetTabState();

    #endregion

    #region Handle Functions

    private void HandleCloseTabTriggered()
    {
        Debug.Log("Close Tab Triggered");
        // Implement logic to handle tab closing

        ToggleTabRelay(false);
        if (_isDirty)
            DirtyPreferencesData();
    }

    private void HandleInvertZoomingChanged(bool isInverted)
    {
        Debug.Log($"Invert Zooming changed to: {isInverted}");
        // Implement logic to handle invert zooming preference change

        _currentPreferencesData.invertZooming = isInverted;
        _isDirty = true;
    }

    private void HandleZoomingSpeedChanged(int speed)
    {
        Debug.Log($"Zooming Speed changed to: {speed}");
        // Implement logic to handle zooming speed preference change

        _currentPreferencesData.zoomingSpeed = speed;
        _isDirty = true;
    }

    private void HandlePanningSpeedChanged(int speed)
    {
        Debug.Log($"Panning Speed changed to: {speed}");
        // Implement logic to handle panning speed preference change

        _currentPreferencesData.panningSpeed = speed;
        _isDirty = true;
    }

    private void DirtyPreferencesData()
    {
        OnPreferencesChanged?.Invoke(_currentPreferencesData);
        _isDirty = false;
    }

    #endregion

}
