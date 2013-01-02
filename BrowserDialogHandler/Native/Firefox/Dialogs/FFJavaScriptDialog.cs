// <copyright file="FFJavaScriptDialog.cs" company="BrowserDialogHandler Project">
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
using System.Linq;
using System.Text;
using BrowserDialogHandler.Native.Windows;
using BrowserDialogHandler.UtilityClasses;

namespace BrowserDialogHandler.Native.Mozilla.Dialogs
{
    /// <summary>
    /// Represents the Firefox-specific version of a JavaScript dialog.
    /// </summary>
    internal class FFJavaScriptDialog : NativeDialog
    {
        private readonly int okButtonId = 0;
        private readonly int cancelButtonId = 0;
        private readonly string messageLabelClass = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FFJavaScriptDialog"/> class.
        /// </summary>
        public FFJavaScriptDialog()
        {
            this.Kind = NativeDialogConstants.JavaScriptAlertDialog; 
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                this.okButtonId = 1;
                this.cancelButtonId = 2;
                this.messageLabelClass = WindowFactory.GetWindowClassForRole(AccessibleRole.Label, false);
            }
            else
            {
                this.okButtonId = 10;
                this.cancelButtonId = 11;
                this.messageLabelClass = WindowFactory.GetWindowClassForRole(AccessibleRole.Text, false);
            }
        }

        /// <inheritdoc />
        public override object GetProperty(string propertyId)
        {
            object propertyValue = null;
            if (propertyId == NativeDialogConstants.TitleProperty)
            {
                    propertyValue = this.DialogWindow.Text;
            }
            else if (propertyId == NativeDialogConstants.MessageProperty)
            {
                string className = WindowFactory.GetWindowClassForRole(AccessibleRole.Text, false);
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    className = WindowFactory.GetWindowClassForRole(AccessibleRole.Label, false);
                }

                IList<Window> staticLabel = this.DialogWindow.GetChildWindows(w => w.ClassName == className);
                propertyValue = staticLabel[0].Text;
                WindowFactory.DisposeWindows(staticLabel);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid property name '{0}'", propertyId), "propertyId");
            }

            return propertyValue;
        }

        /// <inheritdoc />
        public override void PerformAction(string actionId, object[] args)
        {
            if (actionId == NativeDialogConstants.ClickCancelAction || actionId == NativeDialogConstants.ClickOkAction)
            {
                int buttonId = this.okButtonId;
                if (actionId == NativeDialogConstants.ClickCancelAction)
                {
                    buttonId = this.cancelButtonId;
                }

                IList<Window> buttons = this.DialogWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, false) && b.ItemId == buttonId);
                buttons[0].Press();
                WindowFactory.DisposeWindows(buttons);
                this.WaitForWindowToDisappear();
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid action name '{0}'", actionId), "propertyId");
            }
        }

        /// <inheritdoc />
        public override bool WindowIsDialogInstance(Window candidateWindow)
        {
            bool windowIsDialog = false;
            IList<Window> buttons = candidateWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, false));
            IList<Window> staticLabel = candidateWindow.GetChildWindows(w => w.ClassName == this.messageLabelClass);
            if (buttons.Count == 1 && buttons[0].ItemId == this.okButtonId && staticLabel.Count == 1)
            {
                this.Kind = NativeDialogConstants.JavaScriptAlertDialog;
                windowIsDialog = true;
            }
            else if (buttons.Count == 2 && buttons[0].ItemId == this.okButtonId && buttons[1].ItemId == this.cancelButtonId && staticLabel.Count == 1)
            {
                this.Kind = NativeDialogConstants.JavaScriptConfirmDialog;
                windowIsDialog = true;
            }

            WindowFactory.DisposeWindows(buttons);
            WindowFactory.DisposeWindows(staticLabel);
            return windowIsDialog;
        }
    }
}
