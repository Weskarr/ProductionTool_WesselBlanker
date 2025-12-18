using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeStackSystem
{
    public class NodeStackGenerator : MonoBehaviour
    {
        [Header("Output Location")]
        [SerializeField] private RectTransform _visualContent;

        [Header("Prefabs")]
        [SerializeField] private GameObject _visualNodePrefab;

        // Currents
        private Dictionary<Transform, UINode> _currentNodeVisuals = new();
        private Dictionary<Transform, float> _currentNodeWidths = new();

        // Events
        public event Action<Bounds, float> OnBoundsCalculated;
        public event Action OnGeneratedNodeStack;


        // ----------------- Functions -----------------


        #region Command Functions

        public void GenerateNodeStackVisuals(NodeStackStyleHolder nodeStackStyleHolder, NodeStackLayoutHolder nodeStackLayoutHolder)
        {
            // From Layout holder
            Transform rootLayoutNode = nodeStackLayoutHolder.CurrentRootLayoutNode;
            Transform rootLayoutNodeParent = nodeStackLayoutHolder.LayoutNodeContainer;
            //GameObject prefab = nodeStackLayoutHolder.LayoutNodePrefab;

            // From Style Holder
            NodeStackStyleData style = nodeStackStyleHolder.NodeStackStyle;

            // Cleanslate
            ClearContainer();
            _currentNodeVisuals.Clear();
            _currentNodeWidths.Clear();

            // Calculated Variables
            bool isBaseOrName = !style.NodesNameWidth;
            float extraWidth = style.NodesExtraWidth * 20f;
            float extraHeight = style.NodesExtraHeight * 10f;
            float linesWidth = style.NodesLineWidth * 2;
            float baseHeight = nodeStackStyleHolder.BaseHeight + extraHeight;
            float baseWidth = nodeStackStyleHolder.BaseWidth;
            float vSpacing = style.NodesVerticalSpacing * 5;
            float hSpacing = style.NodesHorizontalSpacing * 5;
            float padding = style.NodeStackPadding * 5;

            // Node Spawning
            SpawnRecursive(rootLayoutNode, _visualNodePrefab, _visualContent, baseHeight, linesWidth, vSpacing);
            _currentNodeVisuals[rootLayoutNode].DependencyLineSetup(0, 0); // Don't Show Line on root!

            // Widths Calculations
            WidthRecursive(rootLayoutNode, baseWidth, hSpacing, extraWidth, isBaseOrName);

            // Positions Calculations
            PositionRecursive(rootLayoutNode, 0, 0, baseHeight, vSpacing, hSpacing, padding);

            // Bounds Calculations
            OnBoundsCalculated?.Invoke(CalculateNodeStackBounds(rootLayoutNode, padding), padding);
            OnGeneratedNodeStack?.Invoke();
        }

        #endregion

        #region Node Removal Functions

        public void ClearContainer()
        {
            if (_visualContent.childCount == 0)
                return;

            foreach (Transform child in _visualContent)
            {
                Destroy(child.gameObject);
            }
        }

        #endregion

        #region Node Spawning Functions

        private void SpawnRecursive(Transform layoutNode, GameObject prefab, Transform parent, float height, float lineWidth, float vSpacing)
        {
            GameObject newPrefab = Instantiate(prefab, parent);
            UINode visualData = newPrefab.GetComponent<UINode>();
            visualData.MainRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            LayoutNodeData layoutNodeData = layoutNode.GetComponent<LayoutNodeDataHolder>().LayoutNodeData;
            visualData.LayoutNodeData = layoutNodeData;
            visualData.LayoutNodeTransform = layoutNode;
            visualData.LabelText.text = layoutNodeData.name;
            visualData.DependencyLineSetup(lineWidth, vSpacing);
            _currentNodeVisuals[layoutNode] = newPrefab.GetComponent<UINode>();

            // Recursive Logic
            foreach (Transform subNode in layoutNode)
                SpawnRecursive(subNode, prefab, parent, height, lineWidth, vSpacing);
        }

        #endregion

        #region Widths Calculations Functions

        private void WidthRecursive(Transform node, float baseWidth, float hSpacing, float extraWidth, bool isBaseOrName)
        {
            // Recursive Logic
            foreach (Transform subNode in node)
                WidthRecursive(subNode, baseWidth, hSpacing, extraWidth, isBaseOrName);

            _currentNodeWidths[node] = CalculateWidth(node, baseWidth, hSpacing, extraWidth, isBaseOrName);
        }

        float CalculateWidth(Transform node, float baseWidth, float hSpacing, float extraWidth, bool isBaseOrName)
        {
            // My Width Option A
            float myWidth = CalculateNodeWidth(node, baseWidth, extraWidth, isBaseOrName);

            // My Children Width & Horizontal Combined Option B
            float myTotalChildrenWidth = CalculateTotalChildrenWidth(node);
            float myTotalChildrenSpacing = CalculateTotalChildrenSpacing(node, hSpacing);
            float myTotalChildrenCombined = myTotalChildrenWidth + myTotalChildrenSpacing;

            // Pick the Largest Option between A & B
            return Mathf.Max(myWidth, myTotalChildrenCombined);
        }

        private float CalculateNodeWidth(Transform node, float baseWidth, float extraWidth, bool isBaseOrName)
        {
            float width = isBaseOrName ? baseWidth : _currentNodeVisuals[node].LabelText.preferredWidth;
            width += extraWidth;
            return width;
        }

        private float CalculateTotalChildrenWidth(Transform node)
        {
            float width = 0f;
            if (node.childCount > 0)
            {
                // Sum child subtree widths
                foreach (Transform subNode in node)
                    width += _currentNodeWidths[subNode];
            }
            return width;
        }

        private float CalculateTotalChildrenSpacing(Transform node, float hSpacing)
        {
            float spacing = 0f;
            if (node.childCount > 1)
            {
                spacing = (node.childCount - 1) * hSpacing;
            }
            return spacing;
        }

        #endregion

        #region Positions Calculations Functions

        void PositionRecursive(Transform node, float offset, int depth, float height, float vSpacing, float hSpacing, float padding)
        {
            // Get Node Variables
            RectTransform nodeRect = _currentNodeVisuals[node].MainRect;
            float width = _currentNodeWidths[node];

            // Position Node
            nodeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            nodeRect.anchoredPosition = new Vector2(offset + (padding), depth * (height + vSpacing) + (padding));

            // Offsets Children Recursive
            float layerOffset = offset;
            foreach (Transform subNode in node)
            {
                float subNodeWidth = _currentNodeWidths[subNode];
                PositionRecursive(subNode, layerOffset, depth + 1, height, vSpacing, hSpacing, padding);
                layerOffset += subNodeWidth + hSpacing;
            }
        }

        #endregion

        #region Bounds Calculations Functions

        public Bounds CalculateNodeStackBounds(Transform root, float padding)
        {
            Vector2 min = new(float.MaxValue, float.MaxValue);
            Vector2 max = new(float.MinValue, float.MinValue);

            TraverseBounds(root, ref min, ref max);

            min -= new Vector2(padding, padding);
            max += new Vector2(padding, padding);

            Vector2 size = max - min;
            Vector2 center = min + size * 0.5f;

            return new Bounds(center, size);
        }

        void TraverseBounds(Transform t, ref Vector2 min, ref Vector2 max)
        {
            RectTransform r = _currentNodeVisuals[t].MainRect;
            Vector2 bl = r.anchoredPosition;
            Vector2 tr = bl + r.rect.size;

            min = Vector2.Min(min, bl);
            max = Vector2.Max(max, tr);

            foreach (Transform child in t)
                TraverseBounds(child, ref min, ref max);
        }

        #endregion
    }
}