using System.Runtime.InteropServices;

namespace GraphvizWpf.Renderer.Interop;

internal static class NativeMethods
{
    [DllImport("cgraph.dll")]
    public static extern SafeGraphHandle agmemread(string graphVizData);

    [DllImport("cgraph.dll")]
    public static extern void agclose(IntPtr file);

    [DllImport("gvc.dll")]
    public static extern SafeContextHandle gvContext();

    [DllImport("gvc.dll")]
    public static extern int gvFreeLayout(SafeContextHandle context, SafeGraphHandle graph);

    [DllImport("gvc.dll")]
    public static extern int gvLayout(SafeContextHandle context, SafeGraphHandle graph, string engine);

    [DllImport("gvc.dll")]
    public static extern int gvRenderData(SafeContextHandle context, SafeGraphHandle graph, string format, out SafeRenderDataHandle result, out uint length);

    [DllImport("gvc.dll")]
    public static extern void gvFreeRenderData(IntPtr buffer);

    [DllImport("gvc.dll")]
    public static extern int gvFreeContext(IntPtr gvc);

    [DllImport("cgraph.dll")]
    public static extern string aglasterr();
}
