// <copyright file="FFRestoreSessionDialog.cs" company="BrowserDialogHandler Project">
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

namespace BrowserDialogHandler.Native.Mozilla.Dialogs
{
    /// <summary>
    /// Represents the Firefox-specific version of a "restore previous browsing session" dialog.
    /// </summary>
    internal class FFRestoreSessionDialog : NativeDialog
    {
        private readonly int restoreSessionButtonId = 0;
        private readonly int newSessionButtonId = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FFRestoreSessionDialog"/> class.
        /// </summary>
        public FFRestoreSessionDialog()
        {
            this.Kind = NativeDialogConstants.FireFoxRestoreSessionDialog;
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                this.restoreSessionButtonId = 2;
                this.newSessionButtonId = 1;
            }
            else
            {
                this.restoreSessionButtonId = 10;
                this.newSessionButtonId = 11;
            }
        }

        /// <inheritdoc />
        public override object GetProperty(string propertyId)
        {
            // No properties worth getting in this dialog.
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void PerformAction(string actionId, object[] args)
        {
            if (actionId == NativeDialogConstants.ClickRestoreSessionAction || actionId == NativeDialogConstants.ClickStartNewSessionAction)
            {
                int buttonId = this.restoreSessionButtonId;
                if (actionId == NativeDialogConstants.ClickStartNewSessionAction)
                {
                    buttonId = this.newSessionButtonId;
                }
                
                IList<Window> buttons = this.DialogWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, false) && b.ItemId == buttonId);
                buttons[0].Press();
                WindowFactory.DisposeWindows(buttons);
                this.WaitForWindowToDisappear();
            }
        }

        /// <inheritdoc />
        public override bool WindowIsDialogInstance(Window candidateWindow)
        {
            bool windowIsDialog = false;
            IList<Window> buttons = candidateWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, false));
            if (buttons.Count == 2 && candidateWindow.Text.StartsWith("Firefox - "))
            {
                windowIsDialog = true;
            }

            WindowFactory.DisposeWindows(buttons);
            return windowIsDialog;
        }
    }
}
