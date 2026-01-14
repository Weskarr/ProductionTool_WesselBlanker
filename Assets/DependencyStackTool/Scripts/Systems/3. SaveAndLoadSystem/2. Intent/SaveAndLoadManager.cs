
using UnityEngine;
using System;

// Bad: ToolMaster -> Here
using NodeStackSystem;

namespace SaveAndLoadSystem
{
    public class SaveAndLoadManager : MonoBehaviour
    {
        // Bad: ToolMaster -> Here
        [Header("Other Managers")]
        [SerializeField] private NodeStackManager _nodeStackManager;

        [Header("UI Input")]
        [SerializeField] private SaveAndLoadTab _saveAndLoadTab;

        [Header("Subsystems")]
        [SerializeField] private SaveReloader _saveReloader;
        [SerializeField] private SaveCreator _saveCreator;
        [SerializeField] private SaveLoader _saveLoader;
        [SerializeField] private SaveDeleter _saveDeleter;

        [Header("Subsystems For WebGL")]
        [SerializeField] private SaveDirectSaver _saveDirectSaver;
        [SerializeField] private SaveDirectLoader _saveDirectLoader;
        [SerializeField] private SaveDirectReset _saveDirectReset;


        // ----------------- Functions -----------------


        #region OnEnable Functions

        private void OnEnable()
        {
            _saveAndLoadTab.OnCloseTabTriggered += HandleCloseTabTriggered;
            _saveAndLoadTab.OnSaveTriggered += context => HandleSaveTriggered(context);
            _saveAndLoadTab.OnLoadTriggeredRelay += context => HandleLoadTriggered(context);
            _saveAndLoadTab.OnDeleteTriggeredRelay += context => HandleDeleteTriggered(context);
        }

        #endregion

        #region OnDisable Functions

        private void OnDisable()
        {
            _saveAndLoadTab.OnCloseTabTriggered -= HandleCloseTabTriggered;
            _saveAndLoadTab.OnSaveTriggered -= context => HandleSaveTriggered(context);
            _saveAndLoadTab.OnLoadTriggeredRelay -= context => HandleLoadTriggered(context);
            _saveAndLoadTab.OnDeleteTriggeredRelay -= context => HandleDeleteTriggered(context);
        }

        #endregion

        #region Handle IsWebGL Functions

        public void HandleDirectSaveFinalRelay() => _saveDirectSaver.CreateSave(_nodeStackManager);

        public void HandleDirectLoadFinalRelay() => _saveDirectLoader.RequestLoad();

        public void HandleDirectResetFinalRelay() => _saveDirectReset.ResetSave();

        #endregion

        #region Relay Functions

        public void ToggleTabRelay(bool direction)
        {
            _saveAndLoadTab.ToggleTab(direction);

            if (direction)
                RefreshSavesRelay();
        }

        public bool GetTabStateRelay() => _saveAndLoadTab.GetTabState();

        public void RefreshSavesRelay() => _saveAndLoadTab.RefreshSaveVisuals(_saveReloader.GetSaveFilesNames());

        public void LoadSaveRelay(string saveFileName) => _saveLoader.LoadSave(saveFileName);

        public void DeleteSaveRelay(string saveFileName) => _saveDeleter.DeleteSave(saveFileName);

        public void CreateSaveRelay(string saveFileName) => _saveCreator.CreateSave(_nodeStackManager, saveFileName);

        #endregion

        #region Handle Functions

        private void HandleCloseTabTriggered()
        {
            Debug.Log("Close Tab Triggered");
            // Implement logic to handle tab closing

            ToggleTabRelay(false);
        }

        private void HandleSaveTriggered(string context)
        {
            Debug.Log($"Save Triggered with context: {context}");
            // Implement logic to handle save action

            CreateSaveRelay(context);
            RefreshSavesRelay(); // Do via Deleter events later..
        }

        private void HandleLoadTriggered(string context)
        {
            Debug.Log($"Load Triggered with context: {context}");
            // Implement logic to handle load action

            LoadSaveRelay(context);
            HandleCloseTabTriggered();
        }

        private void HandleDeleteTriggered(string context)
        {
            Debug.Log($"Delete Triggered with context: {context}");
            // Implement logic to handle delete action

            DeleteSaveRelay(context);
            RefreshSavesRelay(); // Do via Deleter events later..
        }

        #endregion

    }
}