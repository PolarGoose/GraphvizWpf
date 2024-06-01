using GraphvizWpf.Renderer;
using System.Windows.Controls;

namespace GraphvizWpf;

public partial class GraphvizWpfCanvas : Canvas
{
    public void DrawGraph(string graphDescriptionInDotLanguage, DotLayoutEngine layoutEngine)
    {
        Children.Clear();
        DotGraphRenderer.Render(this, graphDescriptionInDotLanguage, layoutEngine);
    }
}
