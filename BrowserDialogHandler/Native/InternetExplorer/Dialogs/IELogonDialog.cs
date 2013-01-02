// <copyright file="IELogonDialog.cs" company="BrowserDialogHandler Project">
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

namespace BrowserDialogHandler.Native.InternetExplorer.Dialogs
{
    /// <summary>
    /// Represents the IE-specific version of a logon dialog.
    /// </summary>
    internal class IELogonDialog : NativeDialog
    {
        private const string SysCredentialWindowClass = "SysCredential";
        private const int UserNameTextBoxId = 0x3EB;
        private const int PasswordTextBoxId = 0x3ED;

        /// <summary>
        /// Initializes a new instance of the <see cref="IELogonDialog"/> class.
        /// </summary>
        public IELogonDialog()
        {
            this.Kind = NativeDialogConstants.LogonDialog;
        }

        /// <inheritdoc />
        public override object GetProperty(string propertyId)
        {
            object propertyValue = null;
            if (propertyId == NativeDialogConstants.UserNameProperty || propertyId == NativeDialogConstants.PasswordProperty)
            {
                int textBoxId = UserNameTextBoxId;
                if (propertyId == NativeDialogConstants.PasswordProperty)
                {
                    textBoxId = PasswordTextBoxId;
                }

                using (Window sysCredentialsWindow = this.GetSysCredentialWindow(this.DialogWindow))
                {
                    if (sysCredentialsWindow != null)
                    {
                        IList<Window> textBoxList = sysCredentialsWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.Text, true) && w.ItemId == textBoxId);
                        if (textBoxList.Count > 0)
                        {
                            propertyValue = textBoxList[0].Text;
                        }

                        WindowFactory.DisposeWindows(textBoxList);
                    }
                }
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
                int buttonId = 1;
                if (actionId == NativeDialogConstants.ClickCancelAction)
                {
                    buttonId = 2;
                }

                IList<Window> buttons = this.DialogWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, true) && b.ItemId == buttonId);
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
                using (Window sysCredentialsWindow = this.GetSysCredentialWindow(this.DialogWindow))
                {
                    if (sysCredentialsWindow != null)
                    {
                        int textBoxId = UserNameTextBoxId;
                        if (actionId == NativeDialogConstants.SetPasswordAction)
                        {
                            textBoxId = PasswordTextBoxId;
                        }

                        IList<Window> textBoxes = sysCredentialsWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.Text, true) && b.ItemId == textBoxId);
                        if (textBoxes.Count > 0)
                        {
                            textBoxes[0].SetFocus();
                            textBoxes[0].SendKeystrokes(textValue);
                        }

                        WindowFactory.DisposeWindows(textBoxes);
                    }
                }
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
            Window sysCredentialWindow = this.GetSysCredentialWindow(candidateWindow);
            if (sysCredentialWindow != null)
            {
                windowIsDialog = true;
                sysCredentialWindow.Dispose();
            }

            return windowIsDialog;
        }

        private Window GetSysCredentialWindow(Window parentWindow)
        {
            Window sysCredentialWindow = null;
            IList<Window> credentialWindowList = parentWindow.GetChildWindows(w => w.ClassName == SysCredentialWindowClass);
            if (credentialWindowList.Count > 0)
            {
                sysCredentialWindow = credentialWindowList[0];
                credentialWindowList.Remove(sysCredentialWindow);
            }

            WindowFactory.DisposeWindows(credentialWindowList);
            return sysCredentialWindow;
        }
    }
}
