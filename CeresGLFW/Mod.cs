using System;

namespace CeresGLFW
{
    [Flags]
    public enum Mod
    {
        NONE          = 0,
        SHIFT         = 0x0001,
        CONTROL       = 0x0002,
        ALT           = 0x0004,
        SUPER         = 0x0008,
        CAPS_LOCK     = 0x0010,
        NUM_LOCK      = 0x0020
    }
}