using UnityEngine;

public class LayoutNodeDataHolder : MonoBehaviour
{
    [SerializeField] private LayoutNodeData _layoutNodeData;

    public LayoutNodeData LayoutNodeData
    {
        get => _layoutNodeData;
        set => _layoutNodeData = value;
    }
}
