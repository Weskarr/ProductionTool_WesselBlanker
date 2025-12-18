using UnityEngine;

// Bad: ToolMaster -> Here
using SaveAndLoadSystem;

public class TabBarManager : MonoBehaviour
{
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
        _toolbarVisual.OnHelpTriggered += HandleHelp;
        _toolbarVisual.OnPreferencesTriggered += HandlePreferences;
        _toolbarVisual.OnStylisationTriggered += HandleStylisation;
        _toolbarVisual.OnSaveAndLoadTriggered += HandleSaveAndLoad;
        _toolbarVisual.OnExportTriggered += HandleExport;
        _toolbarVisual.OnQuitTriggered += HandleQuit;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        _toolbarVisual.OnHelpTriggered -= HandleHelp;
        _toolbarVisual.OnPreferencesTriggered -= HandlePreferences;
        _toolbarVisual.OnStylisationTriggered -= HandleStylisation;
        _toolbarVisual.OnSaveAndLoadTriggered -= HandleSaveAndLoad;
        _toolbarVisual.OnExportTriggered -= HandleExport;
        _toolbarVisual.OnQuitTriggered -= HandleQuit;
    }

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

    private void HandleExport()
    {
        //Debug.Log("Export action triggered.");
        // Implement export functionality here
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
    }

    #endregion

}
