using System.Collections.Generic;
using UnityEngine;

namespace NodeStackSystem
{
    public class NodeStackLayoutHolder : MonoBehaviour
    {
        [Header("Other Subsystems")]
        [SerializeField] private NodeStackStyleHolder _nodeStackStyleHolder;

        [Header("Container")]
        [SerializeField] private Transform _layoutNodeContainer;

        [Header("Prefabs")]
        [SerializeField] private GameObject _layoutNodePrefab;

        // Currents
        [SerializeField] private Transform _currentRootLayoutNode;


        // ----------------- Functions -----------------


        #region Getter & Setter Functions
        
        public Transform CurrentRootLayoutNode
        {
            get => _currentRootLayoutNode;
            set => _currentRootLayoutNode = value;
        }

        public Transform LayoutNodeContainer
        {
            get => _layoutNodeContainer;
            set => _layoutNodeContainer = value;
        }

        public GameObject LayoutNodePrefab
        {
            get => _layoutNodePrefab;
            set => _layoutNodePrefab = value;
        }

        #endregion

        #region Load Functions

        public void LoadLayout(NodeStackData data)
        {
            ClearContainer();

            Stack<Transform> parentStack = new();
            parentStack.Push(_layoutNodeContainer);

            int _currentIndex = 0;
            foreach (char c in data.NodeLayoutDataOrder)
            {
                switch (c)
                {
                    case 'F':
                        GameObject nodeObj = Instantiate(_layoutNodePrefab, parentStack.Peek());
                        LayoutNodeDataHolder holder = nodeObj.GetComponent<LayoutNodeDataHolder>();
                        holder.LayoutNodeData = data.NodeLayoutDataArray[_currentIndex];
                        nodeObj.name = holder.LayoutNodeData.name;
                        if (_currentIndex == 0)
                            _currentRootLayoutNode = nodeObj.transform;
                        _currentIndex++;
                        break;

                    case '[':
                        parentStack.Push(parentStack.Peek().GetChild(parentStack.Peek().childCount - 1));
                        break;

                    case ']':
                        if (parentStack.Count > 1)
                            parentStack.Pop();
                        break;
                }
            }
        }

        #endregion

        #region Create Functions

        public LayoutNodeData[] CreateLayoutNodeDataArray()
        {
            if (_currentRootLayoutNode == null)
                return null;

            List<LayoutNodeData> list = new();
            CollectLayoutNodeDataRecursive(_currentRootLayoutNode, list);

            // assign sequential IDs
            //for (int i = 0; i < list.Count; i++)
            //    list[i].id = i;

            return list.ToArray();
        }

        public string CreateLayoutNodeDataOrder()
        {
            if (_currentRootLayoutNode == null)
                return null;

            return SerializeNodeDataRecursive(_currentRootLayoutNode);
        }

        #endregion

        #region Clear Functions

        public void ClearContainer()
        {
            if (_layoutNodeContainer.childCount == 0)
                return;

            _currentRootLayoutNode = null;
            foreach (Transform child in _layoutNodeContainer)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion

        #region Recursive Functions

        private void CollectLayoutNodeDataRecursive(Transform node, List<LayoutNodeData> outList)
        {
            LayoutNodeDataHolder holder = node.GetComponent<LayoutNodeDataHolder>();
            if (holder != null && holder.LayoutNodeData != null)
                outList.Add(holder.LayoutNodeData);

            foreach (Transform child in node)
                CollectLayoutNodeDataRecursive(child, outList);
        }

        private string SerializeNodeDataRecursive(Transform node)
        {
            string result = "F";
            // If node has children, open bracket, serialize children, close bracket
            if (node.childCount > 0)
            {
                result += "[";
                foreach (Transform child in node)
                    result += SerializeNodeDataRecursive(child);
                result += "]";
            }
            return result;
        }

        #endregion

    }
}