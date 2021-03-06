﻿// <copyright file="VBScriptRetryCancelDialog.cs" company="BrowserDialogHandler Project">
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
    /// Represents a VBScript dialog with Retry and Cancel buttons.
    /// </summary>
    [Handleable(NativeDialogConstants.VBScriptRetryCancelDialog)]
    public class VBScriptRetryCancelDialog : VBScriptMsgBoxDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VBScriptRetryCancelDialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal VBScriptRetryCancelDialog(INativeDialog nativeDialog)
            : base(nativeDialog)
        {
        }

        /// <summary>
        /// Clicks the Retry button.
        /// </summary>
        public void ClickRetryButton()
        {
            this.ClickButton(NativeDialogConstants.ClickRetryAction, "Retry");
        }

        /// <summary>
        /// Clicks the Cancel button.
        /// </summary>
        public void ClickCancelButton()
        {
            this.ClickButton(NativeDialogConstants.ClickCancelAction, "Cancel");
        }
    }
}
