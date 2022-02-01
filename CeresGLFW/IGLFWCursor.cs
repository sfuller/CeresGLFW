using System;

namespace CeresGLFW
{
    public interface IGLFWCursor : IDisposable
    {
        internal IntPtr Handle { get; }
    }
}
