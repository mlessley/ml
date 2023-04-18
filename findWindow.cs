using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_RESTORE = 9;

    static void Main(string[] args)
    {
        // Find the window with the specified title
        IntPtr hWnd = FindWindow(null, "Window Title");

        if (hWnd != IntPtr.Zero)
        {
            // Show the window
            ShowWindow(hWnd, SW_RESTORE);
        }
        else
        {
            Console.WriteLine("Window not found.");
        }
    }
}
