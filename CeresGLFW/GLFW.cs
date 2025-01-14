using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace CeresGLFW
{
    public static class GLFW
    {
        public const int DONT_CARE = -1;
        
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
        private static extern void glfwWaitEventsTimeout(double timeout);

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

        [DllImport(DllName)]
        private static extern void glfwSetMonitorCallback(MonitorFunction callback);

        [DllImport(DllName)]
        private static extern IntPtr glfwGetPrimaryMonitor();
        
        [DllImport(DllName)]
        private static extern IntPtr glfwGetMonitors(ref int count);

        [DllImport(DllName)]
        private static extern IntPtr glfwGetKeyName(int key, int scancode);
        
        [DllImport(DllName)]
        private static extern int glfwGetGamepadState(int jid, out GamepadState state);
        
        [DllImport(DllName)]
        private static extern int glfwGetError(IntPtr description);

        [DllImport(DllName)]
        private static extern unsafe byte** glfwGetRequiredInstanceExtensions(out uint count);
        
        private static readonly Dictionary<IntPtr, GLFWMonitor> _validMonitors = new();
        
        private delegate void MonitorFunction(IntPtr monitor, int eventValue);
        
        public static Thread? MainThread { get; private set; }

        private static MonitorFunction? _setMonitorCallback;

        public static void Init()
        {
            if (MainThread != null) {
                throw new InvalidOperationException("GLFW already initialized.");
            }
            if (glfwInit() == 0) {
                throw new InvalidOperationException("Failed to initialize GLFW.");
            }
            MainThread = Thread.CurrentThread;
            _setMonitorCallback = HandleMonitorChanged;
            glfwSetMonitorCallback(_setMonitorCallback);
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
        /// GLFW binding.
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

        public static void WaitEventsTimeout(double timeout)
        {
            glfwWaitEventsTimeout(timeout);
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
        
        public static bool GetGamepadState(int jid, out GamepadState state)
        {
            int result = glfwGetGamepadState(jid, out state);
            return result == 1;
        }

        public static double GetTime()
        {
            return glfwGetTime();
        }

        public static void SetTime(double time)
        {
            glfwSetTime(time);
        }

        public static GLFWMonitor GetPrimaryMonitor()
        {
            return MakeMonitor(glfwGetPrimaryMonitor());
        }

        public static GLFWMonitor[] GetMonitors()
        {
            int count = 0;
            GLFWMonitor[] monitors = new GLFWMonitor[count];
            IntPtr array = glfwGetMonitors(ref count);

            lock (_validMonitors) {
                unsafe {
                    IntPtr* monitorHandles = (IntPtr*)array.ToPointer();
                    for (int i = 0; i < count; ++i) {
                        monitors[i] = MakeMonitor(*monitorHandles++);
                    }
                }    
            }
            
            return monitors;
        }

        private static GLFWMonitor MakeMonitor(IntPtr handle)
        {
            lock (_validMonitors) {
                GLFWMonitor? monitor;
                if (!_validMonitors.TryGetValue(handle, out monitor)) {
                    monitor = new GLFWMonitor(handle);
                    _validMonitors.Add(handle, monitor);
                    return monitor;
                }
                return monitor;
            }
        }
        
        private static void HandleMonitorChanged(IntPtr monitorHandle, int eventValue)
        {
            lock (_validMonitors) {
                foreach (GLFWMonitor monitor in _validMonitors.Values) {
                    monitor.Invalidate();
                }
                _validMonitors.Clear();
            }
        }

        public static string? GetKeyName(Key key, int scancode)
        {
            return Marshal.PtrToStringUTF8(glfwGetKeyName((int)key, scancode));
        }

        public static unsafe string[] GetRequiredInstanceExtensions()
        {
            byte** result = glfwGetRequiredInstanceExtensions(out uint count);
            string? error = GetError();
            if (error != null) {
                throw new InvalidOperationException(error);
            }
            string[] extensions = new string[count];
            for (int i = 0; i < count; ++i) {
                extensions[i] = Marshal.PtrToStringAnsi(new IntPtr(result[i])) ?? string.Empty;
            }
            return extensions;
        }

        internal static unsafe string? GetError()
        {
            void* descPtr;
            int result = glfwGetError(new IntPtr(&descPtr));

            if (result == 0 || descPtr == null) {
                return null;
            }

            return Marshal.PtrToStringUTF8(new IntPtr(descPtr));
        }
        
    }
}
