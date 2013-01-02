// <copyright file="FFLogonDialog.cs" company="BrowserDialogHandler Project">
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
    /// Represents the Firefox-specific version of a logon dialog.
    /// </summary>
    internal class FFLogonDialog : NativeDialog
    {
        private readonly int okButtonId = 0;
        private readonly int cancelButtonId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FFLogonDialog"/> class.
        /// </summary>
        public FFLogonDialog()
        {
            this.Kind = NativeDialogConstants.LogonDialog;
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                this.okButtonId = 1;
                this.cancelButtonId = 2;
            }
            else
            {
                this.okButtonId = 14;
                this.cancelButtonId = 15;
            }
        }

        /// <inheritdoc />
        public override object GetProperty(string propertyId)
        {
            object propertyValue = null;
            if (propertyId == NativeDialogConstants.UserNameProperty || propertyId == NativeDialogConstants.PasswordProperty)
            {
                string targetClassName = WindowFactory.GetWindowClassForRole(AccessibleRole.Text, false);
                if (propertyId == NativeDialogConstants.PasswordProperty)
                {
                    targetClassName = AccessibleRole.PasswordText.ToString();
                }

                IList<Window> windowList = this.DialogWindow.GetChildWindows(w => w.ClassName == targetClassName);
                if (windowList.Count > 0)
                {
                    propertyValue = windowList[0].Text;
                }

                WindowFactory.DisposeWindows(windowList);
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
                int buttonId = this.okButtonId;
                if (actionId == NativeDialogConstants.ClickCancelAction)
                {
                    buttonId = this.cancelButtonId;
                }

                IList<Window> buttons = this.DialogWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, false) && b.ItemId == buttonId);
                if (buttons.Count > 0)
                {
                    buttons[0].Press();
                }

                WindowFactory.DisposeWindows(buttons);
                this.WaitForWindowToDisappear();
            }
            else if (actionId == NativeDialogConstants.SetUserNameAction || actionId == NativeDialogConstants.SetPasswordAction)
            {
                string textValue = this.EscapeSendKeysCharacters(args[0].ToString());
                string targetClassName = WindowFactory.GetWindowClassForRole(AccessibleRole.Text, false);
                if (actionId == NativeDialogConstants.SetPasswordAction)
                {
                    targetClassName = AccessibleRole.PasswordText.ToString();
                }

                IList<Window> windowList = this.DialogWindow.GetChildWindows(w => w.ClassName == targetClassName && !w.AccessibleObject.StateSet.Contains(AccessibleState.SelectableText));
                if (windowList.Count > 0)
                {
                    windowList[0].SendKeystrokes(textValue);
                }

                WindowFactory.DisposeWindows(windowList);
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
            IList<Window> childWindowList = candidateWindow.GetChildWindows(w => w.ClassName == AccessibleRole.PasswordText.ToString());
            if (childWindowList.Count > 0)
            {
                windowIsDialog = true;
            }

            WindowFactory.DisposeWindows(childWindowList);
            return windowIsDialog;
        }
    }
}
