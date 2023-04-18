import ctypes
import win32con
import win32api
import win32gui

def find_window(class_name, window_name):
    return win32gui.FindWindow(class_name, window_name)

def show_window(hwnd, nCmdShow):
    win32gui.ShowWindow(hwnd, nCmdShow)

def keyboard_hook(nCode, wParam, lParam):
    if nCode == win32con.HC_ACTION and wParam == win32con.WM_KEYDOWN:
        vkCode = ctypes.c_ulong(lParam).value & 0xff
        if vkCode == ord('c') and win32api.GetKeyState(win32con.VK_LWIN) < 0:
            hwnd = find_window(None, "Window Title")
            if hwnd != 0:
                show_window(hwnd, win32con.SW_RESTORE)
            else:
                print("Window not found.")
    return win32gui.CallNextHookEx(None, nCode, wParam, lParam)

def set_hook():
    hook_proc = win32gui.LowLevelKeyboardProc(keyboard_hook)
    hook = win32gui.SetWindowsHookEx(win32con.WH_KEYBOARD_LL, hook_proc, win32api.GetModuleHandle(None), 0)
    return hook

def remove_hook(hook):
    win32gui.UnhookWindowsHookEx(hook)

def main():
    hook = set_hook()

    # Start message loop
    try:
        win32gui.PumpMessages()
    except KeyboardInterrupt:
        pass
    finally:
        remove_hook(hook)

if __name__ == "__main__":
    main()
