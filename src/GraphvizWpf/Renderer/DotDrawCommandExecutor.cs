using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphvizWpf.Renderer;

[Flags]
internal enum FontCharacteristics : uint
{
    Bold = 1,
    Italic = 2,
    Underline = 4,
    Superscript = 8,
    Subscript = 16,
    StrikeThrough = 32,
    Overline = 64
}

internal class DotDrawCommandExecutor
{
    private readonly Canvas canvas;
    private readonly IEnumerable<DotJsonDrawCommand> drawCommands;
    private readonly double maxY;
    private readonly object tag;

    // Drawing state
    private Color fillColor;
    private Color penColor;
    private int fontSize;
    private string fontFamilyName;
    private string style;
    private FontCharacteristics fontStyle;

    // All canvas elements that will be drawn will have the same tag.
    // It will allow a user to identify the element when handling events like MouseClick, e.t.c.
    // maxY is needed to convert the y-coordinate from Graphviz to WPF.
    public DotDrawCommandExecutor(Canvas canvas, IEnumerable<DotJsonDrawCommand> drawCommands, object tag, double maxY)
    {
        this.canvas = canvas;
        this.drawCommands = drawCommands;
        this.tag = tag;
        this.maxY = maxY;
        fillColor = Colors.Transparent;
    }

    public void Execute()
    {
        if (drawCommands == null)
        {
            return;
        }

        foreach (var drawCommand in drawCommands)
        {
            ExecuteDrawCommand(drawCommand);
        }
    }

    private void ExecuteDrawCommand(DotJsonDrawCommand cmd)
    {
        // The draw commands are described in the https://graphviz.org/docs/outputs/canon/#xdot
        switch (cmd.op)
        {
            case "E":
            case "e":
                DrawEllipse(cmd.rect[0], cmd.rect[1], cmd.rect[2], cmd.rect[3]);
                break;
            case "P":
            case "p":
                DrawPolygon(cmd.points.Select(p => new Point(p[0], p[1])));
                break;
            case "L":
                DrawPolyline(cmd.points.Select(p => new Point(p[0], p[1])));
                break;
            case "B":
            case "b":
                var splinePoints = cmd.points.Select(p => new Point(p[0], p[1]));
                DrawBSpline(splinePoints.First(), splinePoints.Skip(1));
                break;
            case "T":
                DrawText(cmd.pt[0], cmd.pt[1], cmd.width, ToTextAlignment(cmd.align), cmd.text);
                break;
            case "t":
                fontStyle = (FontCharacteristics) cmd.fontchar;
                break;
            case "C":
                fillColor = (Color)ColorConverter.ConvertFromString(cmd.color);
                break;
            case "c":
                penColor = (Color)ColorConverter.ConvertFromString(cmd.color);
                break;
            case "F":
                fontFamilyName = cmd.face;
                fontSize = (int)cmd.size;
                break;
            case "S":
                style = cmd.style;
                break;
            case "I":
                // Drawing an image is not supported
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cmd.op), $"Unexpected draw command: {cmd.op}.");
        }
    }

    // w is the distance from the center to the furthest point on the ellipse along the x-axis.
    // h is the distance from the center to the furthest point on the ellipse along the y-axis.
    private void DrawEllipse(double xCenter, double yCenter, double w, double h)
    {
        var ellipse = new Ellipse
        {
            Width = w * 2,
            Height = h * 2,
            Stroke = new SolidColorBrush(penColor),
            StrokeDashArray = StyleToStrokeDashArray(style),
            Tag = tag,
            Fill = new SolidColorBrush(fillColor)
        };
        var ellipseUpperLeftCorner = new Point(xCenter - w, yCenter + h);
        var ellipseUpperLeftCornerOnCanvas = ToCanvasPosition(ellipseUpperLeftCorner);
        Canvas.SetLeft(ellipse, ellipseUpperLeftCornerOnCanvas.X);
        Canvas.SetTop(ellipse, ellipseUpperLeftCornerOnCanvas.Y);
        canvas.Children.Add(ellipse);
    }

    private void DrawPolygon(IEnumerable<Point> points)
    {
        canvas.Children.Add(new Polygon
        {
            Stroke = new SolidColorBrush(penColor),
            StrokeDashArray = StyleToStrokeDashArray(style),
            Fill = new SolidColorBrush(fillColor),
            Tag = tag,
            Points = new PointCollection(points.Select(ToCanvasPosition))
        });
    }

    private void DrawPolyline(IEnumerable<Point> points)
    {
        canvas.Children.Add(new Polyline
        {
            Stroke = new SolidColorBrush(penColor),
            Tag = tag,
            StrokeDashArray = StyleToStrokeDashArray(style),
            Points = new PointCollection(points.Select(ToCanvasPosition)),
        });
    }

    private void DrawText(double xBottomLeft, double yBottomLeft, double textWidth, TextAlignment alignment, string text)
    {
        // By default, the font family is Times-Roman.
        // We need to convert it to the proper font family name.
        fontFamilyName = fontFamilyName == "Times-Roman" ? "Times New Roman" : fontFamilyName;

        var fontFamily = new FontFamily(fontFamilyName);
        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(penColor),
            FontSize = fontSize,
            FontFamily = fontFamily,
            FontWeight = IsFontStyleSet(FontCharacteristics.Bold) ? FontWeights.Bold: FontWeights.Thin,
            FontStyle = IsFontStyleSet(FontCharacteristics.Italic) ? FontStyles.Italic: FontStyles.Normal,
            TextAlignment = alignment,
            Tag = tag
        };
        if (IsFontStyleSet(FontCharacteristics.Underline))
        {
            textBlock.TextDecorations.Add(TextDecorations.Underline);
        }
        if (IsFontStyleSet(FontCharacteristics.StrikeThrough))
        {
            textBlock.TextDecorations.Add(TextDecorations.Strikethrough);
        }
        if (IsFontStyleSet(FontCharacteristics.Overline))
        {
            textBlock.TextDecorations.Add(TextDecorations.OverLine);
        }

        var textHeight = fontSize * fontFamily.LineSpacing;
        var textUpperLeftCorner = new Point(xBottomLeft, yBottomLeft + textHeight);
        if (alignment == TextAlignment.Center)
        {
            textUpperLeftCorner.X -= textWidth / 2.0;
        }
        var textUpperLeftCornerOnCanvas = ToCanvasPosition(textUpperLeftCorner);
        Canvas.SetLeft(textBlock, textUpperLeftCornerOnCanvas.X);
        Canvas.SetTop(textBlock, textUpperLeftCornerOnCanvas.Y);
        canvas.Children.Add(textBlock);
    }

    private void DrawBSpline(Point startPosition, IEnumerable<Point> controlPoints)
    {
        canvas.Children.Add(new Path
        {
            Tag = tag,
            Stroke = new SolidColorBrush(penColor),
            StrokeDashArray = StyleToStrokeDashArray(style),
            Data = new PathGeometry
            {
                Figures = new PathFigureCollection([new PathFigure
                {
                    StartPoint = ToCanvasPosition(startPosition),
                    Segments = new PathSegmentCollection([new PolyBezierSegment
                    {
                        Points = new PointCollection(controlPoints.Select(ToCanvasPosition))
                    }]),
                }])
            }
        });
    }

    static TextAlignment ToTextAlignment(string align) => align switch
    {
        "c" => TextAlignment.Center,
        "r" => TextAlignment.Right,
        "l" => TextAlignment.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(align), $"Unexpected text alignment value: {align}.")
    };

    // The y-coordinate in Graphviz is measured from the bottom of the canvas, while in WPF, it is measured from the top.
    private Point ToCanvasPosition(Point graphvizPosition) => new(graphvizPosition.X, maxY - graphvizPosition.Y);

    private DoubleCollection StyleToStrokeDashArray(string style) => style switch
    {
        "dashed" => new DoubleCollection([6, 2]),
        "dotted" => new DoubleCollection([1]),
        // Other styles are not supported
        _ => new DoubleCollection()
    };

    private bool IsFontStyleSet(FontCharacteristics fontCharacteristic) => (fontStyle & fontCharacteristic) == fontCharacteristic;
}
