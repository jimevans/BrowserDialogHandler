// <copyright file="X11Window.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native.Windows.Linux
{
    /// <summary>
    /// Represents a window on an X11 windowing system (notably Linux).
    /// </summary>
    internal class X11Window : Window
    {
        private int processId = 0;
        private int itemId = 0;
        private IntPtr handle = IntPtr.Zero;
        private IntPtr parentHandle = IntPtr.Zero;
        private AtSpiObject accessibleObject = null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="X11Window"/> class.
        /// </summary>
        /// <param name="processId">Process ID of the window.</param>
        internal X11Window(int processId)
        {
            this.processId = processId;

            // XQueryTree returns windows in Z-order, with topmost windows at
            // the end. GetProcessTopLevelWindows uses XQueryTree to build the
            // list, so the topmost window should be the one at the end of the
            // list.
            IList<IntPtr> windowList = X11WindowsNativeMethods.FindTopLevelWindowsForProcess(processId);
            if (windowList.Count > 0)
            {
                this.handle = windowList[windowList.Count - 1];
            }

            this.ChildEnumerationMethod = WindowEnumerationMethod.AssistiveTechnologyApi;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Window"/> class.
        /// </summary>
        /// <param name="processId">Process ID of the window.</param>
        /// <param name="parentWindowHandle">Window handle for the parent window.</param>
        /// <param name="itemId">The item ID for this window.</param>
        /// <param name="accessibleObject">The accessible object corresponding to this window.</param>
        internal X11Window(int processId, IntPtr parentWindowHandle, int itemId, AtSpiObject accessibleObject)
        {
            this.processId = processId;
            this.parentHandle = parentWindowHandle;
            this.itemId = itemId;
            this.accessibleObject = accessibleObject;
            this.EnumerationMethod = WindowEnumerationMethod.AssistiveTechnologyApi;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Window"/> class.
        /// </summary>
        /// <param name="handle">The handle to this window.</param>
        internal X11Window(IntPtr handle)
        {
            this.handle = handle;
            this.processId = X11WindowsNativeMethods.GetProcessIdForWindow(this.handle);
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
            get { return this.parentHandle; }
        }

        /// <inheritdoc />
        public override IntPtr OwnerHandle
        {
            get { return this.ParentHandle; }
        }

        /// <inheritdoc />
        public override string ClassName
        {
            get
            {
                string windowClass = string.Empty;
                if (this.Exists)
                {
                    if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                    {
                        windowClass = X11WindowsNativeMethods.GetWindowClass(this.handle);
                    }
                    else
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
                if (this.Exists)
                {
                    if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                    {
                        windowText = X11WindowsNativeMethods.GetWindowText(this.handle);
                    }
                    else
                    {
                        AtSpiObject nativeAccObj = this.AccessibleObject as AtSpiObject;
                        if (nativeAccObj != null)
                        {
                            if (nativeAccObj.SupportsText)
                            {
                                windowText = nativeAccObj.Text;
                            }
                            else
                            {
                                windowText = nativeAccObj.Name;
                            }
                        }
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
                bool windowIsVisible = false;
                if (this.Exists)
                {
                    if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                    {
                        windowIsVisible = X11WindowsNativeMethods.IsWindowViewable(this.handle);
                    }
                    else
                    {
                        windowIsVisible = this.AccessibleObject.StateSet.Contains(AccessibleState.Visible);
                    }
                }

                return windowIsVisible;
            }
        }

        /// <inheritdoc />
        public override bool Enabled
        {
            get
            { 
                bool windowIsEnabled = false;
                if (this.Exists)
                {
                    if (this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi)
                    {
                        windowIsEnabled = true;
                    }
                    else
                    {
                        windowIsEnabled = this.AccessibleObject.StateSet.Contains(AccessibleState.Enabled);
                    }
                }

                return windowIsEnabled;
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
                    windowExists = X11WindowsNativeMethods.IsWindowValid(this.handle);
                }
                else
                {
                    AtSpiObject exisitingAccessible = this.AccessibleObject as AtSpiObject;
                    if (exisitingAccessible != null)
                    {
                        windowExists = exisitingAccessible.IsValid;
                    }
                }

                return windowExists;
            }
        }

        /// <inheritdoc />
        public override bool IsTopLevelWindow
        {
            get { return this.handle != IntPtr.Zero; }
        }

        /// <inheritdoc />
        public override bool IsDialog
        {
            get
            {
                bool windowIsDialog = false;
                if (this.Exists && this.IsTopLevelWindow)
                {
                    windowIsDialog = X11WindowsNativeMethods.IsDialogWindow(this.handle);
                }

                return windowIsDialog; 
            }
        }

        /// <inheritdoc />
        public override WindowEnumerationMethod ChildEnumerationMethod
        {
            get
            {
                return base.ChildEnumerationMethod;
            }

            set
            {
                base.ChildEnumerationMethod = WindowEnumerationMethod.AssistiveTechnologyApi;
            }
        }

        /// <inheritdoc />
        public override long Style
        {
            get { return 0L; }
        }

        /// <inheritdoc />
        public override string StyleDescriptor
        {
            get { return this.Style.ToString("X"); }
        }

        /// <inheritdoc />
        public override int ProcessId
        {
            get { return this.processId; }
        }

        /// <inheritdoc />
        public override WindowShowStyle WindowStyle
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public override int ItemId
        {
            get { return this.itemId; }
        }

        /// <inheritdoc />
        public override bool IsPressable
        {
            get
            {
                string pushButtonClass = WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, this.EnumerationMethod == WindowEnumerationMethod.WindowManagementApi);
                bool isClickableObject = false;
                isClickableObject = !this.IsTopLevelWindow && this.ClassName == pushButtonClass;
                return isClickableObject;
            }
        }

        /// <inheritdoc />
        internal override AssistiveTechnologyObject AccessibleObject
        {
            get
            {
                if (this.accessibleObject == null && this.handle != IntPtr.Zero)
                {
                    // The window returned may or may not include the window manager
                    // frame. Get the window frame extents to pass for window matching.
                    WindowManagerFrameExtents extents = X11WindowsNativeMethods.GetFrameExtents(this.handle);
                    this.accessibleObject = new AtSpiObject(this.processId, X11WindowsNativeMethods.GetWindowRectangle(this.handle), extents);
                }

                return this.accessibleObject;
            }
        }

        /// <summary>
        /// Gets a list of all of the top-level windows on the system.
        /// </summary>
        /// <returns>A list of <see cref="Window"/> objects representing the top-level windows on the system.</returns>
        public static IList<Window> GetAllTopLevelWindows()
        {
            List<Window> allWindowList = new List<Window>();
            IList<IntPtr> windowPointerList = X11WindowsNativeMethods.FindTopLevelWindowsForProcess(X11WindowsNativeMethods.AllProcesses);
            foreach (IntPtr windowPointer in windowPointerList)
            {
                allWindowList.Add(new X11Window(windowPointer));
            }

            return allWindowList;
        }

        /// <inheritdoc />
        public override void Press()
        {
            if (this.IsPressable && this.AccessibleObject != null && this.AccessibleObject.SupportsActions)
            {
                this.AccessibleObject.DoAction(0);
            }
        }

        /// <inheritdoc />
        public override bool SetFocus()
        {
            bool focusWasSet = false;
            if (this.Exists)
            {
                if (this.IsTopLevelWindow)
                {
                    X11WindowsNativeMethods.ActivateWindow(this.handle);
                    focusWasSet = true;
                }
                else
                {
                    AtSpiObject exisitingAccessible = this.AccessibleObject as AtSpiObject;
                    if (exisitingAccessible != null)
                    {
                        focusWasSet = exisitingAccessible.SetFocus();
                    }
                }
            }

            return focusWasSet;
        }

        /// <inheritdoc />
        public override IList<Window> GetChildWindows(WindowCriteriaConstraint constraint)
        {
            List<Window> childWindowList = new List<Window>();
            IList<AssistiveTechnologyObject> childObjectList = this.AccessibleObject.GetChildrenByRole(AccessibleRole.AnyRole, true, true);
            foreach (AssistiveTechnologyObject childObject in childObjectList)
            {
                int itemIndex = childObjectList.IndexOf(childObject);
                Window candidateWindow = this.WindowFromAccessibleObject(itemIndex, (AtSpiObject)childObject);
                if (constraint(candidateWindow))
                {
                    childWindowList.Add(candidateWindow);
                }
                else
                {
                    candidateWindow.Dispose();
                }
            }

            return childWindowList;
        }

        /// <inheritdoc />
        public override void ForceClose()
        {
            if (this.IsTopLevelWindow)
            {
                X11WindowsNativeMethods.CloseWindow(this.handle);
            }
            else
            {
                X11WindowsNativeMethods.CloseWindow(this.parentHandle);
            }
        }

        /// <inheritdoc />
        public override void SendKeystrokes(string keystrokes)
        {
            if (!this.IsTopLevelWindow)
            {
                this.accessibleObject.SetText(keystrokes);
            }
        }

        /// <inheritdoc />
        public override bool IsDialogWindowFor(Window ownerWindow)
        {
            bool windowIsDialogForCandidate = false;
            IntPtr owner = X11WindowsNativeMethods.GetTransientForWindow(this.handle);
            if (owner == ownerWindow.Handle)
            {
                windowIsDialogForCandidate = true;
            }

            return windowIsDialogForCandidate;
        }

        /// <inheritdoc />
        public override System.Drawing.Image CaptureImage()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (this.accessibleObject != null)
                {
                    this.accessibleObject.UnreferenceObject();
                    this.accessibleObject = null;
                }

                this.IsDisposed = true;
            }

            base.Dispose(disposing);
        }

        private X11Window WindowFromAccessibleObject(int id, AtSpiObject accessibleObject)
        {
            IntPtr parentWindowHandle = this.handle;
            if (!this.IsTopLevelWindow)
            {
                parentWindowHandle = this.parentHandle;
            }

            X11Window returnedWindow = new X11Window(this.processId, parentWindowHandle, id, accessibleObject);
            return returnedWindow;
        }
    }
}
