using GraphvizWpf;
using GraphvizWpf.Renderer;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Test;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Initialized(object sender, EventArgs e)
    {
        var files = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}/DotFiles");
        dotFileComboBox.ItemsSource = files.Select(Path.GetFileName);
        dotFileComboBox.SelectedIndex = 0;
        dotFileComboBox.SelectionChanged += DotFileComboBox_SelectionChanged;

        layoutEngineComboBox.ItemsSource = Enum.GetValues(typeof(DotLayoutEngine));
        layoutEngineComboBox.SelectedIndex = 0;
        layoutEngineComboBox.SelectionChanged += LayoutEngine_SelectionChanged;

        DrawGraph();
    }

    private void LayoutEngine_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        DrawGraph();
    }

    private void DotFileComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        DrawGraph();
    }

    private void DrawGraph()
    {
        // Read the graph decribed in the Dot language.
        // Alternatively the graph can be created using https://github.com/vfrz/DotNetGraph
        var dot = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/DotFiles/{dotFileComboBox.SelectedItem}");

        // Render the graph on the GraphvizWpfCanvas.
        // When the graph is rendered, each child of the canvas gets a Tag property with the corresponding DotElement like Vertex, Edge, or Cluster,
        // The Tag property can be used to implement mouse click events or add a tooltip like the one shown below.
        graphvizCanvas.DrawGraph(dot, (DotLayoutEngine)layoutEngineComboBox.SelectedItem);

        // Add a tooltip to each vertex
        foreach (var child in graphvizCanvas.Children.OfType<FrameworkElement>().Where(c => c.Tag is DotVertex))
        {
            var vertex = (DotVertex)child.Tag;
            var tooltip = new ToolTip
            {
                Content = $"Node: {vertex.Name}"
            };
            ToolTipService.SetToolTip(child, tooltip);
        }

        // Generate the SVG representation of the graph to use it as a reference to compare with the picture on GraphvizWpfCanvas
        var svg = DotGraphDescriptionProcessor.Process(dot, (DotLayoutEngine)layoutEngineComboBox.SelectedItem, "svg");
        svgViewBox.SvgSource = svg;
    }

    // Example of how to handle mouse click events on the elements of the Graph
    private void graphvizCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not FrameworkElement)
        {
            return;
        }

        var shape = (FrameworkElement)e.OriginalSource;
        switch (shape.Tag)
        {
            case DotVertex vertex:
                logTextBox.Text += $"OnMouseClick: Vertex Name={vertex.Name} Comment={vertex.Comment}\n";
                break;
            case DotEdge edge:
                logTextBox.Text += $"OnMouseClick: Edge Name={edge.Name} Comment={edge.Comment}\n";
                break;
            case DotCluster cluster:
                logTextBox.Text += $"OnMouseClick: Cluster Name={cluster.Name} Comment={cluster.Comment}\n";
                break;
        }

        logTextBox.ScrollToEnd();
    }
}
