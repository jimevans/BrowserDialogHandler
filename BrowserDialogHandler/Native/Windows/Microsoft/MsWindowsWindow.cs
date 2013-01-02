// <copyright file="MsWindowsWindow.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native.Windows.Microsoft
{
    /// <summary>
    /// Represents a window on Microsoft Windows.
    /// </summary>
    internal class MsWindowsWindow : Window
    {
        private static readonly List<string> DialogClassNames = new List<string>();
        private IntPtr handle = IntPtr.Zero;
        private IntPtr parentWindowHandle = IntPtr.Zero;
        private MsaaObject accessibleObject = null;
        private int itemId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsWindowsWindow"/> class, given a pointer to the window handle.
        /// </summary>
        /// <param name="handle">The handle to this window.</param>
        internal MsWindowsWindow(IntPtr handle)
            : this(handle, IntPtr.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsWindowsWindow"/> class, given pointers to the handles of this window and its parent.
        /// </summary>
        /// <param name="handle">The handle to this window.</param>
        /// <param name="parentWindowHandle">The handle to this window's parent.</param>
        internal MsWindowsWindow(IntPtr handle, IntPtr parentWindowHandle)
        {
            this.handle = handle;
            this.parentWindowHandle = parentWindowHandle;
            if (DialogClassNames.Count == 0)
            {
                DialogClassNames.Add("#32770");
                DialogClassNames.Add("MozillaDialogClass");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MsWindowsWindow"/> class, given an Active Accessibility object for the window.
        /// </summary>
        /// <param name="accessibleObject">The Active Accessibility object for this window.</param>
        /// <param name="parentWindowHandle">The handle to this window's parent.</param>
        /// <param name="itemId">The item ID for this object.</param>
        internal MsWindowsWindow(MsaaObject accessibleObject, IntPtr parentWindowHandle, int itemId)
        {
            this.accessibleObject = accessibleObject;
            this.parentWindowHandle = parentWindowHandle;
            this.itemId = itemId;
            this.EnumerationMethod = WindowEnumerationMethod.AssistiveTechnologyApi;
            this.ChildEnumerationMethod = WindowEnumerationMethod.AssistiveTechnologyApi;
        }

        /// <inheritdoc />
        public override IntPtr Handle
        {
            get { return this.handle; }
        }

        /// <inheritdoc />
        public override IntPtr ParentHandle
        {
            get { return this.parentWindowHandle; }
        }

        /// <inheritdoc />
        public override IntPtr OwnerHandle
        {
            get { return MsWindowsNativeMethods.GetOwnerHandle(this.handle); }
        }

        /// <inheritdoc />
        public override string ClassName
        {
            get
            {
                string windowClass = string.Empty;
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    windowClass = MsWindowsNativeMethods.GetWindowClass(this.handle);
                }
                else
                {
                    if (this.Exists)
                    {
                        windowClass = this.AccessibleObject.Role.ToString();
                    }
                }

                return windowClass;
            }
        }

        /// <inheritdoc />
        public override string Text
        {
            get
            {
                string windowText = string.Empty;
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    windowText = MsWindowsNativeMethods.GetWindowText(this.handle);
                }
                else
                {
                    if (this.Exists)
                    {
                        windowText = this.AccessibleObject.Name;
                    }
                }

                return windowText;
            }
        }

        /// <inheritdoc />
        public override bool Visible
        {
            get
            {
                bool isVisible = false;
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    isVisible = MsWindowsNativeMethods.IsWindowVisible(this.handle);
                }
                else
                {
                    if (this.Exists)
                    {
                        isVisible = this.AccessibleObject.StateSet.Contains(AccessibleState.Visible);
                    }
                }

                return isVisible;
            }
        }

        /// <inheritdoc />
        public override bool Enabled
        {
            get
            {
                bool isEnabled = false;
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    isEnabled = MsWindowsNativeMethods.IsWindowEnabled(this.handle);
                }
                else
                {
                    if (this.Exists)
                    {
                        isEnabled = this.AccessibleObject.StateSet.Contains(AccessibleState.Enabled);
                    }
                }

                return isEnabled;
            }
        }

        /// <inheritdoc />
        public override bool Exists
        {
            get
            {
                bool windowExists = false;
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    windowExists = MsWindowsNativeMethods.IsWindowValid(this.handle);
                }
                else
                {
                    windowExists = this.AccessibleObject != null;
                }

                return windowExists;
            }
        }

        /// <inheritdoc />
        public override bool IsTopLevelWindow
        {
            get { return this.parentWindowHandle == IntPtr.Zero; }
        }

        /// <inheritdoc />
        public override bool IsDialog
        {
            get { return DialogClassNames.Contains(this.ClassName); }
        }

        /// <inheritdoc />
        public override long Style
        {
            get { return MsWindowsNativeMethods.GetWindowStyle(this.handle); }
        }

        /// <inheritdoc />
        public override string StyleDescriptor
        {
            get { return this.Style.ToString("X"); }
        }

        /// <inheritdoc />
        public override int ProcessId
        {
            get { return MsWindowsNativeMethods.GetProcessIdForWindow(this.handle); }
        }

        /// <inheritdoc />
        public override WindowShowStyle WindowStyle
        {
            get { return MsWindowsNativeMethods.GetWindowShowStyle(this.handle); }
            set { MsWindowsNativeMethods.SetWindowShowStyle(this.handle, value); }
        }

        /// <inheritdoc />
        public override bool IsPressable
        {
            get 
            {
                string pushButtonClass = WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, EnumerationMethod == WindowEnumerationMethod.WindowManagementApi);
                bool isClickableObject = false;
                isClickableObject = !this.IsTopLevelWindow && this.ClassName == pushButtonClass;
                return isClickableObject;
            }
        }

        /// <inheritdoc />
        public override int ItemId
        {
            get
            {
                int idOfItem = 0;
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    idOfItem = MsWindowsNativeMethods.GetDlgCtrlID(this.handle);
                }
                else
                {
                    idOfItem = this.itemId;
                }

                return idOfItem;
            }
        }

        /// <inheritdoc />
        internal override AssistiveTechnologyObject AccessibleObject
        {
            get
            {
                if (this.accessibleObject == null && this.IsTopLevelWindow)
                {
                    this.accessibleObject = new MsaaObject(this.handle);
                }

                return this.accessibleObject;
            }
        }

        /// <summary>
        /// Gets all top-level windows in the system.
        /// </summary>
        /// <returns>A list of <see cref="Window"/> objects representing the top-level windows in the system.</returns>
        public static IList<Window> GetAllTopLevelWindows()
        {
            MsWindowsEnumerator enumerator = new MsWindowsEnumerator();
            IList<Window> allWindowList = enumerator.GetWindows(window => true);
            return allWindowList;
        }

        /// <inheritdoc />
        public override void Press()
        {
            if (this.IsPressable)
            {
                if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                {
                    MsWindowsNativeMethods.SendMessage(this.Handle, MsWindowsNativeMethods.WM_ACTIVATE, MsWindowsNativeMethods.MA_ACTIVATE, 0);
                    MsWindowsNativeMethods.SendMessage(this.Handle, MsWindowsNativeMethods.BM_CLICK, 0, 0);
                }
                else
                {
                    if (this.AccessibleObject != null && this.AccessibleObject.SupportsActions)
                    {
                        this.AccessibleObject.DoAction(0);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override bool SetFocus()
        {
            return MsWindowsNativeMethods.SetFocusToWindow(this.handle, this.parentWindowHandle);
        }

        /// <inheritdoc />
        public override IList<Window> GetChildWindows(WindowCriteriaConstraint constraint)
        {
            IList<Window> childWindowList = new List<Window>();
            if (this.ChildEnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
            {
                MsWindowsEnumerator enumerator = new MsWindowsEnumerator();
                childWindowList = enumerator.GetChildWindows(this.handle, constraint);
            }
            else
            {
                if (this.AccessibleObject != null)
                {
                    IList<AssistiveTechnologyObject> accessibleChildren = this.AccessibleObject.GetChildrenByRole(AccessibleRole.AnyRole, true, true);
                    foreach (AssistiveTechnologyObject accessibleChild in accessibleChildren)
                    {
                        int itemIndex = accessibleChildren.IndexOf(accessibleChild);
                        MsWindowsWindow foundWindow = new MsWindowsWindow((MsaaObject)accessibleChild, this.handle, itemIndex);
                        if (constraint(foundWindow))
                        {
                            childWindowList.Add(foundWindow);
                        }
                        else
                        {
                            foundWindow.Dispose();
                        }
                    }
                }
            }

            return childWindowList;
        }

        /// <inheritdoc />
        public override void ForceClose()
        {
            MsWindowsNativeMethods.SendMessage(this.handle, MsWindowsNativeMethods.WM_CLOSE, 0, 0);
        }

        /// <inheritdoc />
        public override void SendKeystrokes(string keystrokes)
        {
            if (this.ChildEnumerationMethod == WindowEnumerationMethod.AssistiveTechnologyApi)
            {
                if (this.AccessibleObject != null)
                {
                    this.AccessibleObject.SetFocus();
                }

                foreach (var c in keystrokes)
                {
                    List<MsWindowsNativeMethods.INPUT> keySequence = new List<MsWindowsNativeMethods.INPUT>();
                    MsWindowsNativeMethods.INPUT keyDown = new MsWindowsNativeMethods.INPUT();
                    keyDown.type = MsWindowsNativeMethods.InputType.Keyboard;
                    keyDown.ki.wVk = Convert.ToInt16(char.ToUpper(c));
                    keyDown.ki.dwFlags = MsWindowsNativeMethods.KeyEventFlags.None;
                    keyDown.ki.time = 0;
                    keyDown.ki.wScan = 0;
                    keyDown.ki.dwExtraInfo = IntPtr.Zero;

                    MsWindowsNativeMethods.INPUT keyUp = new MsWindowsNativeMethods.INPUT();
                    keyUp.type = MsWindowsNativeMethods.InputType.Keyboard;
                    keyUp.ki.wVk = Convert.ToInt16(char.ToUpper(c));
                    keyUp.ki.dwFlags = MsWindowsNativeMethods.KeyEventFlags.KeyUp;
                    keyUp.ki.time = 0;
                    keyUp.ki.wScan = 0;
                    keyUp.ki.dwExtraInfo = IntPtr.Zero;

                    keySequence.Add(keyDown);
                    keySequence.Add(keyUp);

                    if (c >= 'A' && c <= 'Z')
                    {
                        MsWindowsNativeMethods.INPUT shiftKeyDown = new MsWindowsNativeMethods.INPUT();
                        shiftKeyDown.type = MsWindowsNativeMethods.InputType.Keyboard;
                        shiftKeyDown.ki.wVk = (short)MsWindowsNativeMethods.VK.Shift;

                        MsWindowsNativeMethods.INPUT shiftKeyUp = new MsWindowsNativeMethods.INPUT();
                        shiftKeyUp.type = MsWindowsNativeMethods.InputType.Keyboard;
                        shiftKeyUp.ki.wVk = (short)MsWindowsNativeMethods.VK.Shift;
                        shiftKeyUp.ki.dwFlags = MsWindowsNativeMethods.KeyEventFlags.KeyUp;

                        keySequence.Insert(0, shiftKeyDown);
                        keySequence.Add(shiftKeyUp);
                    }

                    this.SetFocus();
                    MsWindowsNativeMethods.SendInput((uint)keySequence.Count, keySequence.ToArray(), Marshal.SizeOf(keySequence[0]));
                }
            }
            else
            {
                foreach (var c in keystrokes)
                {
                    System.Threading.Thread.Sleep(50);
                    MsWindowsNativeMethods.SendMessage(new HandleRef(null, this.handle), MsWindowsNativeMethods.WM_CHAR, new IntPtr(Convert.ToInt64(c)), IntPtr.Zero);
                }
            }
        }

        /// <inheritdoc />
        public override bool IsDialogWindowFor(Window ownerWindow)
        {
            bool windowIsDialogFor = false;
            windowIsDialogFor = (ownerWindow.Handle == this.OwnerHandle || ownerWindow.Handle == this.ParentHandle) && this.Visible;
            return windowIsDialogFor;
        }

        /// <inheritdoc />
        public override System.Drawing.Image CaptureImage()
        {
            return MsWindowsNativeMethods.GetWindowImage(this.Handle);
        }

        /// <summary>
        /// Registers a windows message with the system.
        /// </summary>
        /// <param name="message">The message to register.</param>
        /// <returns>An integer value representing the message code.</returns>
        internal int RegisterWindowMessage(string message)
        {
            return MsWindowsNativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
        }

        /// <summary>
        /// Sends a message to the specified window.
        /// </summary>
        /// <param name="msg">The message code to send.</param>
        /// <param name="shortParam">The short parameter of the message.</param>
        /// <param name="longParam">The long parameter of the message.</param>
        /// <param name="timeout">The timeout in milliseconds for the message.</param>
        /// <param name="lpdwResult">The result of the message.</param>
        /// <returns>The return value of the SendMessage call.</returns>
        internal int SendMessageWithAbortIfHungTimeout(int msg, int shortParam, int longParam, int timeout, ref int lpdwResult)
        {
            return this.SendMessageTimeout(msg, shortParam, longParam, MsWindowsNativeMethods.SMTO_ABORTIFHUNG, timeout, ref lpdwResult);
        }

        private int SendMessageTimeout(int msg, int shortParam, int longParam, int flags, int timeout, ref int lpdwResult)
        {
            return MsWindowsNativeMethods.SendMessageTimeout(this.handle, msg, shortParam, longParam, flags, timeout, ref lpdwResult);
        }
    }
}
