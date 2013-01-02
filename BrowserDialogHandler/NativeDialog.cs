// <copyright file="NativeDialog.cs" company="BrowserDialogHandler Project">
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
using System.Text;
using BrowserDialogHandler.Native.Windows;
using BrowserDialogHandler.UtilityClasses;

namespace BrowserDialogHandler.Native
{
    /// <summary>
    /// Implementation of a native dialog, with behavior unique to a specific operating system or browser.
    /// </summary>
    internal abstract class NativeDialog : INativeDialog
    {
        private const string SendKeysCharactersToBeEscaped = "~%^+{}[]()";

        private string kind = string.Empty;
        private Window dialogWindow = null;
        private bool isDisposed = false;

        /// <inheritdoc />
        public event EventHandler DialogDismissed;

        /// <inheritdoc />
        public Window DialogWindow
        {
            get { return this.dialogWindow; }
            set { this.dialogWindow = value; }
        }

        /// <inheritdoc />
        public string Kind
        {
            get { return this.kind; }
            protected set { this.kind = value; }
        }

        /// <inheritdoc />
        public abstract object GetProperty(string propertyId);

        /// <inheritdoc />
        public abstract void PerformAction(string actionId, object[] args);

        /// <inheritdoc />
        public abstract bool WindowIsDialogInstance(Window candidateWindow);

        /// <inheritdoc />
        public virtual void Dismiss()
        {
            this.dialogWindow.ForceClose();
            this.WaitForWindowToDisappear();
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.dialogWindow != null)
                    {
                        this.dialogWindow.Dispose();
                    }
                }

                this.dialogWindow = null;
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Fires the <see cref="DialogDismissed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> object containing information about the event.</param>
        protected void OnDialogDismissed(EventArgs e)
        {
            if (this.DialogDismissed != null)
            {
                this.DialogDismissed(this, e);
            }
        }

        /// <summary>
        /// Waits for the window of this dialog to disappear.
        /// </summary>
        protected void WaitForWindowToDisappear()
        {
            if (TryUntilTimeoutExecutor.Try<bool>(TimeSpan.FromSeconds(10), () => { return !dialogWindow.Exists; }))
            {
                this.OnDialogDismissed(new EventArgs());
            }
        }

        /// <summary>
        /// Escapes special characters to be sent to dialogs.
        /// </summary>
        /// <param name="value">The characters to be sent.</param>
        /// <returns>The string of the characters with special characters escaped.</returns>
        protected string EscapeSendKeysCharacters(string value)
        {
            if (value.IndexOfAny(SendKeysCharactersToBeEscaped.ToCharArray()) > -1)
            {
                string returnvalue = null;

                foreach (var c in value)
                {
                    if (SendKeysCharactersToBeEscaped.IndexOf(c) != -1)
                    {
                        // Escape sendkeys special characters
                        returnvalue = returnvalue + "{" + c + "}";
                    }
                    else
                    {
                        returnvalue = returnvalue + c;
                    }
                }

                return returnvalue;
            }

            return value;
        }
    }
}
