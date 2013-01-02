// <copyright file="NativeDialogConstants.cs" company="BrowserDialogHandler Project">
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

namespace BrowserDialogHandler.Native
{
    /// <summary>
    /// Class containing identifying constants for dialog handlers.
    /// </summary>
    public static class NativeDialogConstants
    {
        /// <summary>
        /// Represents the title of a dialog.
        /// </summary>
        public static readonly string TitleProperty = "TitleProperty";

        /// <summary>
        /// Represents the text message displayed by a dialog.
        /// </summary>
        public static readonly string MessageProperty = "MessageProperty";

        /// <summary>
        /// Represents the file name displayed by a file selection dialog.
        /// </summary>
        public static readonly string FileNameProperty = "FileNameProperty";
        
        /// <summary>
        /// Represents the user name used in a logon dialog.
        /// </summary>
        public static readonly string UserNameProperty = "UserName";

        /// <summary>
        /// Represents the password used in a logon dialog.
        /// </summary>
        public static readonly string PasswordProperty = "Password";

        /// <summary>
        /// Represents the action of clicking the OK button on a dialog.
        /// </summary>
        public static readonly string ClickOkAction = "ClickOk";

        /// <summary>
        /// Represents the action of clicking the Cancel button on a dialog.
        /// </summary>
        public static readonly string ClickCancelAction = "ClickCancel";

        /// <summary>
        /// Represents the action of clicking the Yes button on a dialog.
        /// </summary>
        public static readonly string ClickYesAction = "ClickYes";

        /// <summary>
        /// Represents the action of clicking the No button on a dialog.
        /// </summary>
        public static readonly string ClickNoAction = "ClickNo";

        /// <summary>
        /// Represents the action of clicking the Abort button on a dialog.
        /// </summary>
        public static readonly string ClickAbortAction = "ClickAbort";

        /// <summary>
        /// Represents the action of clicking the Retry button on a dialog.
        /// </summary>
        public static readonly string ClickRetryAction = "ClickRetry";

        /// <summary>
        /// Represents the action of clicking the Ignore button on a dialog.
        /// </summary>
        public static readonly string ClickIgnoreAction = "ClickIgnore";

        /// <summary>
        /// Represents the action of clicking the Open button on a file selection dialog.
        /// </summary>
        public static readonly string ClickOpenAction = "ClickOpen";

        /// <summary>
        /// Represents the action of clicking the Save button on a file selection dialog.
        /// </summary>
        public static readonly string ClickSaveAction = "ClickSave";

        /// <summary>
        /// Represents the action of clicking the Restore button on the restore session dialog.
        /// </summary>
        public static readonly string ClickRestoreSessionAction = "ClickRestoreSession";

        /// <summary>
        /// Represents the action of clicking the Start New Session button on the restore session dialog.
        /// </summary>
        public static readonly string ClickStartNewSessionAction = "ClickStartNewSession";

        /// <summary>
        /// Represents the action of setting the file name on a file selection dialog.
        /// </summary>
        public static readonly string SetFileNameAction = "SetFileName";

        /// <summary>
        /// Represents the action of setting the user name on a logon dialog.
        /// </summary>
        public static readonly string SetUserNameAction = "SetUserName";

        /// <summary>
        /// Represents the action of setting the password on a logon dialog.
        /// </summary>
        public static readonly string SetPasswordAction = "SetPassword";

        /// <summary>
        /// Identifies a JavaScript alert() dialog.
        /// </summary>
        internal const string JavaScriptAlertDialog = "AlertDialog";

        /// <summary>
        /// Identifies a JavaScript confirm() dialog.
        /// </summary>
        internal const string JavaScriptConfirmDialog = "ConfirmDialog";

        /// <summary>
        /// Identifies the dialog displayed by Firefox when a session is unexpectedly exited.
        /// </summary>
        internal const string FireFoxRestoreSessionDialog = "FireFoxRestoreSessionDialog";

        /// <summary>
        /// Identifies a VBScript message box dialog containing only an OK button.
        /// </summary>
        internal const string VBScriptOKOnlyDialog = "VBScriptOKOnlyDialog";

        /// <summary>
        /// Identifies a VBScript message box dialog containing an OK and Cancel buttons.
        /// </summary>
        internal const string VBScriptOKCancelDialog = "VBScriptOKCancelDialog";

        /// <summary>
        /// Identifies a VBScript message box dialog containing Abort, Retry, and Ignore buttons.
        /// </summary>
        internal const string VBScriptAbortRetryIgnoreDialog = "VBScriptAbortRetryIgnoreDialog";

        /// <summary>
        /// Identifies a VBScript message box dialog containing Yes, No, and Cancel buttons.
        /// </summary>
        internal const string VBScriptYesNoCancelDialog = "VBScriptYesNoCancelDialog";

        /// <summary>
        /// Identifies a VBScript message box dialog containing Yes and No buttons.
        /// </summary>
        internal const string VBScriptYesNoDialog = "VBScriptYesNoDialog";

        /// <summary>
        /// Identifies a VBScript message box dialog containing Retry and Cancel buttons.
        /// </summary>
        internal const string VBScriptRetryCancelDialog = "VBScriptRetryCancelDialog";

        /// <summary>
        /// Identifies a system logon dialog.
        /// </summary>
        internal const string LogonDialog = "LogonDialog";
    }
}
