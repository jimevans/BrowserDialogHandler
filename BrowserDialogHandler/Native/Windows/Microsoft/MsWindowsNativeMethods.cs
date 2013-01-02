// <copyright file="MsWindowsNativeMethods.cs" company="BrowserDialogHandler Project">
// Copyright 2013 Jim Evans
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Accessibility;
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native.Windows.Microsoft
{
    /// <summary>
    /// Class containing the native methods for Microsoft Windows operating systems.
    /// </summary>
    internal static class MsWindowsNativeMethods
    {
        #region P/Invoke constants
        internal const int BM_CLICK = 245;
        internal const int WM_ACTIVATE = 6;
        internal const int MA_ACTIVATE = 1;
        internal const int WM_SYSCOMMAND = 0x0112;
        internal const int WM_CLOSE = 0x0010;
        internal const uint WM_CHAR = 0x0102;
        internal const int SC_CLOSE = 0xF060;

        internal const int KEYEVENTF_EXTENDEDKEY = 0x1;
        internal const int KEYEVENTF_KEYUP = 0x2;
        internal const int KEYEVENTF_TAB = 0x09;

        internal const int SMTO_ABORTIFHUNG = 2;

        internal static Guid IID_IAccessible = new Guid("618736E0-3C3D-11CF-810C-00AA00389B71");
        #endregion

        #region P/Invoke object enumeration callback delegates
        /// <summary>
        /// Delegate for enumerating threads.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <param name="lParam">A parameter to the procedure.</param>
        /// <returns><see langword="true"/> to continue enumerating; otherwise, <see langword="false"/>.</returns>
        public delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// Delegate for enumerating windows.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <param name="lParam">A parameter to the procedure.</param>
        /// <returns><see langword="true"/> to continue enumerating; otherwise, <see langword="false"/>.</returns>
        public delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// Delegate for enumerating child windows.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <param name="lParam">A parameter to the procedure.</param>
        /// <returns><see langword="true"/> to continue enumerating; otherwise, <see langword="false"/>.</returns>
        public delegate bool EnumChildWindowProc(IntPtr hwnd, IntPtr lParam);
        #endregion

        #region P/Invoke enumerated values
        /// <summary>
        /// Values for keyboard events.
        /// </summary>
        [Flags]
        internal enum KeyEventFlags : uint
        {
            /// <summary>
            /// No flags.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// Extended key.
            /// </summary>
            ExtendedKey = 0x0001,

            /// <summary>
            /// Key up.
            /// </summary>
            KeyUp = 0x0002,

            /// <summary>
            /// Unicode key point.
            /// </summary>
            Unicode = 0x0004,

            /// <summary>
            /// Use scan code.
            /// </summary>
            ScanCode = 0x0008
        }

        /// <summary>
        /// Values for mouse events.
        /// </summary>
        [Flags]
        internal enum MouseEventFlags : uint
        {
            /// <summary>
            /// Move the mouse
            /// </summary>
            Move = 0x0001,

            /// <summary>
            /// Left button down
            /// </summary>
            LeftDown = 0x0002,

            /// <summary>
            /// Left button up
            /// </summary>
            LeftUp = 0x0004,

            /// <summary>
            /// Right button down
            /// </summary>
            RightDown = 0x0008,

            /// <summary>
            /// Right button up
            /// </summary>
            RightUp = 0x0010,

            /// <summary>
            /// Middle button down
            /// </summary>
            MiddleDown = 0x0020,

            /// <summary>
            /// Middle button up
            /// </summary>
            MiddleUp = 0x0040,

            /// <summary>
            /// X button down
            /// </summary>
            XDown = 0x0080,

            /// <summary>
            /// X button up
            /// </summary>
            XUp = 0x0100,

            /// <summary>
            /// Wheel event
            /// </summary>
            Wheel = 0x0800,

            /// <summary>
            /// Use virtual desktop
            /// </summary>
            VirtualDesk = 0x4000,

            /// <summary>
            /// Use absolute coordinates
            /// </summary>
            Absolute = 0x8000
        }

        /// <summary>
        /// Values for type of input
        /// </summary>
        internal enum InputType
        {
            /// <summary>
            /// Structure contains mouse input
            /// </summary>
            Mouse = 0,

            /// <summary>
            /// Structure contains keyboard input
            /// </summary>
            Keyboard = 1,

            /// <summary>
            /// Structure contains other hardware input
            /// </summary>
            Hardware = 2
        }

        /// <summary>
        /// Values for virtual key codes
        /// </summary>
        internal enum VK : short
        {
            Nothing = -1,
            Backspace = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,
            Shift = 0x10,
            Control = 0x11,
            Menu = 0x12,
            Pause = 0x13,
            CapsLock = 0x14,
            Escape = 0x1B,
            Spacebar = 0x20,
            Prior = 0x21,
            Next = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            Snapshot = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            Help = 0x2F,

            // VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
            // 0x40 : unassigned
            // VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
            NumPad0 = 0x60,
            NumPad1 = 0x61,
            NumPad2 = 0x62,
            NumPad3 = 0x63,
            NumPad4 = 0x64,
            NumPad5 = 0x65,
            NumPad6 = 0x66,
            NumPad7 = 0x67,
            NumPad8 = 0x68,
            NumPad9 = 0x69,
            Multiply = 0x6A,
            Add = 0x6B,
            Separator = 0x6C,
            Subtract = 0x6D,
            Decimal = 0x6E,
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            NumLock = 0x90,
            ScrollLock = 0x91,
            OemSemicolon = 0xBA,   // ';:' for US
            OemPlus = 0xBB,   // '+' any country
            OemComma = 0xBC,   // ',' any country
            OemMinus = 0xBD,   // '-' any country
            OemPeriod = 0xBE,   // '.' any country
            OemQuestion = 0xBF,   // '/?' for US
            OemTilde = 0xC0,   // '`~' for US
            OemOpenBrackets = 0xDB,   // '[{' for US
            OemPipe = 0xDC,   // '|' for US
            OemCloseBrackets = 0xDD,   // ']}' for US
            OemQuotes = 0xDE,   // ''"' for US
            OemBackslash = 0xE2,   // '\' for US
            LeftWindows = 0x5B,
            RightWindows = 0x5C
        }

        [Flags]
        private enum tagOLECONTF
        {
            OLECONTF_EMBEDDINGS = 1,
            OLECONTF_LINKS = 2,
            OLECONTF_OTHERS = 4,
            OLECONTF_ONLYUSER = 8,
            OLECONTF_ONLYIFRUNNING = 16,
        }

        private enum GetWindowCommand : uint
        {
            First = 0,
            Last = 1,
            Next = 2,
            Previous = 3,
            Owner = 4,
            Child = 5,
            EnabledPopup = 6
        }

        private enum AccessibleObjectTypeId : uint
        {
            Window = 0x00000000,
            SysMenu = 0xFFFFFFFF,
            TitleBar = 0xFFFFFFFE,
            Menu = 0xFFFFFFFD,
            Client = 0xFFFFFFFC,
            VScroll = 0xFFFFFFFB,
            HScroll = 0xFFFFFFFA,
            SizeGrip = 0xFFFFFFF9,
            Caret = 0xFFFFFFF8,
            Cursor = 0xFFFFFFF7,
            Alert = 0xFFFFFFF6,
            Sound = 0xFFFFFFF5,
        }
        #endregion

        /// <summary>
        /// This method encapsulates all the details of getting
        /// the full length text in a StringBuffer and returns
        /// the StringBuffer contents as a string.
        /// </summary>
        /// <param name="hwnd">The handle to the window</param>
        /// <returns>Text of the window</returns>
        internal static string GetWindowText(IntPtr hwnd)
        {
            var length = GetWindowTextLength(hwnd) + 1;
            var buffer = new StringBuilder(length);
            GetWindowText(hwnd, buffer, length);

            return buffer.ToString();
        }

        /// <summary>
        /// Gets the shown style of the window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The value of the shown style of the window</returns>
        internal static WindowShowStyle GetWindowShowStyle(IntPtr hwnd)
        {
            var placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);

            GetWindowPlacement(hwnd, ref placement);

            return (WindowShowStyle)placement.showCmd;
        }

        /// <summary>
        /// Sets the shown style of the window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <param name="style">The style value to set the window style to.</param>
        internal static void SetWindowShowStyle(IntPtr hwnd, WindowShowStyle style)
        {
            ShowWindow(hwnd, (int)style);
        }

        /// <summary>
        /// This method encapsulates all the details of getting
        /// the full length class name in a StringBuffer and returns
        /// the StringBuffer contents as a string.
        /// </summary>
        /// <param name="hwnd">The handle to the window</param>
        /// <returns>Text of the window</returns>
        internal static string GetWindowClass(IntPtr hwnd)
        {
            const int maxCapacity = 255;

            var className = new StringBuilder(maxCapacity);
            var lRes = GetClassName(hwnd, className, maxCapacity);

            return lRes == 0 ? string.Empty : className.ToString();
        }

        /// <summary>
        /// Gets the window style.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>A numeric value representing the window style.</returns>
        internal static long GetWindowStyle(IntPtr hwnd)
        {
            var info = new WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            GetWindowInfo(hwnd, ref info);

            return Convert.ToInt64(info.dwStyle);
        }

        /// <summary>
        /// Compares the class names.
        /// </summary>
        /// <param name="hWnd">The hWND of the window if which the class name should be retrieved.</param>
        /// <param name="expectedClassName">Expected name of the class.</param>
        /// <returns><see langword="true"/> if the class name matches; otherwise, <see langword="false"/>.</returns>
        internal static bool CompareClassNames(IntPtr hWnd, string expectedClassName)
        {
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }

            if (string.IsNullOrEmpty(expectedClassName))
            {
                return false;
            }

            var className = GetWindowClass(hWnd);
            return className.Equals(expectedClassName);
        }

        /// <summary>
        /// Gets an item on a dialog.
        /// </summary>
        /// <param name="hwnd">The handle to the dialog.</param>
        /// <param name="dialogItemId">The ID of the item on the dialog.</param>
        /// <returns>The handle to the item in the dialog.</returns>
        internal static IntPtr GetDialogItem(IntPtr hwnd, int dialogItemId)
        {
            IntPtr dialogItemHwnd = GetDlgItem(hwnd, dialogItemId);
            return dialogItemHwnd;
        }

        /// <summary>
        /// Gets the owner handle of the specified window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The handle to the owner window.</returns>
        internal static IntPtr GetOwnerHandle(IntPtr hwnd)
        {
            IntPtr ownerHwnd = IntPtr.Zero;
            ownerHwnd = GetWindow(hwnd, GetWindowCommand.Owner);
            return ownerHwnd;
        }

        /// <summary>
        /// Gets a value indicating whether the specified window is valid.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns><see langword="true"/> if the window is valid; otherwise, <see langword="false"/>.</returns>
        internal static bool IsWindowValid(IntPtr hwnd)
        {
            return IsWindow(hwnd);
        }

        /// <summary>
        /// Gets the process ID for the specified window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The ID of the process for the window.</returns>
        internal static int GetProcessIdForWindow(IntPtr hwnd)
        {
            int windowProcessId = 0;
            GetWindowThreadProcessId(hwnd, out windowProcessId);
            return windowProcessId;
        }

        /// <summary>
        /// Gets the thread ID for the specified window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The ID of the thread for the window.</returns>
        internal static int GetThreadIdForWindow(IntPtr hwnd)
        {
            int windowProcessId = 0;
            int windowThreadId = GetWindowThreadProcessId(hwnd, out windowProcessId);
            return windowThreadId;
        }

        /// <summary>
        /// Sets focus to the specified window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <param name="parentWindowHwnd">The handle to the parent window.</param>
        /// <returns><see langword="true"/> if the focus was successfully set; otherwise, <see langword="false"/>.</returns>
        internal static bool SetFocusToWindow(IntPtr hwnd, IntPtr parentWindowHwnd)
        {
            bool focusWasSet = false;

            IntPtr topLevelWindowHwnd = hwnd;
            if (parentWindowHwnd != IntPtr.Zero)
            {
                topLevelWindowHwnd = parentWindowHwnd;
            }

            // Attach to the input thread of the window.
            int currentThreadId = GetCurrentThreadId();
            int windowThreadId = GetThreadIdForWindow(hwnd);
            AttachThreadInput(currentThreadId, windowThreadId, true);

            // Bring the top level window to the foreground if it is not
            // already there, then set focus to the child window, if this
            // is a child window.
            if (MsWindowsNativeMethods.GetForegroundWindow() != topLevelWindowHwnd)
            {
                focusWasSet = SetForegroundWindow(topLevelWindowHwnd);
            }

            if (parentWindowHwnd != IntPtr.Zero)
            {
                focusWasSet = MsWindowsNativeMethods.SetFocus(hwnd) != IntPtr.Zero;
            }

            // Always remember to detach from the input thread.
            MsWindowsNativeMethods.AttachThreadInput(currentThreadId, windowThreadId, false);
            return focusWasSet;
        }

        /// <summary>
        /// Gets the window extent for the specified window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The rectangle representing the window on the screen.</returns>
        internal static Rectangle GetWindowExtents(IntPtr hwnd)
        {
            RECT windowRect = new RECT();
            GetWindowRect(hwnd, ref windowRect);
            return Rectangle.FromLTRB(windowRect.Left, windowRect.Top, windowRect.Right, windowRect.Bottom);
        }

        /// <summary>
        /// Gets a screenshot of the specified window.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>A <see cref="System.Drawing.Image"/> containing the screenshot.</returns>
        internal static System.Drawing.Image GetWindowImage(IntPtr hwnd)
        {
            Rectangle windowExtents = GetWindowExtents(hwnd);
            Bitmap imageBitmap = new Bitmap(windowExtents.Width, windowExtents.Height, PixelFormat.Format48bppRgb);
            Graphics imageGraphics = Graphics.FromImage(imageBitmap);
            IntPtr hdc = imageGraphics.GetHdc();

            PrintWindow(hwnd, hdc, 0);
            
            imageGraphics.ReleaseHdc(hdc);
            imageGraphics.Flush();
            imageGraphics.Dispose();

            IntPtr hBitmap = imageBitmap.GetHbitmap();
            System.Drawing.Image windowImage = System.Drawing.Image.FromHbitmap(hBitmap);

            DeleteObject(hBitmap);
            return windowImage;
        }

        /// <summary>
        /// Gets the accessible object from the specified window handle.
        /// </summary>
        /// <param name="hwnd">The handle to the window.</param>
        /// <returns>The accessible object from the window.</returns>
        internal static object GetAccessibleObjectFromWindow(IntPtr hwnd)
        {
            object accessibleObject = null;
            AccessibleObjectFromWindow(hwnd, AccessibleObjectTypeId.Window, ref IID_IAccessible, ref accessibleObject);
            return accessibleObject;
        }

        /// <summary>
        /// Gets all of the accessible children of the specified object.
        /// </summary>
        /// <param name="accessibleObject">The object on which to find the children.</param>
        /// <param name="children">An array containing the child objects.</param>
        /// <returns>The number of children retrieved.</returns>
        internal static int GetAccessibleChildren(IAccessible accessibleObject, object[] children)
        {
            int expectedChildCount = children.Length;
            int actualChildCount = 0;
            MsWindowsNativeMethods.AccessibleChildren(accessibleObject, 0, expectedChildCount, children, ref actualChildCount);
            return actualChildCount;
        }

        #region P/Invoke DllImports
        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumWindows(EnumWindowProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildWindowProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetDlgCtrlID(IntPtr hwndCtl);

        [DllImport("user32.dll")]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int SendMessageTimeout(IntPtr hWnd, int msg, int wParam, int lParam, int fuFlags, int uTimeout, ref int lpdwResult);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("User32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr window, out int processId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetClassNameA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int GetClassName(IntPtr handleToWindow, StringBuilder className, int maxClassNameLength);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr handleToWindow, StringBuilder windowText, int maxTextLength);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetDlgItem(IntPtr handleToWindow, int controlId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCommand uCmd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr hwnd, AccessibleObjectTypeId id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        [DllImport("oleacc.dll")]
        private static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] object[] rgvarChildren, ref int pcObtained);
        #endregion

        #region P/Invoke structures
        /// <summary>
        /// Represents mouse input.
        /// </summary>
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal uint mouseData;
            internal MouseEventFlags dwFlags;
            internal uint time;
            internal IntPtr dwExtraInfo;
        }

        /// <summary>
        /// Represents keyboard input.
        /// </summary>
        internal struct KEYBDINPUT
        {
            internal short wVk;
            internal short wScan;
            internal KeyEventFlags dwFlags;
            internal uint time;
            internal IntPtr dwExtraInfo;
        }

        /// <summary>
        /// Represents hardware input.
        /// </summary>
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        /// <summary>
        /// Represents input values for the SendInput API.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 28)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            internal InputType type;
            [FieldOffset(4)]
            internal MOUSEINPUT mi;
            [FieldOffset(4)]
            internal KEYBDINPUT ki;
            [FieldOffset(4)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;
        }
        #endregion
    }
}
