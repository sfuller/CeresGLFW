using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CeresGLFW
{
    public sealed class GLFWWindow : IDisposable
    {
        [DllImport(GLFW.DllName)]
        private static extern void glfwWindowHint(int hint, int value);

        [DllImport(GLFW.DllName)]
        private static extern void glfwWindowHintString(int hint, IntPtr value);
        
        [DllImport(GLFW.DllName)]
        private static extern IntPtr glfwCreateWindow(int width, int height, string title, IntPtr monitor, IntPtr share);

        [DllImport(GLFW.DllName)]
        private static extern void glfwDestroyWindow(IntPtr window);

        [DllImport(GLFW.DllName)]
        private static extern int glfwWindowShouldClose(IntPtr window);
        
        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowShouldClose(IntPtr window, int value);
        
        [DllImport(GLFW.DllName)]
        private static extern void glfwSwapBuffers(IntPtr window);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowUserPointer(IntPtr window, IntPtr pointer);

        [DllImport(GLFW.DllName)]
        internal static extern IntPtr glfwGetWindowUserPointer(IntPtr window);

        [DllImport(GLFW.DllName)]
        private static extern void glfwGetWindowContentScale(IntPtr window, out float xscale, out float yscale);

        [DllImport(GLFW.DllName)]
        private static extern void glfwGetCursorPos(IntPtr window, ref double x, ref double y);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetCursorPos(IntPtr window, double xpos, double ypos);

        [DllImport(GLFW.DllName)]
        private static extern int glfwGetKey(IntPtr window, Key key);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetCursor(IntPtr window, IntPtr cursor);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowTitle(IntPtr window, [MarshalAs(UnmanagedType.LPUTF8Str)] string title);

        [DllImport(GLFW.DllName)]
        private static extern int glfwGetMouseButton(IntPtr window, int button);

        [DllImport(GLFW.DllName)]
        private static extern int glfwGetWindowAttrib(IntPtr window, int attrib);

        [DllImport(GLFW.DllName)]
        private static extern int glfwGetInputMode(IntPtr window, int mode);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetInputMode(IntPtr window, int mode, int value);

        [DllImport(GLFW.DllName)]
        private static extern void glfwGetWindowSize(IntPtr window, ref int width, ref int height);

        [DllImport(GLFW.DllName)]
        private static extern void glfwGetFramebufferSize(IntPtr window, ref int width, ref int height);

        private delegate void SizeCallback(IntPtr window, int width, int height);
        private delegate void PosCallback(IntPtr window, int xpos, int ypos);
        private delegate void FramebufferSizeCallback(IntPtr window, int width, int height);
        private delegate void WindowRefreshCallback(IntPtr window);
        private delegate void CursorPositionCallback(IntPtr window, double x, double y);
        private delegate void MouseButtonCallback(IntPtr window, int button, int action, int mods);
        private delegate void ScrollCallback(IntPtr window, double x, double y);
        private delegate void CharCallback(IntPtr window, uint character);
        private delegate void KeyCallback(IntPtr window, int key, int scancode, int action, int mods);
        private delegate void CloseCallback(IntPtr window);
        private delegate void MaximizedCallback(IntPtr window, int maximized);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowSizeCallback(IntPtr window, SizeCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowPosCallback(IntPtr window, PosCallback callback);
        
        [DllImport(GLFW.DllName)]
        private static extern void glfwSetFramebufferSizeCallback(IntPtr window, FramebufferSizeCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowRefreshCallback(IntPtr window, WindowRefreshCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowCloseCallback(IntPtr window, CloseCallback callback);
        
        [DllImport(GLFW.DllName)]
        private static extern void glfwSetCursorPosCallback(IntPtr window, CursorPositionCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetMouseButtonCallback(IntPtr window, MouseButtonCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetScrollCallback(IntPtr window, ScrollCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetCharCallback(IntPtr window, CharCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetKeyCallback(IntPtr window, KeyCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetWindowMaximizeCallback(IntPtr window, MaximizedCallback callback);

        [DllImport(GLFW.DllName)]
        private static extern IntPtr glfwGetCocoaWindow(IntPtr window);

        [DllImport(GLFW.DllName)]
        private static extern IntPtr glfwGetClipboardString(IntPtr window);

        [DllImport(GLFW.DllName)]
        private static extern void glfwSetClipboardString(IntPtr window, IntPtr str);

        private IntPtr _window;

        internal IntPtr Handle {
            get {
                AssertNotDisposed();
                return _window;
            }
        }

        // These callbacks must be stored at a field level to avoid GC while they are in use by GLFW.
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly SizeCallback _sizeCallback;
        private readonly PosCallback _posCallback;
        private readonly MaximizedCallback _maximizedCallback;
        private readonly FramebufferSizeCallback _framebufferSizeCallback;
        private readonly WindowRefreshCallback _windowRefreshCallback;
        private readonly CursorPositionCallback _cursorPositionCallback;
        private readonly MouseButtonCallback _mouseButtonCallback;
        private readonly ScrollCallback _scrollCallback;
        private readonly CharCallback _charCallback;
        private readonly KeyCallback _keyCallback;
        private readonly CloseCallback _closeCallback;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
        
        public GLFWWindow(int width, int height, string title, GLFWWindow? share, WindowHints hints)
        {
            if (Thread.CurrentThread != GLFW.MainThread) {
                throw new InvalidOperationException("A GLFW window may only be created from the main GLFW thread.");
            }

            glfwWindowHint(0x00020003, hints.Resizable ? 1 : 0);
            glfwWindowHint(0x00020004, hints.Visible ? 1 : 0);
            glfwWindowHint(0x00020005, hints.Decorated ? 1 : 0);
            glfwWindowHint(0x00020001, hints.Focused ? 1 : 0);
            glfwWindowHint(0x00020006, hints.AutoIconify ? 1 : 0);
            glfwWindowHint(0x00020007, hints.Floating ? 1 : 0);
            glfwWindowHint(0x00020008, hints.Maximized ? 1 : 0);
            glfwWindowHint(0x00020009, hints.CenterCursor ? 1 : 0);
            glfwWindowHint(0x0002000A, hints.TransparentFramebuffer ? 1 : 0);
            glfwWindowHint(0x0002000C, hints.FocusOnShow ? 1 : 0);
            glfwWindowHint(0x0002200C, hints.ScaleToMonitor ? 1 : 0);
            glfwWindowHint(0x00021001, hints.RedBits);
            glfwWindowHint(0x00021002, hints.GreenBits);
            glfwWindowHint(0x00021003, hints.BlueBits);
            glfwWindowHint(0x00021004, hints.AlphaBits);
            glfwWindowHint(0x00021005, hints.DepthBits);
            glfwWindowHint(0x00021006, hints.StencilBits);
            glfwWindowHint(0x00021007, hints.AccumRedBits);
            glfwWindowHint(0x00021008, hints.AccumGreenBits);
            glfwWindowHint(0x00021009, hints.AccumBlueBits);
            glfwWindowHint(0x0002100A, hints.AccumAlphaBits);
            glfwWindowHint(0x0002100B, hints.AuxBuffers);
            glfwWindowHint(0x0002100D, hints.Samples);
            glfwWindowHint(0x0002100F, hints.RefreshRate);
            glfwWindowHint(0x0002100C, hints.Stereo ? 1 : 0);
            glfwWindowHint(0x0002100E, hints.SRGBCapable ? 1 : 0);
            glfwWindowHint(0x00021010, hints.DoubleBuffer ? 1 : 0);
            glfwWindowHint(0x00022001, (int)hints.ClientApi);
            glfwWindowHint(0x0002200B, (int)hints.ContextCreationApi);
            glfwWindowHint(0x00022002, hints.ContextVersionMajor);
            glfwWindowHint(0x00022003, hints.ContextVersionMinor);
            glfwWindowHint(0x00022005, (int)hints.ContextRobustness);
            glfwWindowHint(0x00022009, (int)hints.ReleaseBehavior);
            glfwWindowHint(0x00022006, hints.OpenGLForwardCompat ? 1 : 0);
            glfwWindowHint(0x00022007, hints.OpenGLDebugContext ? 1 : 0);
            glfwWindowHint(0x00022008, (int)hints.OpenGLProfile);
            glfwWindowHint(0x00023001, hints.CocoaRetinaFramebuffer ? 1 : 0);
            glfwWindowHint(0x00023003, hints.CocoaGraphicsSwitching ? 1 : 0);

            byte[] cocoaFrameName = Encoding.UTF8.GetBytes(hints.CocoaFrameName);
            GCHandle cocoaFrameNameHandle = GCHandle.Alloc(cocoaFrameName, GCHandleType.Pinned);
            glfwWindowHintString(0x00023002, cocoaFrameNameHandle.AddrOfPinnedObject());
            cocoaFrameNameHandle.Free();
            
            byte[] x11ClassName = Encoding.ASCII.GetBytes(hints.X11ClassName);
            GCHandle x11ClassNameHandle = GCHandle.Alloc(x11ClassName, GCHandleType.Pinned);
            glfwWindowHintString(0x00024001, x11ClassNameHandle.AddrOfPinnedObject());
            x11ClassNameHandle.Free();
            
            byte[] x11InstanceName = Encoding.ASCII.GetBytes(hints.X11InstanceName);
            GCHandle x11InstanceNameHandle = GCHandle.Alloc(x11InstanceName, GCHandleType.Pinned);
            glfwWindowHintString(0x00024002, x11InstanceNameHandle.AddrOfPinnedObject());
            x11InstanceNameHandle.Free();

            IntPtr shareHandle = share == null ? IntPtr.Zero : share._window;
            
            _window = glfwCreateWindow(width, height, title, IntPtr.Zero, shareHandle);
            if (_window == IntPtr.Zero) {
                string? error = GLFW.GetError();
                throw new InvalidOperationException($"Failed to create GLFW window: {error}");
            }

            GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
            glfwSetWindowUserPointer(_window, GCHandle.ToIntPtr(handle));

            _sizeCallback = HandleWindowResized;
            _posCallback = HandlePositionChanged;
            _framebufferSizeCallback = HandleFramebufferResized;
            _windowRefreshCallback = HandleRefreshRequested;
            _cursorPositionCallback = HandleCursorPositionChanged;
            _mouseButtonCallback = HandleMouseButtonChanged;
            _scrollCallback = HandleScrollChanged;
            _charCallback = HandleCharacterInput;
            _keyCallback = HandleKeyChanged;
            _closeCallback = HandleClosed;
            _maximizedCallback = HandleMaximizedChanged;
            
            glfwSetWindowPosCallback(_window, _posCallback);
            glfwSetWindowSizeCallback(_window, _sizeCallback);
            glfwSetFramebufferSizeCallback(_window, _framebufferSizeCallback);
            glfwSetWindowRefreshCallback(_window, _windowRefreshCallback);
            glfwSetWindowCloseCallback(_window, _closeCallback);
            glfwSetCursorPosCallback(_window, _cursorPositionCallback);
            glfwSetMouseButtonCallback(_window, _mouseButtonCallback);
            glfwSetScrollCallback(_window, _scrollCallback);
            glfwSetCharCallback(_window, _charCallback);
            glfwSetKeyCallback(_window, _keyCallback);
            glfwSetWindowMaximizeCallback(_window, _maximizedCallback);
        }

        public bool ShouldClose {
            get => glfwWindowShouldClose(Handle) != 0;
            set => glfwSetWindowShouldClose(Handle, value ? 1 : 0);
        }

        public event Action<int, int>? SizeChanged;
        public event Action<int, int>? SizeChangedInScreenCoordinates;
        public event Action<int, int>? PositionChanged;
        public event Action<int, int>? FramebufferResized;
        public event Action? RefreshRequested;
        public event Action<double, double>? CursorPositionChanged;
        public event Action<int, InputAction, Mod>? MouseButtonChanged;
        public event Action<double, double>? ScrollChanged;
        public event Action<uint>? CharacterInput;
        public event Action<Key, int, InputAction, Mod>? KeyChanged;
        public event Action? Closed;
        public event Action<bool>? MaximizedChanged;

        public void SwapBuffers()
        {
            glfwSwapBuffers(Handle);
        }

        public void GetContentScale(out float xscale, out float yscale)
        {
            glfwGetWindowContentScale(_window, out xscale, out yscale);
        }
        
        public void GetCursorPos(out double x, out double y)
        {
            AssertNotDisposed();
            x = y = 0;
            glfwGetCursorPos(_window, ref x, ref y);
        }

        public void SetCursorPos(double xpos, double ypos)
        {
            glfwSetCursorPos(_window, xpos, ypos);
        }

        public bool GetKey(Key key)
        {
            return glfwGetKey(_window, key) != 0;
        }

        public void SetCursor(IGLFWCursor cursor)
        {
            glfwSetCursor(_window, cursor.Handle);
        }

        public void SetTitle(string title)
        {
            glfwSetWindowTitle(_window, title);
        }

        public InputAction GetMouseButton(int button)
        {
            return (InputAction)glfwGetMouseButton(_window, button);
        }

        public int GetAttrib(WindowAttribute attrib)
        {
            return glfwGetWindowAttrib(_window, (int)attrib);
        }

        public CursorMode GetCursorMode()
        {
            return (CursorMode)glfwGetInputMode(_window, 0x00033001 /* GLFW_CURSOR */);
        }
        
        public void SetCursorMode(CursorMode mode)
        {
            glfwSetInputMode(_window, 0x00033001 /* GLFW_CURSOR */, (int)mode);
        }

        public void GetSize(out int width, out int height)
        {
            width = height = 0;
            glfwGetWindowSize(_window, ref width, ref height);
        }

        /// <summary>
        /// Not part of the GLFW api, but a convenience function which always returns the window size in
        /// screen coordinate space.
        ///
        /// GetSize (glfwGetWindowSize) returns screen coordinates on macOS, but not on Windows/X11 (Presumably due to
        /// backwards compatibility?) 
        ///
        /// See https://github.com/glfw/glfw/issues/845
        /// </summary>
        public void GetSizeInScreenCoordinates(out int width, out int height)
        {
            GetFramebufferSize(out int framebufferX, out int framebufferY);
            GetContentScale(out float scaleX, out float scaleY);
            width = (int)(framebufferX / scaleX);
            height = (int)(framebufferY / scaleY);
        }

        public void GetFramebufferSize(out int width, out int height)
        {
            width = height = 0;
            glfwGetFramebufferSize(_window, ref width, ref height);
        }

        public IntPtr GetCocoaWindow()
        {
            return glfwGetCocoaWindow(_window);
        }

        public string? GetClipboardString()
        {
            return Marshal.PtrToStringAnsi(glfwGetClipboardString(_window));
        }

        public unsafe void* GetClipboardStringRaw()
        {
            return (void*)glfwGetClipboardString(_window);
        }

        public void SetClipboardString(string str)
        {
            IntPtr strPtr = Marshal.StringToHGlobalAnsi(str);
            try {
                glfwSetClipboardString(_window, strPtr);
            } finally {
                Marshal.FreeHGlobal(strPtr);
            }
        }

        public unsafe void SetClipboardStringRaw(void* str)
        {
            glfwSetClipboardString(_window, new IntPtr(str));
        }

        private void ReleaseUnmanagedResources()
        {
            glfwDestroyWindow(_window);
            _window = IntPtr.Zero;
        }

        public void Dispose()
        {
            AssertNotDisposed();
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~GLFWWindow()
        {
            ReleaseUnmanagedResources();
        }

        private void AssertNotDisposed()
        {
            if (_window == IntPtr.Zero) {
                throw new ObjectDisposedException(typeof(GLFWWindow).Name);
            }
        }

        private static GLFWWindow GetWindowFromPtr(IntPtr windowPtr)
        {
            GLFWWindow? window = (GLFWWindow?) GCHandle.FromIntPtr(glfwGetWindowUserPointer(windowPtr)).Target;
            if (window == null) {
                // TODO: Is this the best way to handle this?
                throw new InvalidOperationException("GLFW Binding Bug! Somehow the managed class was GC'd, but the glfw resource still exists.");
            }
            return window;
        }

        private static void HandleWindowResized(IntPtr windowPtr, int width, int height)
        {
            GetWindowFromPtr(windowPtr).SizeChanged?.Invoke(width, height);
        }

        private static void HandlePositionChanged(IntPtr windowPtr, int xpos, int ypos)
        {
            GetWindowFromPtr(windowPtr).PositionChanged?.Invoke(xpos, ypos);
        }

        private static void HandleMaximizedChanged(IntPtr windowPtr, int maximized)
        {
            GetWindowFromPtr(windowPtr).MaximizedChanged?.Invoke(maximized != 0);
        }
        
        private static void HandleFramebufferResized(IntPtr windowPtr, int width, int height)
        {
            GLFWWindow window = GetWindowFromPtr(windowPtr);
            
            window.FramebufferResized?.Invoke(width, height);
            
            if (window.SizeChangedInScreenCoordinates != null) {
                window.GetContentScale(out float scaleX, out float scaleY);
                window.SizeChangedInScreenCoordinates.Invoke((int)(width / scaleX), (int)(height / scaleY));    
            }
        }

        private static void HandleRefreshRequested(IntPtr windowPtr)
        {
            GetWindowFromPtr(windowPtr).RefreshRequested?.Invoke();
        }

        private static void HandleCursorPositionChanged(IntPtr windowPtr, double x, double y)
        {
            GetWindowFromPtr(windowPtr).CursorPositionChanged?.Invoke(x, y);
        }

        private static void HandleMouseButtonChanged(IntPtr windowPtr, int button, int action, int mods)
        {
            GetWindowFromPtr(windowPtr).MouseButtonChanged?.Invoke(button, (InputAction)action, (Mod)mods);
        }

        private static void HandleScrollChanged(IntPtr windowPtr, double x, double y)
        {
            GetWindowFromPtr(windowPtr).ScrollChanged?.Invoke(x, y);
        }

        private static void HandleCharacterInput(IntPtr windowPtr, uint character)
        {
            GetWindowFromPtr(windowPtr).CharacterInput?.Invoke(character);
        }

        private static void HandleKeyChanged(IntPtr windowPtr, int key, int scancode, int action, int mods)
        {
            GetWindowFromPtr(windowPtr).KeyChanged?.Invoke((Key)key, scancode, (InputAction)action, (Mod)mods);
        }

        private static void HandleClosed(IntPtr windowPtr)
        {
            GetWindowFromPtr(windowPtr).Closed?.Invoke();
        }
    }
}
