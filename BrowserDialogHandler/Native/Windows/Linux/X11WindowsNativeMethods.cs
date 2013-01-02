// <copyright file="X11WindowsNativeMethods.cs" company="BrowserDialogHandler Project">
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
using System.Runtime.InteropServices;

namespace BrowserDialogHandler.Native.Windows.Linux
{
    /// <summary>
    /// Wraps X11 Windows native methods.
    /// </summary>
    internal static class X11WindowsNativeMethods
    {
        internal static readonly int AllProcesses = -1;
        private const string MessageActivateWindow = "_NET_ACTIVE_WINDOW";
        private const string MessageCloseWindow = "_NET_CLOSE_WINDOW";
        private const string PropertyWindowState = "WM_STATE";
        private const string PropertyDesktopIndex = "_NET_WM_DESKTOP";
        private const string PropertyProcessId = "_NET_WM_PID";
        private const string PropertyWindowType = "_NET_WM_WINDOW_TYPE";
        private const string PropertyWindowFrameExtents = "_NET_FRAME_EXTENTS";
        private const string PropertyTransientFor = "WM_TRANSIENT_FOR";
        private const string AtomDialogWindowType = "_NET_WM_WINDOW_TYPE_DIALOG";
        private const uint AnyPropertyType = 0;

        private static XErrorHandler originalEventHandler;

        private delegate int XErrorHandler(IntPtr display, ref XErrorEvent errorEvent);

        private enum XMapState : int
        {
            IsUnmapped,
            IsUnviewable,
            IsViewable
        }

        [Flags]
        private enum EventMask : int
        {
            KeyPressMask,
            KeyReleaseMask,  
            ButtonPressMask,
            ButtonReleaseMask,
            EnterWindowMask,
            LeaveWindowMask,
            PointerMotionMask,
            PointerMotionHintMask,
            Button1MotionMask,
            Button2MotionMask,
            Button3MotionMask,
            Button4MotionMask,
            Button5MotionMask,
            ButtonMotionMask,
            KeymapStateMask,
            ExposureMask,
            VisibilityChangeMask,
            StructureNotifyMask,
            ResizeRedirectMask,
            SubstructureNotifyMask,
            SubstructureRedirectMask,
            FocusChangeMask,
            PropertyChangeMask,
            ColormapChangeMask,
            OwnerGrabButtonMask
        }
    
        private enum EventType
        {
            KeyPress = 2,
            KeyRelease = 3,
            ButtonPress = 4,
            ButtonRelease = 5,
            MotionNotify = 6,
            EnterNotify = 7,
            LeaveNotify = 8,
            FocusIn = 9,
            FocusOut = 10,
            KeymapNotify = 11,
            Expose = 12,
            GraphicsExpose = 13,
            NoExpose = 14,
            VisibilityNotify = 15,
            CreateNotify = 16,
            DestroyNotify = 17,
            UnmapNotify = 18,
            MapNotify = 19,
            MapRequest = 20,
            ReparentNotify = 21,
            ConfigureNotify = 22,
            ConfigureRequest = 23,
            GravityNotify = 24,
            ResizeRequest = 25,
            CirculateNotify = 26,
            CirculateRequest = 27,
            PropertyNotify = 28,
            SelectionClear = 29,
            SelectionRequest = 30,
            SelectionNotify = 31,
            ColormapNotify = 32,
            ClientMessage = 33,
            MappingNotify = 34,
            GenericEvent = 35
        }
    
        private enum XErrorCode : byte
        {
            Success = 0,
            BadRequest = 1,
            BadValue = 2,
            BadWindow = 3,
            BadPixmap = 4,
            BadAtom = 5,
            BadCursor = 6,
            BadFont = 7,
            BadMatch = 8,
            BadDrawable = 9,
            BadAccess = 10,
            BadAlloc = 11,
            BadColor = 12,
            BadGC = 13,
            BadIDChoice = 14,
            BadName = 15,
            BadLength = 16, 
            BadImplementation = 17
        }

        /// <summary>
        /// Opens the server connection.
        /// </summary>
        /// <returns>A pointer to the X display.</returns>
        internal static IntPtr OpenServerConnection()
        {
            return XOpenDisplay(null);
        }
        
        /// <summary>
        /// Closes the server connection.
        /// </summary>
        /// <param name="display">A pointer to the display.</param>
        internal static void CloseServerConnection(IntPtr display)
        {
            XSync(display, false);
            XCloseDisplay(display);
        }

        /// <summary>
        /// Flushes the display.
        /// </summary>
        /// <param name="display">The pointer to the X display.</param>
        internal static void FlushDisplay(IntPtr display)
        {
            XFlush(display);
        }
        
        /// <summary>
        /// Gets the class of a window.
        /// </summary>
        /// <param name="windowPointer">The pointer to the window.</param>
        /// <returns>The class of the window.</returns>
        internal static string GetWindowClass(IntPtr windowPointer)
        {
            string windowClass = string.Empty;
            using (XServerConnection server = new XServerConnection())
            {
                IntPtr display = server.Display;
                XClassHint hints = new XClassHint();
                int success = XGetClassHint(display, windowPointer, ref hints);
                if (success != 0)
                {
                    windowClass = Marshal.PtrToStringAnsi(hints.res_class);
                    XFree(hints.res_class);
                    XFree(hints.res_name);
                }
            }

            return windowClass;
        }
        
        /// <summary>
        /// Gets the text of the window.
        /// </summary>
        /// <param name="windowPointer">The pointer to the window.</param>
        /// <returns>The text of the window.</returns>
        internal static string GetWindowText(IntPtr windowPointer)
        {
            string windowText = string.Empty;
            using (XServerConnection server = new XServerConnection())
            {
                IntPtr display = server.Display;
                IntPtr namePointer = IntPtr.Zero;
                int success = XFetchName(display, windowPointer, ref namePointer);
                string name = Marshal.PtrToStringAuto(namePointer);
                if (success != 0)
                {
                    windowText = name;
                }

                XFree(namePointer);
            }

            return windowText;
        }
        
        /// <summary>
        /// Gets the process ID for the window.
        /// </summary>
        /// <param name="windowPointer">The pointer to the window.</param>
        /// <returns>The process ID for the window.</returns>
        internal static int GetProcessIdForWindow(IntPtr windowPointer)
        {
            int procId = 0;
            using (XServerConnection server = new XServerConnection())
            {
                procId = GetProcessId(server.Display, windowPointer);
            }

            return procId;
        }
        
        /// <summary>
        /// Gets a value indicating whether the window is valid.
        /// </summary>
        /// <param name="window">The pointer to the window.</param>
        /// <returns><see langword="true"/> if the window is valid; otherwise, <see langword="false"/>.</returns>
        internal static bool IsWindowValid(IntPtr window)
        {
            bool validWindow = false;
            XWindowAttributes attributes = new XWindowAttributes();
            XErrorHandler handler = new XErrorHandler(IgnoreBadWindowHandler);
            originalEventHandler = XSetErrorHandler(handler);
            using (XServerConnection serverConnection = new XServerConnection())
            {
                validWindow = XGetWindowAttributes(serverConnection.Display, window, out attributes) != 0;
            }

            XSetErrorHandler(originalEventHandler);
            originalEventHandler = null;
            return validWindow;
        }
        
        /// <summary>
        /// Gets a value indicating if the window is viewable.
        /// </summary>
        /// <param name="window">A pointer to the window.</param>
        /// <returns><see langword="true"/> if the window is viewable; otherwise, <see langword="false"/>.</returns>
        internal static bool IsWindowViewable(IntPtr window)
        {
            bool viewableWindow = false;
            XWindowAttributes attributes = new XWindowAttributes();
            using (XServerConnection serverConnection = new XServerConnection())
            {
                XGetWindowAttributes(serverConnection.Display, window, out attributes);
            }

            viewableWindow = attributes.map_state == XMapState.IsViewable;
            return viewableWindow;
        }
        
        /// <summary>
        /// Activates the specified window.
        /// </summary>
        /// <param name="window">The pointer to the window.</param>
        internal static void ActivateWindow(IntPtr window)
        {
            using (XServerConnection serverConnection = new XServerConnection())
            {
                SendClientMessage(serverConnection.Display, window, MessageActivateWindow, new int[5] { 0, 0, 0, 0, 0 });
                XMapRaised(serverConnection.Display, window);
                XFlush(serverConnection.Display);
            }
        }
        
        /// <summary>
        /// Closes the specified window.
        /// </summary>
        /// <param name="window">The pointer to the window.</param>
        internal static void CloseWindow(IntPtr window)
        {
            using (XServerConnection serverConnection = new XServerConnection())
            {
                SendClientMessage(serverConnection.Display, window, MessageCloseWindow, new int[5] { 0, 0, 0, 0, 0 });
                XFlush(serverConnection.Display);
            }
        }
        
        /// <summary>
        /// Gets all of the top-level windows for the specified process ID.
        /// </summary>
        /// <param name="processId">The ID of the process.</param>
        /// <returns>A list of pointers to the process's top-level windows.</returns>
        internal static List<IntPtr> FindTopLevelWindowsForProcess(int processId)
        {
            List<IntPtr> windowList = new List<IntPtr>();
            using (XServerConnection serverConnection = new XServerConnection())
            {
                IntPtr rootWindow = GetRootWindow(serverConnection.Display);
                windowList = GetProcessTopLevelWindows(serverConnection.Display, rootWindow, processId);
            }

            return windowList;
        }
        
        /// <summary>
        /// Gets a value indicating whether the window is a dialog window.
        /// </summary>
        /// <param name="window">The pointer to the window.</param>
        /// <returns><see langword="true"/> if the window is a dialog window; otherwise, <see langword="false"/>.</returns>
        internal static bool IsDialogWindow(IntPtr window)
        {
            bool windowIsDialog = false;
            using (XServerConnection serverConnection = new XServerConnection())
            {
                int dialogAtom = (int)XInternAtom(serverConnection.Display, AtomDialogWindowType, true);
                uint returnType = 0;
                ulong itemCount = 0L;
                IntPtr prop_return = GetWindowProperty(serverConnection.Display, window, PropertyWindowType, 0, 4, out returnType, out itemCount);
                if (prop_return != IntPtr.Zero && itemCount > 0)
                {
                    // Possible window type Atoms are returned in an
                    // array, with the preferred type in the first 
                    // element. Assume that if element 0 is dialog,
                    // the window will be a dialog.
                    int[] windowTypes = new int[itemCount];
                    Marshal.Copy(prop_return, windowTypes, 0, (int)itemCount);
                    windowIsDialog = windowTypes[0] == dialogAtom;
                    XFree(prop_return);
                }
            }

            return windowIsDialog;
        }
        
        /// <summary>
        /// Gets the transient value for the window.
        /// </summary>
        /// <param name="window">The name of the window.</param>
        /// <returns>A pointer to the window.</returns>
        internal static IntPtr GetTransientForWindow(IntPtr window)
        {
            IntPtr transientForWindowHandle = IntPtr.Zero;
            using (XServerConnection serverConnection = new XServerConnection())
            {
                int success = XGetTransientForHint(serverConnection.Display, window, out transientForWindowHandle);
                if (success == 0 || transientForWindowHandle == window)
                {
                    transientForWindowHandle = IntPtr.Zero;
                }
            }

            return transientForWindowHandle;
        }
        
        /// <summary>
        /// Gets the rectangle of the specified window.
        /// </summary>
        /// <param name="window">A pointer to the window.</param>
        /// <returns>The rectangle defining the display of the window.</returns>
        internal static Rectangle GetWindowRectangle(IntPtr window)
        {
            XWindowAttributes attributes;
            int absoluteX = 0;
            int absoluteY = 0;
            IntPtr childWindow = IntPtr.Zero;
            using (XServerConnection serverConnection = new XServerConnection())
            {
                int success = XGetWindowAttributes(serverConnection.Display, window, out attributes);
                if (success != 0)
                {
                    XTranslateCoordinates(serverConnection.Display, window, attributes.root, attributes.x, attributes.y, out absoluteX, out absoluteY, out childWindow);
                }
            }

            return new Rectangle(absoluteX - attributes.x, absoluteY - attributes.y, attributes.width, attributes.height);
        }
        
        /// <summary>
        /// Gets the frame extents for the specified window.
        /// </summary>
        /// <param name="window">A pointer to the window.</param>
        /// <returns>The <see cref="WindowManagerFrameExtents"/> of the window.</returns>
        internal static WindowManagerFrameExtents GetFrameExtents(IntPtr window)
        {
            WindowManagerFrameExtents extents = WindowManagerFrameExtents.Empty;
            using (XServerConnection serverConnection = new XServerConnection())
            {
                IntPtr prop_return = GetWindowProperty(serverConnection.Display, window, PropertyWindowFrameExtents, 0, 4 * 32);
                if (prop_return != IntPtr.Zero)
                {
                    int[] returnedExtents = new int[4] { 0, 0, 0, 0 };
                    Marshal.Copy(prop_return, returnedExtents, 0, 4);
                    extents = new WindowManagerFrameExtents();
                    extents.Left = returnedExtents[0];
                    extents.Right = returnedExtents[1];
                    extents.Top = returnedExtents[2];
                    extents.Bottom = returnedExtents[3];
                    XFree(prop_return);
                }
            }

            return extents;
        }

        private static IntPtr GetRootWindow(IntPtr display)
        {
            IntPtr rootWindow = IntPtr.Zero;
            int defaultScreen = XDefaultScreen(display);
            rootWindow = XRootWindow(display, defaultScreen);
            return rootWindow;
        }
        
        private static void SendClientMessage(IntPtr display, IntPtr window, string message, int[] data)
        {
            EventMask mask = EventMask.SubstructureRedirectMask | EventMask.SubstructureNotifyMask;
            XClientMessageEvent window_event = new XClientMessageEvent();
            window_event.type = EventType.ClientMessage;
            window_event.serial = 0;
            window_event.send_event = true;
            window_event.format = 32;
            window_event.window = window;
            window_event.message_type = XInternAtom(display, message, false);
            window_event.data.l.l0 = data[0];
            window_event.data.l.l1 = data[1];
            window_event.data.l.l2 = data[2];
            window_event.data.l.l3 = data[3];
            window_event.data.l.l4 = data[4];
            
            IntPtr rootWindow = GetRootWindow(display);
            
            XSendEvent(display, rootWindow, false, mask, ref window_event);
        }
        
        private static IntPtr GetWindowProperty(IntPtr display, IntPtr windowPointer, string propertyName, int offset, int length)
        {
            uint returnType = 0;
            return GetWindowProperty(display, windowPointer, propertyName, offset, length, out returnType);
        }

        private static IntPtr GetWindowProperty(IntPtr display, IntPtr windowPointer, string propertyName, int offset, int length, out uint returnType)
        {
            ulong itemCount = 0;
            return GetWindowProperty(display, windowPointer, propertyName, offset, length, out returnType, out itemCount);
        }
        
        private static IntPtr GetWindowProperty(IntPtr display, IntPtr windowPointer, string propertyName, int offset, int length, out uint returnType, out ulong itemCount)
        {
            uint wmPropertyAtom = XInternAtom(display, propertyName, true);
            int format_return = 0;
            ulong byte_count = 0L;
            IntPtr prop_return = IntPtr.Zero;
            XGetWindowProperty(display, windowPointer, wmPropertyAtom, offset, length, false, AnyPropertyType, out returnType, out format_return, out itemCount, out byte_count, out prop_return);
            return prop_return;
        }
        
        private static bool WindowHasStateAtom(IntPtr display, IntPtr windowPointer)
        {
            bool hasState = false;
            uint returnType = 0;
            IntPtr prop_return = GetWindowProperty(display, windowPointer, PropertyWindowState, 0, 0, out returnType);
            if (prop_return != IntPtr.Zero)
            {
                XFree(prop_return);
            }

            if (returnType != 0)
            {
                hasState = true;
            }

            return hasState;
        }
        
        private static int GetDesktopIndex(IntPtr display, IntPtr windowPointer)
        {
            int desktopId = -2;
            IntPtr prop_return = GetWindowProperty(display, windowPointer, PropertyDesktopIndex, 0, 4);
            
            if (prop_return != IntPtr.Zero)
            {
                desktopId = Marshal.ReadInt32(prop_return);
                XFree(prop_return);
            }

            return desktopId;
        }
        
        private static int GetProcessId(IntPtr display, IntPtr windowPointer)
        {
            int procId = 0;
            IntPtr prop_return = GetWindowProperty(display, windowPointer, PropertyProcessId, 0, 4);
            
            if (prop_return != IntPtr.Zero)
            {
                procId = Marshal.ReadInt32(prop_return);
                XFree(prop_return);
            }

            return procId;
        }
        
        private static int IgnoreBadWindowHandler(IntPtr display, ref XErrorEvent errorEvent)
        {
            if (errorEvent.error_code != XErrorCode.BadWindow && originalEventHandler != null)
            {
                originalEventHandler(display, ref errorEvent);
            }

            return 0;
        }
        
        private static List<IntPtr> GetProcessTopLevelWindows(IntPtr display, IntPtr rootWindow, int processId)
        {
            List<IntPtr> windowList = new List<IntPtr>();
            IntPtr rootReturn = IntPtr.Zero;
            IntPtr parentReturn = IntPtr.Zero;
            IntPtr childrenReturn = IntPtr.Zero;
            int childrenCount = 0;
            
            int status = XQueryTree(display, rootWindow, out rootReturn, out parentReturn, out childrenReturn, out childrenCount);
            if (status != 0 && childrenCount > 0)
            {
                IntPtr[] windows = new IntPtr[childrenCount];
                Marshal.Copy(childrenReturn, windows, 0, childrenCount);
                foreach (IntPtr windowPointer in windows)
                {
                    bool hasState = WindowHasStateAtom(display, windowPointer);
                    int desktopIndex = GetDesktopIndex(display, windowPointer);
                    if (hasState && (desktopIndex >= 0))
                    {
                        int windowProcessId = GetProcessId(display, windowPointer);
                        if (processId == windowProcessId || processId == AllProcesses)
                        {
                            windowList.Add(windowPointer);
                        }
                    }

                    windowList.AddRange(GetProcessTopLevelWindows(display, windowPointer, processId));
                }
            }

            XFree(childrenReturn);
            return windowList;
        }

        [DllImport("libX11")]
        private static extern IntPtr XOpenDisplay(string dispName);

        [DllImport("libX11")]
        private static extern void XCloseDisplay(IntPtr display);

        [DllImport("libX11")]
        private static extern void XFlush(IntPtr display);

        [DllImport("libX11")]
        private static extern int XSync(IntPtr display, bool discard);

        [DllImport("libX11")]
        private static extern int XDefaultScreen(IntPtr display);

        [DllImport("libX11")]
        private static extern IntPtr XRootWindow(IntPtr display, int screen);

        [DllImport("libX11")]
        private static extern int XQueryTree(IntPtr display, IntPtr window, out IntPtr rootWindow, out IntPtr parentWindow, out IntPtr children, out int childrenCount);

        [DllImport("libX11")]
        private static extern int XFetchName(IntPtr display, IntPtr window, ref IntPtr name);

        [DllImport("libX11")]
        private static extern int XGetWindowAttributes(IntPtr display, IntPtr window, out XWindowAttributes attributes);

        [DllImport("libX11")]
        private static extern int XFree(IntPtr data);

        [DllImport("libX11")]
        private static extern void XMapRaised(IntPtr display, IntPtr window);

        [DllImport("libX11")]
        private static extern uint XInternAtom(IntPtr display, string atom_name, bool only_if_exists);

        [DllImport("libX11")]
        private static extern int XGetWindowProperty(IntPtr display, IntPtr window, uint property, int long_offset, int long_length, bool delete_property, uint req_type, out uint actual_type_return, out int actual_format_return, out ulong nitems_return, out ulong bytes_after_return, out IntPtr prop_return);

        [DllImport("libX11")]
        private static extern int XGetClassHint(IntPtr display, IntPtr window, ref XClassHint hints);

        [DllImport("libX11")]
        private static extern int XGetTransientForHint(IntPtr display, IntPtr window, out IntPtr transient_for_window);

        [DllImport("libX11")]
        private static extern int XSendEvent(IntPtr display, IntPtr window, bool propagate, EventMask event_mask, ref XClientMessageEvent event_send);

        [DllImport("libX11")]
        private static extern bool XTranslateCoordinates(IntPtr display, IntPtr source_window, IntPtr dest_window, int sourceX, int sourceY, out int destinationX, out int destinationY, out IntPtr child);

        [DllImport("libX11")]
        private static extern XErrorHandler XSetErrorHandler(XErrorHandler handler);

        [StructLayout(LayoutKind.Sequential)]
        private struct XClientMessageEvent
        {
            internal EventType type;
            internal uint serial;
            internal bool send_event;
            internal IntPtr display;
            internal IntPtr window;
            internal uint message_type;
            internal int format;
            internal Message data;

            [StructLayout(LayoutKind.Sequential)]
            internal struct ByteMessage
            {
                internal byte b0;
                internal byte b1;
                internal byte b2;
                internal byte b3;
                internal byte b4;
                internal byte b5;
                internal byte b6;
                internal byte b7;
                internal byte b8;
                internal byte b9;
                internal byte b10;
                internal byte b11;
                internal byte b12;
                internal byte b13;
                internal byte b14;
                internal byte b15;
                internal byte b16;
                internal byte b17;
                internal byte b18;
                internal byte b19;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct ShortMessage
            {
                internal short s0;
                internal short s1;
                internal short s2;
                internal short s3;
                internal short s4;
                internal short s5;
                internal short s6;
                internal short s7;
                internal short s8;
                internal short s9;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct LongMessage
            {
                internal int l0;
                internal int l1;
                internal int l2;
                internal int l3;
                internal int l4;
            }

            [StructLayout(LayoutKind.Explicit)]
            internal struct Message
            {
                [FieldOffset(0)]
                internal ByteMessage b;
                [FieldOffset(0)]
                internal ShortMessage s;
                [FieldOffset(0)]
                internal LongMessage l;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XClassHint
        {
            internal IntPtr res_name;
            internal IntPtr res_class;
        }

        private struct XWindowAttributes
        {
            internal int x;
            internal int y;
            internal int width;
            internal int height;
            internal int border_width;
            internal int depth;
            internal IntPtr visual;
            internal IntPtr root;
            internal int c_class;
            internal int bit_gravity;
            internal int win_gravity;
            internal int backing_store;
            internal uint backing_planes;
            internal uint backing_pixel;
            internal int save_under;
            internal IntPtr colormap;
            internal int map_installed;
            internal XMapState map_state;
            internal int all_event_masks;
            internal int your_event_mask;
            internal int do_not_propagate_mask;
            internal int override_redirect;
            internal IntPtr screen;
        }

        private struct XErrorEvent
        {
            internal int type;
            internal IntPtr display;
            internal uint resourceid;
            internal uint serial;
            internal XErrorCode error_code;
            internal byte request_code;
            internal byte minor_code;
        }
    }
}
