package main

import (
	"fmt"
	"syscall"
	"unsafe"
)

var (
	moduser32 = syscall.NewLazyDLL("user32.dll")

	procFindWindowW  = moduser32.NewProc("FindWindowW")
	procShowWindow   = moduser32.NewProc("ShowWindow")
	procSetWindowsHookEx = moduser32.NewProc("SetWindowsHookExW")
	procUnhookWindowsHookEx = moduser32.NewProc("UnhookWindowsHookEx")
	procCallNextHookEx = moduser32.NewProc("CallNextHookEx")
)

const (
	SW_RESTORE = 9
	WH_KEYBOARD_LL = 13
	WM_KEYDOWN = 0x0100
	VK_C = 0x43
)

type HookProc func(nCode int, wParam, lParam uintptr) uintptr

type KBDLLHOOKSTRUCT struct {
    vkCode uint32
    scanCode uint32
    flags uint32
    time uint32
    dwExtraInfo uintptr
}

var (
	hHook uintptr
)

func FindWindow(className, windowName string) (hwnd uintptr) {
	classNamePtr, windowNamePtr := syscall.StringToUTF16Ptr(className), syscall.StringToUTF16Ptr(windowName)
	hwnd, _, _ = procFindWindowW.Call(uintptr(unsafe.Pointer(classNamePtr)), uintptr(unsafe.Pointer(windowNamePtr)))
	return hwnd
}

func ShowWindow(hwnd uintptr, nCmdShow int32) (success bool) {
	ret, _, _ := procShowWindow.Call(hwnd, uintptr(nCmdShow))
	success = ret != 0
	return success
}

func SetWindowsHookEx(idHook int32, lpfn HookProc, hMod uintptr, dwThreadId uint32) (hHook uintptr) {
	ret, _, _ := procSetWindowsHookEx.Call(
		uintptr(idHook),
		syscall.NewCallback(lpfn),
		hMod,
		uintptr(dwThreadId),
	)
	hHook = ret
	return hHook
}

func UnhookWindowsHookEx(hHook uintptr) (success bool) {
	ret, _, _ := procUnhookWindowsHookEx.Call(hHook)
	success = ret != 0
	return success
}

func CallNextHookEx(hHook uintptr, nCode int32, wParam, lParam uintptr) (result uintptr) {
	ret, _, _ := procCallNextHookEx.Call(hHook, uintptr(nCode), wParam, lParam)
	result = ret
	return result
}

func HookCallback(nCode int, wParam, lParam uintptr) uintptr {
	if nCode >= 0 && wParam == WM_KEYDOWN {
		kbdHook := (*KBDLLHOOKSTRUCT)(unsafe.Pointer(lParam))
		if kbdHook.vkCode == VK_C && syscall.GetKeyState(0x5b) < 0 {
			hwnd := FindWindow(nil, "Window Title")
			if hwnd != 0 {
				ShowWindow(hwnd, SW_RESTORE)
			} else {
				fmt.Println("Window not found.")
			}
		}
	}
	return CallNextHookEx(hHook, nCode, wParam, lParam)
}

func main() {
	hHook = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, 0, 0)
	defer UnhookWindowsHookEx(hHook)

	// Prevent the program from exiting
	select {}
}
