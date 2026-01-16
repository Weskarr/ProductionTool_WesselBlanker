
using UnityEngine;
using System;
using Unity.VisualScripting;

public class TabBar : MonoBehaviour
{
    // References
    public RectTransform _tabButtonsRectTransform;
    public RectTransform _tabsParent;

    // Tabs Availability
    public GameObject[] _tabButtonsToActive_ForDesktop;
    public GameObject[] _tabButtonsToActive_ForWebGL;

    // Events for Manager
    public event Action OnAnyTabClosed;

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
        if (_tabButtonsRectTransform != null)
            _tabButtonsRectTransform.gameObject.SetActive(active);
        
        foreach (GameObject item in array)
            item.SetActive(active);
    }

    private void OnEnable()
    {
        foreach(Transform tab in _tabsParent)
        {
            tab.GetComponent<ITab>().OnCloseTabTriggered += OnCloseTabTriggered;
        }
    }

    private void OnDisable()
    {
        foreach (Transform tab in _tabsParent)
        {
            tab.GetComponent<ITab>().OnCloseTabTriggered -= OnCloseTabTriggered;
        }
    }

    private void OnCloseTabTriggered() => OnAnyTabClosed?.Invoke();

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
