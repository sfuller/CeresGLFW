using System.Runtime.InteropServices;

namespace CeresGLFW
{
    
    [StructLayout(LayoutKind.Sequential)]
    public struct GamepadState
    {
        // ***
        // Important! Must be kept up to date with GLFWgamepadstate structure in glfw header!
        // 
        // Also to note, the PInvoke c-array to managed array marshalling is probably very wasteful, we may want to
        // improve the efficiency by doing manual marshalling 
        // ***

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)] // Note: I'm pretty sure PInvoke will add the necesary padding here.
        public byte[] Buttons;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] Axes;
    }
}