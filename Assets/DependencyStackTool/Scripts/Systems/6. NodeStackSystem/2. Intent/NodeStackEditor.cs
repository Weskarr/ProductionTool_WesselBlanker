
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
        [SerializeField] private NodeNamer _nodeNamer;
        [SerializeField] private NodeSwapper _nodeSwapper;
        [SerializeField] private NodeReparenter _nodeReparenter;

        // Events
        public event Action OnNodeStackLayoutEdited;


        // ----------------- Functions -----------------


        #region OnEnable Functions

        private void OnEnable()
        {
            _nodeNamer.OnNodeNameEditedTriggered += HandleNodeNameEdited;
            _nodeSwapper.OnNodeSwappedTriggered += HandleNodeSwapped;
            _nodeReparenter.OnNodeReparentedTriggered += HandleNodeReparented;

            _contextMenu.OnAddSubNodeTriggered += HandleAddSubNode;
            _contextMenu.OnInsertNodeTriggered += HandleInsertNode;
            _contextMenu.OnDestroySingleTriggered += HandleDestroySingle;
            _contextMenu.OnDestroyBranchTriggered += HandleDestroyBranch;
        }

        #endregion

        #region OnDisable Functions

        private void OnDisable()
        {
            _nodeNamer.OnNodeNameEditedTriggered -= HandleNodeNameEdited;
            _nodeSwapper.OnNodeSwappedTriggered -= HandleNodeSwapped;
            _nodeReparenter.OnNodeReparentedTriggered -= HandleNodeReparented;

            _contextMenu.OnAddSubNodeTriggered -= HandleAddSubNode;
            _contextMenu.OnInsertNodeTriggered -= HandleInsertNode;
            _contextMenu.OnDestroySingleTriggered -= HandleDestroySingle;
            _contextMenu.OnDestroyBranchTriggered -= HandleDestroyBranch;
        }

        #endregion

        #region Editing Functions

        public void HandleNodeNameEdited(UINode node)
        {
            OnNodeStackLayoutEdited?.Invoke();
        }

        public void HandleNodeSwapped(Transform layoutNodeOne, Transform layoutNodeTwo)
        {
            if (layoutNodeOne == null || layoutNodeTwo == null)
                return;

            if (layoutNodeOne == layoutNodeTwo)
                return;

            // Get the needed data.
            LayoutNodeDataHolder holderOne = layoutNodeOne.GetComponent<LayoutNodeDataHolder>();
            LayoutNodeDataHolder holderTwo = layoutNodeTwo.GetComponent<LayoutNodeDataHolder>();
            LayoutNodeData dataOne = holderOne.LayoutNodeData;
            LayoutNodeData dataTwo = holderTwo.LayoutNodeData;

            // Exchange root ID.
            if (dataOne.id == 0)
            {
                dataOne.id = 999;
                dataTwo.id = 0;
            }
            else if (dataTwo.id == 0)
            {
                dataOne.id = 0;
                dataTwo.id = 999;
            }

            // Now Swap them.
            holderOne.LayoutNodeData = dataTwo;
            holderTwo.LayoutNodeData = dataOne;

            OnNodeStackLayoutEdited?.Invoke();
        }

        public void HandleNodeReparented(Transform layoutNodeOne, Transform layoutNodeTwo)
        {
            if (layoutNodeOne == null || layoutNodeTwo == null)
                return;

            if (layoutNodeOne == layoutNodeTwo)
                return;

            layoutNodeOne.parent = layoutNodeTwo;

            OnNodeStackLayoutEdited?.Invoke();
        }

        #endregion

        #region Addition Functions

        public void HandleAddSubNode(Transform layoutNode)
        {
            if (layoutNode == null)
                return;

            GameObject newLayoutNode = Instantiate(_nodeStackLayoutHolder.LayoutNodePrefab, layoutNode);
            LayoutNodeData data = newLayoutNode.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;
            data.name = "SubNode";
            data.id = 999;

            OnNodeStackLayoutEdited?.Invoke();
        }

        public void HandleInsertNode(Transform layoutNode)
        {
            Debug.Log($"HandleInsertNode - Start: {layoutNode}");

            if (layoutNode == null)
            {
                Debug.LogWarning("LayoutNode is null!");
                return;
            }


            Transform parent = layoutNode.parent;
            LayoutNodeData dataSelected = layoutNode.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;
            
            if (dataSelected == null)
            {
                Debug.LogWarning("LayoutNodeData is null!");
            }

            GameObject newLayoutNode = Instantiate(_nodeStackLayoutHolder.LayoutNodePrefab, parent);
            LayoutNodeData dataNew = newLayoutNode.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;
            dataNew.name = "NewNode";
            dataNew.id = 999;

            // Set it 0 if new root!
            if (dataSelected.id == 0)
            {
                dataSelected.id = 999;
                dataNew.id = 0;
            }

            layoutNode.SetParent(newLayoutNode.transform);

            Debug.Log($"HandleInsertNode - End: {layoutNode}");

            OnNodeStackLayoutEdited?.Invoke();
        }

        #endregion

        #region Destroy Functions

        public void HandleDestroySingle(Transform layoutNode)
        {
            Transform parent = layoutNode.parent;
            LayoutNodeData dataSelected = layoutNode.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;

            if (dataSelected.id == 0)
            {
                Debug.Log("OnlyChild");
                Transform onlyChild = layoutNode.GetChild(0);
                LayoutNodeData dataChild = onlyChild.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;
                dataChild.id = 0;
                onlyChild.SetParent(parent);
            }
            else
            {
                Debug.Log("MultiChild");

                int childCount = layoutNode.childCount;
                Transform[] children = new Transform[childCount];

                for (int i = 0; i < childCount; i++)
                    children[i] = layoutNode.GetChild(i);

                foreach (Transform child in children)
                    child.SetParent(parent);
            }

            DestroyImmediate(layoutNode.gameObject);
            OnNodeStackLayoutEdited?.Invoke();
        }

        public void HandleDestroyBranch(Transform layoutNode)
        {
            DestroyImmediate(layoutNode.gameObject);
            OnNodeStackLayoutEdited?.Invoke();
        }

        #endregion

    }
}