// <copyright file="LogonDialog.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler.Dialogs
{
    /// <summary>
    /// Represents a system logon dialog.
    /// </summary>
    [Handleable(NativeDialogConstants.LogonDialog)]
    public class LogonDialog : Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogonDialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal LogonDialog(INativeDialog nativeDialog)
            : base(nativeDialog)
        {
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get { return NativeDialog.GetProperty(NativeDialogConstants.UserNameProperty).ToString(); }
            set { NativeDialog.PerformAction(NativeDialogConstants.SetUserNameAction, new object[] { value }); }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return NativeDialog.GetProperty(NativeDialogConstants.PasswordProperty).ToString(); }
            set { NativeDialog.PerformAction(NativeDialogConstants.SetPasswordAction, new object[] { value }); }
        }

        /// <summary>
        /// Clicks the OK button on the dialog.
        /// </summary>
        public void ClickOkButton()
        {
            NativeDialog.PerformAction(NativeDialogConstants.ClickOkAction, null);
        }

        /// <summary>
        /// Clicks the Cancel button on the dialog.
        /// </summary>
        public void ClickCancelButton()
        {
            NativeDialog.PerformAction(NativeDialogConstants.ClickCancelAction, null);
        }

        /// <summary>
        /// Sets the user name on the dialog.
        /// </summary>
        /// <param name="userName">The user name to set.</param>
        public void SetUserName(string userName)
        {
            NativeDialog.PerformAction(NativeDialogConstants.SetUserNameAction, new object[] { userName });
        }

        /// <summary>
        /// Sets the password on the dialog.
        /// </summary>
        /// <param name="password">The password to set.</param>
        public void SetPassword(string password)
        {
            NativeDialog.PerformAction(NativeDialogConstants.SetPasswordAction, new object[] { password });
        }
    }
}
