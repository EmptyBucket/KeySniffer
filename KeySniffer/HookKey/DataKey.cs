using System;
using System.Diagnostics;

namespace SnifferKey
{
    public class DataKey
    {
        public readonly int nCode;
        public readonly IntPtr wParam;
        public readonly IntPtr lParam;
        public readonly int VirtualKeyCode;
        public readonly int ScanCode;
        public readonly int Flags;

        public DataKey(
            int nCode, IntPtr wParam, IntPtr lParam,
            int virtualKeyCode, int scanCode, int flags)
        {
            this.nCode = nCode;
            this.wParam = wParam;
            this.lParam = lParam;
            this.VirtualKeyCode = virtualKeyCode;
            this.ScanCode = scanCode;
            this.Flags = flags;
        }

        public void DebugPrint()
        {
            InterpreterKeys interpreterKeys = new InterpreterKeys();
            char keyAscii = interpreterKeys.InterpretAsciiEx(this);

            string header = string.Format(
                "{0, 10} {1, 10} {2, 15} {3, 10} {4,5}",
                "wParam:", "lParam:", "VirtualKeyCode:", "ScanCode:",
                "Key:");
            string value = string.Format(
                "{0, 10} {1, 10} {2, 15} {3, 10} {4,5}",
                wParam, lParam,
                VirtualKeyCode, ScanCode,
                keyAscii.ToString());

            Debug.Print("{0}\n{1}", header, value);
        }
    }
}
