using UnityEngine;
using TMPro;

public class UINode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _mainRect;
    [SerializeField] private RectTransform _dependencyLineRect;

    [Header("Visual Text")]
    [SerializeField] private TextMeshProUGUI _labelText;

    // Layout Reference
    [SerializeField] private Transform _layoutNodeTransform;
    [SerializeField] private LayoutNodeData _layoutNodeData;


    // ----------------- Functions -----------------


    #region Getter & Setter Functions

    public RectTransform MainRect
    {
        get => _mainRect;
        set => _mainRect = value;
    }

    public RectTransform DependencyLineRect
    {
        get => _dependencyLineRect;
        set => _dependencyLineRect = value;
    }

    public TextMeshProUGUI LabelText
    {
        get => _labelText;
        set => _labelText = value;
    }

    public Transform LayoutNodeTransform
    {
        get => _layoutNodeTransform;
        set => _layoutNodeTransform = value;
    }

    public LayoutNodeData LayoutNodeData
    {
        get => _layoutNodeData;
        set => _layoutNodeData = value;
    }

    #endregion

    #region Setup Functions

    public void DependencyLineSetup(float lineWidth, float lineHeight)
    {
        ApplyRectSize(_dependencyLineRect, lineWidth, lineHeight);
    }

    public void ApplyRectSize(RectTransform rect, float width, float height)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void ApplyRectPadding(RectTransform rect, float padding)
    {
        rect.offsetMin = new Vector2(padding, padding);
        rect.offsetMax = new Vector2(-padding, -padding);
    }

    #endregion

}
