using Microsoft.Win32.SafeHandles;

namespace GraphvizWpf.Renderer.Interop;

internal sealed class SafeGraphHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private SafeGraphHandle() : base(true)
    {
    }


    internal SafeGraphHandle(IntPtr handle, bool ownsHandle)
        : base(ownsHandle)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        NativeMethods.agclose(handle);
        return true;
    }
}

internal sealed class SafeContextHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private SafeContextHandle() : base(true)
    {
    }

    internal SafeContextHandle(IntPtr handle, bool ownsHandle)
        : base(ownsHandle)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        NativeMethods.gvFreeContext(handle);
        return true;
    }
}

internal sealed class SafeRenderDataHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private SafeRenderDataHandle() : base(true)
    {
    }

    internal SafeRenderDataHandle(IntPtr handle, bool ownsHandle)
        : base(ownsHandle)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        NativeMethods.gvFreeRenderData(handle);
        return true;
    }
}
