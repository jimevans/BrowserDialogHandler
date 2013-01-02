// <copyright file="VBScriptMsgBoxDialog.cs" company="BrowserDialogHandler Project">
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
using System.Globalization;
using System.Linq;
using System.Text;
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler.Dialogs
{
    /// <summary>
    /// Represents a dialog displayed by VBScript.
    /// </summary>
    public class VBScriptMsgBoxDialog : Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VBScriptMsgBoxDialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal VBScriptMsgBoxDialog(INativeDialog nativeDialog)
            : base(nativeDialog)
        {
        }

        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        public string Title
        {
            get { return this.NativeDialog.GetProperty(NativeDialogConstants.TitleProperty).ToString(); }
        }

        /// <summary>
        /// Gets the message displayed by the dialog.
        /// </summary>
        public string Message
        {
            get { return this.NativeDialog.GetProperty(NativeDialogConstants.MessageProperty).ToString(); }
        }

        /// <summary>
        /// Clicks the specified button on the dialog.
        /// </summary>
        /// <param name="buttonActionId">The ID of the button to click.</param>
        /// <param name="buttonDescription">The description of the button to click.</param>
        protected void ClickButton(string buttonActionId, string buttonDescription)
        {
            Logger.LogAction(string.Format(CultureInfo.InvariantCulture, "Clicking {0} button on {1} dialog", buttonDescription, NativeDialog.Kind));
            this.NativeDialog.PerformAction(buttonActionId, null);
        }
    }
}
