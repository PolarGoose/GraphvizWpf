using GraphvizWpf.Renderer.Interop;
using System.Runtime.InteropServices;

namespace GraphvizWpf.Renderer;

internal static class DotGraphDescriptionProcessor
{
    public static string Process(string graphDescriptionInDotLanguage, DotLayoutEngine layoutEngine, string outputFormat)
    {
        using var graph = NativeMethods.agmemread(graphDescriptionInDotLanguage);
        if (graph.IsInvalid)
        {
            throw new ArgumentException($"Unable to read the given graph description. aglasterr: {NativeMethods.aglasterr()}");
        }

        using var context = NativeMethods.gvContext();
        if (NativeMethods.gvLayout(context, graph, layoutEngine.ToString()) != 0)
        {
            throw new ArgumentException($"Unable to create the gvContext using the layoutEngine={layoutEngine}. aglasterr: {NativeMethods.aglasterr()}");
        }

        if (NativeMethods.gvRenderData(context, graph, outputFormat, out var renderStringBuffer, out _) != 0)
        {
            throw new ArgumentException($"Unable to convert dotGraph to Json. aglasterr: {NativeMethods.aglasterr()}");
        }

        return Marshal.PtrToStringAnsi(renderStringBuffer.DangerousGetHandle());
    }
}
