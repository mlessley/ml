using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

class Program
{
    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_C = 0x43;
    private static IntPtr _hookId = IntPtr.Zero;

    const int SW_RESTORE = 9;

    static void Main(string[] args)
    {
        // Register the hotkey
        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, IntPtr.Zero, 0);

        // Run the program in the background
        Application.Run();

        // Unregister the hotkey when the program exits
        UnhookWindowsHookEx(_hookId);
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            if ((Keys)vkCode == Keys.C && Control.ModifierKeys == Keys.LWin)
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

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}
