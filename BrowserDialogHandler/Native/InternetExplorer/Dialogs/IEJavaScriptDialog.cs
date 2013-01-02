// <copyright file="IEJavaScriptDialog.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Dialogs;
using BrowserDialogHandler.Native.Windows;
using BrowserDialogHandler.UtilityClasses;

namespace BrowserDialogHandler.Native.InternetExplorer.Dialogs
{
    /// <summary>
    /// Represents the IE-specific version of a JavaScript dialog.
    /// </summary>
    internal class IEJavaScriptDialog : NativeDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IEJavaScriptDialog"/> class.
        /// </summary>
        public IEJavaScriptDialog()
        {
            this.Kind = NativeDialogConstants.JavaScriptAlertDialog;
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
                IList<Window> staticLabel = this.DialogWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.Label, true) && w.ItemId == 0xFFFF);
                propertyValue = staticLabel[0].Text;
                WindowFactory.DisposeWindows(staticLabel);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid property name '{0}'", propertyId), "actionId");
            }

            return propertyValue;
        }

        /// <inheritdoc />
        public override void PerformAction(string actionId, object[] args)
        {
            if (actionId == NativeDialogConstants.ClickCancelAction || actionId == NativeDialogConstants.ClickOkAction)
            {
                int buttonId = 2;
                if (this.Kind != NativeDialogConstants.JavaScriptAlertDialog && actionId == NativeDialogConstants.ClickOkAction)
                {
                    buttonId = 1;
                }

                IList<Window> buttons = this.DialogWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, true) && b.ItemId == buttonId);
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
            if (!candidateWindow.Text.ToLower().Contains("vbscript"))
            {
                IList<Window> buttons = candidateWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, true));
                IList<Window> staticLabel = candidateWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.Label, true) && w.ItemId == 0xFFFF);
                if (buttons.Count == 1 && buttons[0].ItemId == 2 && staticLabel.Count == 1)
                {
                    this.Kind = NativeDialogConstants.JavaScriptAlertDialog;
                    windowIsDialog = true;
                }
                else if (buttons.Count == 2 && buttons[0].ItemId == 1 && buttons[1].ItemId == 2 && staticLabel.Count == 1)
                {
                    this.Kind = NativeDialogConstants.JavaScriptConfirmDialog;
                    windowIsDialog = true;
                }

                WindowFactory.DisposeWindows(buttons);
                WindowFactory.DisposeWindows(staticLabel);
            }

            return windowIsDialog;
        }
    }
}
