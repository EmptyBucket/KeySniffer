using System;
using System.Runtime.InteropServices;

namespace SnifferKey
{
    class InterpreterKeys
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            [In, Optional] byte[] lpKeyState,
            out char lpChar,
            int uFlags
            );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ToAsciiEx(
            int uVirtKey,
            int uScanCode,
            [In, Optional] byte[] lpKeyState,
            out char lpChar,
            int uFlags,
            IntPtr dwhkl
            );

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(
            [In] IntPtr hWnd,
            [Out, Optional] IntPtr lpdwProcessId
            );

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetKeyboardLayout(
            [In] int idThread
            );

        private IntPtr GetKeyboardLayoutCurrentWindow()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            int windowThreadId = GetWindowThreadProcessId(foregroundWindow);

            return GetKeyboardLayout(windowThreadId);
        }

        public char InterpretAsciiEx(DataKey dataKey)
        {
            char key;
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            IntPtr keyboardLayout = GetKeyboardLayoutCurrentWindow();

            ToAsciiEx(
                dataKey.VirtualKeyCode,
                dataKey.ScanCode, keyboardState, out key,
                dataKey.Flags, keyboardLayout);

            return key;
        }

        public char InterpretAscii(DataKey dataKey)
        {
            char key;
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            ToAscii(
                dataKey.VirtualKeyCode,
                dataKey.ScanCode, keyboardState, out key,
                dataKey.Flags);

            return key;
        }
    }
}
