using System;
using System.Runtime.InteropServices;

namespace CeresGLFW
{
    public class GLFWMonitor
    {
        [DllImport(GLFW.DllName)]
        private static extern IntPtr glfwGetVideoMode(IntPtr monitor);
        
        public IntPtr Handle;

        public GLFWMonitor(IntPtr handle)
        {
            Handle = handle;
        }

        internal void Invalidate()
        {
            Handle = IntPtr.Zero;
        }

        private void CheckValidity()
        {
            if (Handle == IntPtr.Zero) {
                throw new ObjectDisposedException(
                    "Monitor handle is no longer valid. " +
                    "Monitor handles are invalidated after monitor configuration changes.");
            }
        }

        public GLFWVideoMode GetVideoMode()
        {
            CheckValidity();
            IntPtr ptr = glfwGetVideoMode(Handle);
            return Marshal.PtrToStructure<GLFWVideoMode>(ptr);
        }

    }
}