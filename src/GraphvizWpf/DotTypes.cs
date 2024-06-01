namespace GraphvizWpf;

public enum DotLayoutEngine
{
    dot,
    neato,
    circo,
    fdp,
    osage,
    twopi
}

public class DotCluster
{
    public string Name { get; }
    public string Comment { get; }

    public DotCluster(string name, string comment)
    {
        Name = name;
        Comment = comment;
    }
}

public class DotVertex
{
    public string Name { get; }
    public string Comment { get; }

    public DotVertex(string name, string comment)
    {
        Name = name;
        Comment = comment;
    }
}

public class DotEdge
{
    public string Name { get; }
    public string Comment { get; }

    public DotEdge(string name, string comment)
    {
        Name = name;
        Comment = comment;
    }
}
