using UnityEngine;

// Bad: ToolMaster -> Here
using SaveAndLoadSystem;

public class TabBarManager : MonoBehaviour
{
    // Bad: ToolMaster -> Here
    [SerializeField] private MovementController _movementController;

    // Bad: ToolMaster -> Here
    [Header("Other Managers")]
    [SerializeField] private HelpManager _helpManager;
    [SerializeField] private PreferencesManager _preferencesManager;
    [SerializeField] private StylisationManager _stylisationManager;
    [SerializeField] private SaveAndLoadManager _saveAndLoadManager;

    [Header("UI Input")]
    [SerializeField] private TabBar _toolbarVisual;


    // ----------------- Functions -----------------


    #region OnEnable Functions

    private void OnEnable()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            _toolbarVisual.ActivateTabBarWithButtonsForWebGL(true);
        #else
            _toolbarVisual.ActivateTabBarWithButtonsForDesktop(true);
        #endif

        _toolbarVisual.OnHelpTriggered += HandleHelp;
        _toolbarVisual.OnPreferencesTriggered += HandlePreferences;
        _toolbarVisual.OnStylisationTriggered += HandleStylisation;
        _toolbarVisual.OnSaveAndLoadTriggered += HandleSaveAndLoad;
        _toolbarVisual.OnQuitTriggered += HandleQuit;
        _toolbarVisual.OnDirectSaveTriggered += HandleDirectSaveRelay;
        _toolbarVisual.OnDirectLoadTriggered += HandleDirectLoadRelay;
        _toolbarVisual.OnDirectResetTriggered += HandleDirectResetRelay;
        _toolbarVisual.OnAnyTabClosed += HandleOnAnyTabClosed;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            _toolbarVisual.ActivateTabBarWithButtonsForWebGL(false);
        #else
            _toolbarVisual.ActivateTabBarWithButtonsForDesktop(false);
        #endif

        _toolbarVisual.OnHelpTriggered -= HandleHelp;
        _toolbarVisual.OnPreferencesTriggered -= HandlePreferences;
        _toolbarVisual.OnStylisationTriggered -= HandleStylisation;
        _toolbarVisual.OnSaveAndLoadTriggered -= HandleSaveAndLoad;
        _toolbarVisual.OnQuitTriggered -= HandleQuit;
        _toolbarVisual.OnDirectSaveTriggered -= HandleDirectSaveRelay;
        _toolbarVisual.OnDirectLoadTriggered -= HandleDirectLoadRelay;
        _toolbarVisual.OnDirectResetTriggered -= HandleDirectResetRelay;
        _toolbarVisual.OnAnyTabClosed -= HandleOnAnyTabClosed;
    }

    #endregion

    #region IsWebGL Handle Relay Functions

    private void HandleDirectSaveRelay() => _saveAndLoadManager.HandleDirectSaveFinalRelay();

    private void HandleDirectLoadRelay() => _saveAndLoadManager.HandleDirectLoadFinalRelay();

    private void HandleDirectResetRelay() => _saveAndLoadManager.HandleDirectResetFinalRelay();

    #endregion

    #region Handle Functions

    private void HandleHelp()
    {
        //Debug.Log("Help action triggered.");
        // Implement help functionality here

        // Poorly optimised!
        bool previousState = _helpManager.GetTabStateRelay();
        CloseAllTabs();
        _helpManager.ToggleTabRelay(!previousState);
    }

    private void HandlePreferences()
    {
        //Debug.Log("Preferences action triggered.");
        // Implement preferences functionality here

        // Poorly optimised!
        bool previousState = _preferencesManager.GetTabStateRelay();
        CloseAllTabs();
        _preferencesManager.ToggleTabRelay(!previousState);
    }

    private void HandleStylisation()
    {
        //Debug.Log("Stylisation action triggered.");
        // Implement stylisation functionality here

        // Poorly optimised!
        bool previousState = _stylisationManager.GetTabStateRelay();
        CloseAllTabs();
        _stylisationManager.ToggleTabRelay(!previousState);
    }

    private void HandleSaveAndLoad()
    {
        //Debug.Log("Save and Load action triggered.");
        // Implement save and load functionality here

        // Poorly optimised!
        bool previousState = _saveAndLoadManager.GetTabStateRelay();
        CloseAllTabs();
        _saveAndLoadManager.ToggleTabRelay(!previousState);
    }

    private void HandleQuit()
    {
        //Debug.Log("Quit action triggered.");
        // Implement quit functionality here

        Application.Quit();
    }

    private void CloseAllTabs()
    {
        _helpManager.ToggleTabRelay(false);
        _preferencesManager.ToggleTabRelay(false);
        _stylisationManager.ToggleTabRelay(false);
        _saveAndLoadManager.ToggleTabRelay(false);

        _movementController.enabled = false;
    }

    private void HandleOnAnyTabClosed()
    {
        _movementController.enabled = true;
    }

    #endregion

}
