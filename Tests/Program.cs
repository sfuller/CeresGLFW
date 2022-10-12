using CeresGLFW;

GLFW.Init();
WindowHints hints = new WindowHints();
GLFWWindow window = new GLFWWindow(800, 600, "CeresGLFW Test Window!", null, hints);
GLFW.SwapInterval(1);

GLFWMonitor primaryMonitor = GLFW.GetPrimaryMonitor();
GLFWVideoMode videoMode = primaryMonitor.GetVideoMode();
Console.WriteLine($"Width: {videoMode.Width}");
Console.WriteLine($"Height: {videoMode.Height}");
Console.WriteLine($"RedBits: {videoMode.RedBits}");
Console.WriteLine($"GreenBits: {videoMode.GreenBits}");
Console.WriteLine($"BlueBits: {videoMode.BlueBits}");
Console.WriteLine($"RefreshRate: {videoMode.RefreshRate}");

GLFW.SetTime(0);
double previous = 0;
while (!window.ShouldClose) {
    double time = GLFW.GetTime();
    double delta = time - previous;
    Console.WriteLine($"{GLFW.GetTime()} - {time - previous} - {1.0 / delta}");

    if (window.GetKey(Key.C)) {
        window.SetClipboardString("Fuzzy pickles!");
    }
    if (window.GetKey(Key.V)) {
        Console.WriteLine($"Clipboard: {window.GetClipboardString()}");
    }
    
    previous = time;
    window.SwapBuffers();
    GLFW.PollEvents();
}
