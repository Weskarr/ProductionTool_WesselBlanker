
using UnityEngine;
using System;

namespace NodeStackSystem
{
    public class NodeStackEditor : MonoBehaviour
    {
        // Bad: NodeStackManager -> Here
        [Header("Other Systems")]
        [SerializeField] private NodeStackLayoutHolder _nodeStackLayoutHolder;
        [SerializeField] private ContextMenu _contextMenu;
        [SerializeField] private NodeMenu _nodeMenu;

        // Events
        public event Action OnNodeStackLayoutEdited;


        // ----------------- Functions -----------------


        #region OnEnable Functions

        private void OnEnable()
        {
            _nodeMenu.OnNodeEditedTriggered += context => HandleEditedNode(context);
            _contextMenu.OnAddSubNodeTriggered += context => HandleAddSubNode(context);
            _contextMenu.OnDestroyNodeTriggered += context => HandleDestroyNode(context);
        }

        #endregion

        #region OnDisable Functions

        private void OnDisable()
        {
            _nodeMenu.OnNodeEditedTriggered -= context => HandleEditedNode(context);
            _contextMenu.OnAddSubNodeTriggered -= context => HandleAddSubNode(context);
            _contextMenu.OnDestroyNodeTriggered += context => HandleDestroyNode(context);
        }

        #endregion

        #region Add SubNode Functions

        public void HandleAddSubNode(Transform layoutNodeParent)
        {
            if (layoutNodeParent == null)
                return;

            GameObject newLayoutNode = Instantiate(_nodeStackLayoutHolder.LayoutNodePrefab, layoutNodeParent);
            LayoutNodeData data = newLayoutNode.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;
            data.name = "SubNode";
            data.id = 999;

            OnNodeStackLayoutEdited?.Invoke();
        }

        #endregion

        #region Edit Node Functions

        public void HandleEditedNode(UINode node)
        {
            OnNodeStackLayoutEdited?.Invoke();
        }

        #endregion

        #region Destroy Node Functions

        public void HandleDestroyNode(Transform nodeToDestroy)
        {
            DestroyImmediate(nodeToDestroy.gameObject);
            OnNodeStackLayoutEdited?.Invoke();
        }

        #endregion

    }
}