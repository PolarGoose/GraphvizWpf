using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Controls;

namespace GraphvizWpf.Renderer;

internal static class DotGraphRenderer
{
    // Returns the Height and Width of the graph
    public static void Render(Canvas canvas, string graphDescriptionInDotLanguage, DotLayoutEngine layoutEngine)
    {
        var dotGraph = (DotJsonGraph)new DataContractJsonSerializer(typeof(DotJsonGraph))
            .ReadObject(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(
                DotGraphDescriptionProcessor.Process(graphDescriptionInDotLanguage, layoutEngine, "json"))));

        var boundingBox = dotGraph.bb.Split(',').Select(p => double.Parse(p, CultureInfo.InvariantCulture)).ToList();

        // The bounding box is a string formatted as "x1,y1,x2,y2", where (x1,y1) is the lower left corner and (x2,y2) is the upper right corner.
        // In Graphviz, the coordinate system origin is the lower left corner, while in WPF Canvas, it is the upper left corner.
        // Consequently, to convert the y-coordinate from Graphviz to WPF, we need to know the maximum Y value (maxY).
        var maxX = boundingBox[2];
        var maxY = boundingBox[3];

        foreach (var vertexOrCluster in dotGraph.objects)
        {
            object dotVertex = vertexOrCluster.name.StartsWith("cluster")
                ? new DotCluster(vertexOrCluster.name, vertexOrCluster.comment)
                : new DotVertex(vertexOrCluster.name, vertexOrCluster.comment);

            new DotDrawCommandExecutor(canvas, vertexOrCluster._draw_, dotVertex, maxY).Execute();
            new DotDrawCommandExecutor(canvas, vertexOrCluster._ldraw_, dotVertex, maxY).Execute();
        }

        foreach (var edge in dotGraph.edges)
        {
            var dotEdge = new DotEdge(edge.name, edge.comment);

            new DotDrawCommandExecutor(canvas, edge._draw_, dotEdge, maxY).Execute();
            new DotDrawCommandExecutor(canvas, edge._ldraw_, dotEdge, maxY).Execute();
            new DotDrawCommandExecutor(canvas, edge._hdraw_, dotEdge, maxY).Execute();
            new DotDrawCommandExecutor(canvas, edge._hldraw_, dotEdge, maxY).Execute();
            new DotDrawCommandExecutor(canvas, edge._tldraw_, dotEdge, maxY).Execute();
        }

        canvas.Height = maxY;
        canvas.Width = maxX;
    }
}
