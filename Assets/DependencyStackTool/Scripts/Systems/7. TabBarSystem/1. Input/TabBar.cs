
using UnityEngine;
using System;

public class TabBar : MonoBehaviour
{
    // References
    public RectTransform _tabsRectTransform;

    // Tabs Availability
    public GameObject[] _tabButtonsToActive_ForDesktop;
    public GameObject[] _tabButtonsToActive_ForWebGL;

    // Events
    public event Action OnHelpTriggered;
    public event Action OnPreferencesTriggered;
    public event Action OnStylisationTriggered;
    public event Action OnSaveAndLoadTriggered;
    public event Action OnQuitTriggered;

    // Events addition for WebGL
    public event Action OnDirectSaveTriggered;
    public event Action OnDirectLoadTriggered;
    public event Action OnDirectResetTriggered;


    // ----------------- Functions -----------------


    #region Setup Functions

    public void ActivateTabBarWithButtonsForDesktop(bool active) => ActivateGameobjectArray(_tabButtonsToActive_ForDesktop, active);

    public void ActivateTabBarWithButtonsForWebGL(bool active) => ActivateGameobjectArray(_tabButtonsToActive_ForWebGL, active);

    private void ActivateGameobjectArray(GameObject[] array, bool active)
    {
        if (_tabsRectTransform != null)
            _tabsRectTransform.gameObject.SetActive(active);
        
        foreach (GameObject item in array)
            item.SetActive(active);
    }

    #endregion

    #region Trigger Functions

    public void HelpTriggered() => OnHelpTriggered?.Invoke();
    public void PreferencesTriggered() => OnPreferencesTriggered?.Invoke();
    public void StylisationTriggered() => OnStylisationTriggered?.Invoke();
    public void SaveAndLoadTriggered() => OnSaveAndLoadTriggered?.Invoke();
    public void QuitTriggered() => OnQuitTriggered?.Invoke();

    // Events addition for WebGL
    public void DirectSaveTriggered() => OnDirectSaveTriggered?.Invoke();
    public void DirectLoadTriggered() => OnDirectLoadTriggered?.Invoke();
    public void DirectResetTriggered() => OnDirectResetTriggered?.Invoke();

    #endregion

}
