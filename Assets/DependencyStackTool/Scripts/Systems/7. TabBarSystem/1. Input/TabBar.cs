
using UnityEngine;
using System;

public class TabBar : MonoBehaviour
{
    // Events
    public event Action OnHelpTriggered;
    public event Action OnPreferencesTriggered;
    public event Action OnStylisationTriggered;
    public event Action OnSaveAndLoadTriggered;
    public event Action OnExportTriggered;
    public event Action OnQuitTriggered;


    // ----------------- Functions -----------------


    #region Trigger Functions

    public void HelpTriggered() => OnHelpTriggered?.Invoke();
    public void PreferencesTriggered() => OnPreferencesTriggered?.Invoke();
    public void StylisationTriggered() => OnStylisationTriggered?.Invoke();
    public void SaveAndLoadTriggered() => OnSaveAndLoadTriggered?.Invoke();
    public void ExportTriggered() => OnExportTriggered?.Invoke();
    public void QuitTriggered() => OnQuitTriggered?.Invoke();

    #endregion

}
