using System.Runtime.Serialization;

namespace GraphvizWpf.Renderer;

#pragma warning disable CS0649

[DataContract]
internal class DotJsonDrawCommand
{
    [DataMember] public string op;
    [DataMember] public string grad;
    [DataMember] public string color;
    [DataMember] public double[][] points;
    [DataMember] public double[] pt;
    [DataMember] public string align;
    [DataMember] public double width;
    [DataMember] public string text;
    [DataMember] public uint fontchar;
    [DataMember] public double size;
    [DataMember] public string face;
    [DataMember] public string style;
    [DataMember] public double[] rect;
}

[DataContract]
internal class DotJsonClusterOrVertex
{
    [DataMember] public string name;
    [DataMember] public string _gvid;
    [DataMember] public string label;
    [DataMember] public string comment;
    [DataMember] public DotJsonDrawCommand[] _draw_;
    [DataMember] public DotJsonDrawCommand[] _ldraw_;
    [DataMember] public string[] nodes;
    [DataMember] public string[] edges;
}

[DataContract]
internal class DotJsonEdge
{
    [DataMember] public string _gvid;
    [DataMember] public string name;
    [DataMember] public string label;
    [DataMember] public string comment;
    [DataMember] public string tail;
    [DataMember] public string head;
    [DataMember] public DotJsonDrawCommand[] _draw_;
    [DataMember] public DotJsonDrawCommand[] _ldraw_;
    [DataMember] public DotJsonDrawCommand[] _hdraw_;
    [DataMember] public DotJsonDrawCommand[] _hldraw_;
    [DataMember] public DotJsonDrawCommand[] _tldraw_;
}

[DataContract]
internal class DotJsonGraph
{
    [DataMember] public string name;
    [DataMember] public string comment;
    [DataMember] public string bb;
    [DataMember] public DotJsonDrawCommand[] _draw_;
    [DataMember] public DotJsonClusterOrVertex[] objects;
    [DataMember] public DotJsonEdge[] edges;
}
