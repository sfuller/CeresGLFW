namespace CeresGLFW
{
    public class WindowHints
    {
        public bool Resizable = true;
        public bool Visible = true;
        public bool Decorated = true;
        public bool Focused = true;
        public bool AutoIconify = true;
        public bool Floating = false;
        public bool Maximized = false;
        public bool CenterCursor = true;
        public bool TransparentFramebuffer = false;
        public bool FocusOnShow = true;
        public bool ScaleToMonitor = false;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int RedBits = 8;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int GreenBits = 8;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int BlueBits = 8;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int AlphaBits = 8;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int DepthBits = 24;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int StencilBits = 8;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int AccumRedBits = 0;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int AccumGreenBits = 0;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int AccumBlueBits = 0;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int AccumAlphaBits = 0;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int AuxBuffers = 0;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int Samples = 0;
        
        /// <summary>
        /// 0 to INT_MAX or GLFW.DONT_CARE
        /// </summary>
        public int RefreshRate = GLFW.DONT_CARE;
        
        public bool Stereo = false;
        public bool SRGBCapable  = false;
        public bool DoubleBuffer = true;
        public Api ClientApi = Api.OpenGL;
        public ContextApi ContextCreationApi = ContextApi.Native;
        
        /// <summary>
        /// Any valid major version number of the chosen client API
        /// </summary>
        public int ContextVersionMajor = 1;
        
        /// <summary>
        /// Any valid minor version number of the chosen client API
        /// </summary>
        public int ContextVersionMinor = 0;
        
        public ContextRobustness ContextRobustness = ContextRobustness.NoRobustness;
        public ReleaseBehavior ReleaseBehavior = ReleaseBehavior.Any;
        public bool OpenGLForwardCompat = false;
        public bool OpenGLDebugContext = false;
        public OpenGLProfile OpenGLProfile = OpenGLProfile.Any;
        public bool CocoaRetinaFramebuffer = true;
        
        /// <summary>
        /// A UTF-8 encoded frame autosave name
        ///
        /// Binding Notes: String will be encoded to UTF-8
        /// </summary>
        public string CocoaFrameName = "";
        
        public bool CocoaGraphicsSwitching = false;
        
        /// <summary>
        /// An ASCII encoded WM_CLASS class name
        ///
        /// Binding Notes: String will be encoded to ASCII. An exception will be thrown if non-ascii characters are
        /// present. TODO: What exception?
        /// </summary>
        public string X11ClassName = "";
        
        /// <summary>
        /// An ASCII encoded WM_CLASS class name
        ///
        /// Binding Notes: String will be encoded to ASCII. An exception will be thrown if non-ascii characters are
        /// present. TODO: What exception?
        /// </summary>
        public string X11InstanceName = "";
    }
}
