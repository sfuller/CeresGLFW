using System.Runtime.InteropServices;

namespace CeresGLFW
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GLFWVideoMode
    {
        // IMPORTANT: These fields and their order must match that of glfw3.h
        
        public int Width;
        public int Height;
        public int RedBits;
        public int GreenBits;
        public int BlueBits;
        public int RefreshRate;
    }
}