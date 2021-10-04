using System;
using System.Runtime.InteropServices;

namespace CeresGLFW
{
    // NOTE: Not disposable.. Standard cursors do not need to be freed (Or so the documentation leads me to believe)
    // TODO: THESE ARE DISPOABLE! THE DOCUMENTATION IS CONFUSING, BUT YES THESE CAN BE FREED with glfwDeleteCursor.
    public sealed class GLFWStandardCursor : IGLFWCursor
    {
        [DllImport(GLFW.DllName)]
        private static extern IntPtr glfwCreateStandardCursor(int shape);

        [DllImport(GLFW.DllName)]
        private static extern void glfwDestroyCursor(IntPtr cursor);

        private readonly IntPtr _handle;
        IntPtr IGLFWCursor.Handle => _handle;

        public GLFWStandardCursor(StandardCursorShape shape)
        {
            _handle = glfwCreateStandardCursor((int) shape);
        }

        private void ReleaseUnmanagedResources()
        {
            glfwDestroyCursor(_handle);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~GLFWStandardCursor() {
            ReleaseUnmanagedResources();
        }
    }
}
