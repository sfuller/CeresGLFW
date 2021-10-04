using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace CeresGLFW
{
    public static class GLFW
    {
        internal const string DllName = "glfw3";

        [DllImport(DllName)]
        private static extern int glfwInit();

        [DllImport(DllName)]
        private static extern void glfwMakeContextCurrent(IntPtr window);

        [DllImport(DllName)]
        private static extern IntPtr glfwGetCurrentContext();
        
        [DllImport(DllName)]
        private static extern void glfwPollEvents();
        
        [DllImport(DllName)]
        private static extern void glfwWaitEvents();

        [DllImport(DllName)]
        private static extern void glfwPostEmptyEvent();

        [DllImport(DllName)]
        private static extern IntPtr glfwGetProcAddress(string name);

        [DllImport(DllName)]
        private static extern void glfwSwapInterval(int interval);

        [DllImport(DllName)]
        private static extern IntPtr glfwGetJoystickAxes(int jid, ref int count);

        [DllImport(DllName)]
        private static extern IntPtr glfwGetJoystickButtons(int jid, ref int count);

        [DllImport(DllName)]
        private static extern double glfwGetTime();

        [DllImport(DllName)]
        private static extern void glfwSetTime(double time);

        public const int DONT_CARE = -1;
        
        public static Thread? MainThread { get; private set; }

        public static void Init()
        {
            if (glfwInit() == 0) {
                throw new InvalidOperationException("Failed to initialize GLFW");
            }
            MainThread = Thread.CurrentThread;
        }

        public static void MakeContextCurrent(GLFWWindow? window)
        {
            if (window == null) {
                glfwMakeContextCurrent(IntPtr.Zero);
            }
            else {
                glfwMakeContextCurrent(window.Handle);    
            }
            
        }

        /// <summary>
        /// Returns the <see cref="GLFWWindow"/> that the current context is associated with.
        /// Returns null if the current context is not set, or if the associated GLFW window is not managed by the
        /// the GLFW binding.
        /// Use <see cref="IsCurrentContextSet"/> if you need to determine if any GLFW window, not just any window
        /// within the binding, is associated with the current context.
        /// </summary>s
        public static GLFWWindow? GetWindowWithCurrentContext()
        {
            IntPtr currentContextHandle = glfwGetCurrentContext();
            if (currentContextHandle == IntPtr.Zero) {
                return null;
            }
            IntPtr userPointer = GLFWWindow.glfwGetWindowUserPointer(currentContextHandle);
            return GCHandle.FromIntPtr(userPointer).Target as GLFWWindow;
        }

        public static bool IsCurrentContextSet()
        {
            IntPtr currentContextHandle = glfwGetCurrentContext();
            return currentContextHandle == IntPtr.Zero;
        }

        public static void PollEvents()
        {
            glfwPollEvents();
        }

        public static void WaitEvents()
        {
            glfwWaitEvents();
        }

        public static void PostEmptyEvent()
        {
            glfwPostEmptyEvent();
        }

        public static T? GetProc<T>(string name) where T : class
        {
            IntPtr proc = glfwGetProcAddress(name);
            if (proc == IntPtr.Zero) {
                return null;
            }
            return Marshal.GetDelegateForFunctionPointer<T>(proc);
        }

        public static void SwapInterval(int interval)
        {
            glfwSwapInterval(interval);
        }

        public static float[] GetJoystickAxes(int jid)
        {
            int count = 0;
            IntPtr results = glfwGetJoystickAxes(jid, ref count);
            float[] axes = new float[count];
            unsafe {
                fixed (float* axesPtr = axes) {
                    long bytes = count * sizeof(float);
                    Buffer.MemoryCopy((float*)results, axesPtr, bytes, bytes);    
                }
            }
            return axes;
        }

        public static bool[] GetJoystickButtons(int jid)
        {
            int count = 0;
            IntPtr results = glfwGetJoystickButtons(jid, ref count);
            bool[] buttons = new bool[count];
            unsafe {
                fixed (bool* axesPtr = buttons) {
                    long bytes = count * sizeof(float);
                    Buffer.MemoryCopy((bool*)results, axesPtr, bytes, bytes);
                }
            }
            return buttons;
        }

        public static double GetTime()
        {
            return glfwGetTime();
        }

        public static void SetTime(double time)
        {
            glfwSetTime(time);
        }

    }
}
