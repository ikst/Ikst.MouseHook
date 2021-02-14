using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ikst.MouseHook
{
    /// <summary>
    /// Class for intercepting low level Windows mouse hooks.
    /// </summary>
    public class MouseHook
    {

        private const int WH_MOUSE_LL = 14;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }


        /// <summary>
        /// マウスフックイベント
        /// </summary>
        internal delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        internal MouseHookHandler hookHandler;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseStruct"></param>
        public delegate void MouseHookCallback(MSLLHOOKSTRUCT mouseStruct);


        #region イベント
        public event MouseHookCallback LeftButtonDown;
        public event MouseHookCallback LeftButtonUp;
        public event MouseHookCallback RightButtonDown;
        public event MouseHookCallback RightButtonUp;
        public event MouseHookCallback MouseMove;
        public event MouseHookCallback MouseWheel;

        [Obsolete("動作しません", error: true)]
        public event MouseHookCallback DoubleClick;

        public event MouseHookCallback MiddleButtonDown;
        public event MouseHookCallback MiddleButtonUp;
        #endregion

        /// <summary>フックID</summary>
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>開始状態</summary>
        public bool IsStarted { get; private set; }


        /// <summary>
        /// マウスフック開始
        /// </summary>
        public void Start()
        {
            if (!IsStarted)
            {
                hookHandler = HookFunc;
                hookID = SetHook(hookHandler);

                IsStarted = true;
            }
        }

        /// <summary>
        /// マウスフック終了
        /// </summary>
        public void Stop()
        {
            if (IsStarted)
            {
                if (hookID == IntPtr.Zero) return;

                NativeMethods.UnhookWindowsHookEx(hookID);
                hookID = IntPtr.Zero;

                IsStarted = false;
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~MouseHook()
        {
            Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        /// <returns>Hook ID</returns>
        private IntPtr SetHook(MouseHookHandler proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
                return NativeMethods.SetWindowsHookEx(WH_MOUSE_LL, proc, NativeMethods.GetModuleHandle(module.ModuleName), 0);
        }


        /// <summary>
        /// 
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0)
            {
                switch ((int)wParam)
                {
                    case WM_LBUTTONDOWN:
                        if (LeftButtonDown != null) LeftButtonDown((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_LBUTTONUP:
                        if (LeftButtonUp != null) LeftButtonUp((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_RBUTTONDOWN:
                        if (RightButtonDown != null) RightButtonDown((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_RBUTTONUP:
                        if (RightButtonUp != null) RightButtonUp((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_MOUSEMOVE:
                        if (MouseMove != null) MouseMove((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_MOUSEWHEEL:
                        if (MouseWheel != null) MouseWheel((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_LBUTTONDBLCLK:
                        if (DoubleClick != null) DoubleClick((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_MBUTTONDOWN:
                        if (MiddleButtonDown != null) MiddleButtonDown((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    case WM_MBUTTONUP:
                        if (MiddleButtonUp != null) MiddleButtonUp((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT)));
                        break;
                    default:
                        break;
                }
            }

            return NativeMethods.CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        #region WinAPI

        /// <summary>
        /// WinAPI
        /// </summary>
        protected static class NativeMethods
        {

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int idHook, MouseHookHandler lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);

        }

        #endregion
    }
}
