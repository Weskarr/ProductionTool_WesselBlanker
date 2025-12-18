
using UnityEngine;
using System;

public class HelpManager : MonoBehaviour
{
    [Header("UI Input")]
    [SerializeField] private HelpTab _helpTab;


    // ----------------- Functions -----------------


    #region OnEnable Functions

    private void OnEnable()
    {
        _helpTab.OnCloseTabTriggered += HandleCloseTabTriggered;
    }

    #endregion

    #region OnDisable Functions

    private void OnDisable()
    {
        _helpTab.OnCloseTabTriggered -= HandleCloseTabTriggered;
    }

    #endregion

    #region Relay Functions

    public void ToggleTabRelay(bool direction) => _helpTab.ToggleTab(direction);
    public bool GetTabStateRelay() => _helpTab.GetTabState();

    #endregion

    #region Handle Functions

    private void HandleCloseTabTriggered()
    {
        Debug.Log("Close Tab Triggered");
        // Implement logic to handle tab closing

        ToggleTabRelay(false);
    }

    #endregion

}
