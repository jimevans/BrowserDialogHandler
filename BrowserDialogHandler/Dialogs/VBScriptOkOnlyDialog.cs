// <copyright file="VBScriptOkOnlyDialog.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler.Dialogs
{
    /// <summary>
    /// Represents a VBScript dialog with only an OK button.
    /// </summary>
    [Handleable(NativeDialogConstants.VBScriptOKOnlyDialog)]
    public class VBScriptOkOnlyDialog : VBScriptMsgBoxDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VBScriptOkOnlyDialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal VBScriptOkOnlyDialog(INativeDialog nativeDialog)
            : base(nativeDialog)
        {
        }

        /// <summary>
        /// Clicks the OK button.
        /// </summary>
        public void ClickOkButton()
        {
            this.ClickButton(NativeDialogConstants.ClickOkAction, "OK");
        }
    }
}
