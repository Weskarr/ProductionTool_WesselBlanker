using UnityEngine;

// Bad: ToolMaster -> Here
using SaveAndLoadSystem;

namespace NodeStackSystem
{
    public class NodeStackManager : MonoBehaviour
    {
        // Bad: ToolMaster -> Here
        [Header("Other Managers")]
        [SerializeField] private SaveLoader _saveLoader;
        [SerializeField] private SaveDirectLoader _saveDirectLoader;
        [SerializeField] private SaveDirectReset _saveDirectReset;
        [SerializeField] private StylisationManager _stylisationManager;

        [Header("Subsystems")]
        [SerializeField] private NodeStackStyleHolder _nodeStackStyleHolder;
        [SerializeField] private NodeStackLayoutHolder _nodeStackLayoutHolder;
        [SerializeField] private NodeStackGenerator _nodeStackGenerator;
        [SerializeField] private NodeStackEditor _nodeStackEditor;

        // Bad: Not my decision to make..
        [Header("Settings")]
        [SerializeField] private bool _startWithGeneratingLayout = false;


        // ----------------- Functions -----------------


        #region Getter Functions

        public NodeStackStyleData GetCurrentNodeStackStyle() => _nodeStackStyleHolder.NodeStackStyle;

        public string GetCurrentLayoutNodeDataOrder() => _nodeStackLayoutHolder.CreateLayoutNodeDataOrder();

        public LayoutNodeData[] GetCurrentLayoutNodeDataArray() => _nodeStackLayoutHolder.CreateLayoutNodeDataArray();

        #endregion

        #region OnEnable Functions

        private void OnEnable()
        {
            _saveLoader.OnSaveLoadedSuccesfully += LoadNodeStack;
            _saveDirectLoader.OnSaveLoadedSuccesfully += LoadNodeStack;
            _saveDirectReset.OnSaveReset += LoadNodeStack;
            _stylisationManager.OnNodeStackStyleChange += GenerateNodeStackVisualsRelay;
            _nodeStackEditor.OnNodeStackLayoutEdited += GenerateNodeStackVisualsRelay;

            if (_startWithGeneratingLayout)
                GenerateNodeStackVisualsRelay();
        }

        #endregion

        #region OnDisable Functions
        private void OnDisable()
        {
            _saveLoader.OnSaveLoadedSuccesfully -= LoadNodeStack;
            _saveDirectLoader.OnSaveLoadedSuccesfully -= LoadNodeStack;
            _saveDirectReset.OnSaveReset -= LoadNodeStack;
            _stylisationManager.OnNodeStackStyleChange -= GenerateNodeStackVisualsRelay;
            _nodeStackEditor.OnNodeStackLayoutEdited -= GenerateNodeStackVisualsRelay;
        }


        #endregion

        #region Loading Functions

        private void LoadNodeStack(NodeStackData data)
        {
            LoadStyleRelay(data);
            LoadLayoutRelay(data);
            GenerateNodeStackVisualsRelay();
        }

        private void LoadLayoutRelay(NodeStackData data) => _nodeStackLayoutHolder.LoadLayout(data);

        private void LoadStyleRelay(NodeStackData data) => _nodeStackStyleHolder.LoadStyle(data);

        #endregion

        #region Generation Functions

        public void GenerateNodeStackVisualsRelay()
        {
            _nodeStackGenerator.GenerateNodeStackVisuals(_nodeStackStyleHolder, _nodeStackLayoutHolder);
        }

        #endregion

    }
}