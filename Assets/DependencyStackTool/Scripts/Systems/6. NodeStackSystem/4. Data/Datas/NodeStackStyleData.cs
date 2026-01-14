using System;

[Serializable]
public class NodeStackStyleData
{
    public bool NodesNameWidth = true;
    public int NodesExtraWidth = 10;
    public int NodesExtraHeight = 10;
    public int NodesLineWidth = 10;
    public int NodesHorizontalSpacing = 10;
    public int NodesVerticalSpacing = 10;
    public int NodeStackPadding = 10;

    public void Reset()
    {
        NodesNameWidth = true;
        NodesExtraWidth = 10;
        NodesExtraHeight = 10;
        NodesLineWidth = 10;
        NodesHorizontalSpacing = 10;
        NodesVerticalSpacing = 10;
        NodeStackPadding = 10;
    }
}
