using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace SignLanguageLearningSystem4
{
    class UnityHost : HwndHost
    {
        private Process _childProcess;
        private HandleRef _childHandleRef;

        public string AppPath { get; set; }
        public HandleRef Child => _childHandleRef;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var cmdline = $"-parentHWND {hwndParent.Handle} -screen-width 640 -screen-height 360";
            _childProcess = Process.Start(AppPath, cmdline);
            while (true)
            {
                var hwndChild = User32.FindWindowEx(hwndParent.Handle, IntPtr.Zero, null, null);
                if (hwndChild != IntPtr.Zero)
                {
                    return (_childHandleRef = new HandleRef(this, hwndChild));
                }
                Thread.Sleep(100);
            }
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _childProcess.Dispose();
        }
    }

    static class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hParent, IntPtr hChildAfter, string pClassName, string pWindowName);
    }
}
