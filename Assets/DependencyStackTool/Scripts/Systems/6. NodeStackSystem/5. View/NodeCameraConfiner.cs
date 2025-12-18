using UnityEngine;

namespace NodeStackSystem
{
    public class NodeCameraConfiner : MonoBehaviour
    {
        // Bad: NodeStackManager -> Here
        [Header("Other Subsystems")]
        [SerializeField] private NodeStackGenerator generator;

        [Header("References")]
        [SerializeField] private RectTransform rectTransform;


        // ----------------- Functions -----------------


        #region OnEnable Functions

        void OnEnable()
        {
            generator.OnBoundsCalculated += UpdateConfiner;
        }

        #endregion

        #region OnDisable Functions

        private void OnDisable()
        {
            generator.OnBoundsCalculated -= UpdateConfiner;
        }

        #endregion

        #region Updating Confiner Functions

        private void UpdateConfiner(Bounds bounds, float padding)
        {
            // Calculate width/height
            float width = bounds.size.x;
            float height = bounds.size.y;

            // Update RectTransform
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        #endregion

    }
}