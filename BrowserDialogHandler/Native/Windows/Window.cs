// <copyright file="Window.cs" company="BrowserDialogHandler Project">
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
using System.Linq;
using System.Text;

namespace BrowserDialogHandler.Native.Windows
{
    /// <summary>
    /// Wraps a native operating system window.
    /// </summary>
    public abstract class Window : IDisposable
    {
        private WindowEnumerationMethod enumerationMethod = WindowEnumerationMethod.WindowManagementApi;
        private WindowEnumerationMethod childEnumerationMethod = WindowEnumerationMethod.WindowManagementApi;
        private bool isDisposed = false;

        /// <summary>
        /// Gets or sets the enumeration method used to find this window.
        /// </summary>
        public virtual WindowEnumerationMethod EnumerationMethod
        {
            get { return this.enumerationMethod; }
            protected set { this.enumerationMethod = value; }
        }

        /// <summary>
        /// Gets or sets the enumeration method used to find child windows of this window.
        /// </summary>
        public virtual WindowEnumerationMethod ChildEnumerationMethod
        {
            get { return this.childEnumerationMethod; }
            set { this.childEnumerationMethod = value; }
        }

        /// <summary>
        /// Gets the operating system handle for this window.
        /// </summary>
        public abstract IntPtr Handle { get; }

        /// <summary>
        /// Gets the operating system handle for this window's parent window.
        /// </summary>
        public abstract IntPtr ParentHandle { get; }

        /// <summary>
        /// Gets the operating system handle for this window's owner window.
        /// </summary>
        public abstract IntPtr OwnerHandle { get; }

        /// <summary>
        /// Gets the class name of this window.
        /// </summary>
        public abstract string ClassName { get; }

        /// <summary>
        /// Gets the text of this window.
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// Gets a value indicating whether this window exists.
        /// </summary>
        public abstract bool Exists { get; }

        /// <summary>
        /// Gets a value indicating whether this window is visible.
        /// </summary>
        public abstract bool Visible { get; }

        /// <summary>
        /// Gets a value indicating whether this window is enabled.
        /// </summary>
        public abstract bool Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether this window is a top-level window.
        /// </summary>
        public abstract bool IsTopLevelWindow { get; }

        /// <summary>
        /// Gets a value indicating whether this window is a dialog.
        /// </summary>
        public abstract bool IsDialog { get; }

        /// <summary>
        /// Gets a value indicating whether this window is able to be pressed or clicked, as defined by the operating system.
        /// </summary>
        public abstract bool IsPressable { get; }

        /// <summary>
        /// Gets or sets the style of how the window is shown.
        /// </summary>
        public abstract WindowShowStyle WindowStyle { get; set; }

        /// <summary>
        /// Gets the style of this window.
        /// </summary>
        public abstract long Style { get; }

        /// <summary>
        /// Gets a text description of the style of this window.
        /// </summary>
        public abstract string StyleDescriptor { get; }

        /// <summary>
        /// Gets the operating system ID of the process that owns this window.
        /// </summary>
        public abstract int ProcessId { get; }

        /// <summary>
        /// Gets the child item ID associated with this window.
        /// </summary>
        public abstract int ItemId { get; }

        /// <summary>
        /// Gets the <see cref="AssistiveTechnologyObject"/> for this window.
        /// </summary>
        internal abstract AssistiveTechnologyObject AccessibleObject { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this object has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get { return this.isDisposed; }
            set { this.isDisposed = value; }
        }

        /// <summary>
        /// Sets the focus to this window.
        /// </summary>
        /// <returns><see langword="true"/> if the focus was successfully set, otherwise <see langword="false"/>.</returns>
        public abstract bool SetFocus();

        /// <summary>
        /// Gets a value indicating whether this window is a dialog for the specified window.
        /// </summary>
        /// <param name="ownerWindow">The <see cref="Window"/> to test to see if this window is a dialog for.</param>
        /// <returns><see langword="true"/> if this window is a dialog for the specified window, otherwise <see langword="false"/>.</returns>
        public abstract bool IsDialogWindowFor(Window ownerWindow);

        /// <summary>
        /// Gets a list of the child windows for this window.
        /// </summary>
        /// <param name="constraint">A <see cref="WindowsCriteriaConstraint"/> that returned windows should match.</param>
        /// <returns>A list of <see cref="Window"/> objects matching the criteria.</returns>
        public abstract IList<Window> GetChildWindows(WindowCriteriaConstraint constraint);

        /// <summary>
        /// Presses, or clicks on, this window.
        /// </summary>
        public abstract void Press();

        /// <summary>
        /// Sends a series of keystrokes to this window.
        /// </summary>
        /// <param name="keystrokes">The keystrokes to send to this window.</param>
        public abstract void SendKeystrokes(string keystrokes);

        /// <summary>
        /// Forces this window to close.
        /// </summary>
        public abstract void ForceClose();

        /// <summary>
        /// Captures a screenshot of this window.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Image"/> containing the screenshot of this window.</returns>
        public abstract System.Drawing.Image CaptureImage();
        
        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
