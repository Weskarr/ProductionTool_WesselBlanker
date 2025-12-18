
using UnityEngine;

namespace NodeStackSystem
{
    public class NodeStackStyleHolder : MonoBehaviour
    {
        [Header("Hard Data")]
        [SerializeField] private float _baseWidth;
        [SerializeField] private float _baseHeight;

        [Header("Soft Data")]
        [SerializeField] private NodeStackStyleData _nodeStackStyle;


        // ----------------- Functions -----------------


        #region Getter & Setter Functions

        public NodeStackStyleData NodeStackStyle
        {
            get => _nodeStackStyle;
            set => _nodeStackStyle = value;
        }

        public float BaseWidth
        {
            get => _baseWidth;
            set => _baseWidth = value;
        }

        public float BaseHeight
        {
            get => _baseHeight;
            set => _baseHeight = value;
        }

        #endregion

        #region Load Functions

        public void LoadStyle(NodeStackData data)
        {
            NodeStackStyle = data.NodeStackStyle;
        }

        #endregion

    }
}