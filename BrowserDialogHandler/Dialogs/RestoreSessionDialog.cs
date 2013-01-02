// <copyright file="RestoreSessionDialog.cs" company="BrowserDialogHandler Project">
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
using System.Text;
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler.Dialogs
{
    /// <summary>
    /// Represents the "Restore Session" dialog displayed by certain browsers.
    /// </summary>
    public class RestoreSessionDialog : Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreSessionDialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal RestoreSessionDialog(INativeDialog nativeDialog)
            : base(nativeDialog)
        {
        }

        /// <summary>
        /// Clicks the Restore Session button.
        /// </summary>
        public void ClickRestoreSessionButton()
        {
            Logger.LogAction(string.Format(CultureInfo.InvariantCulture, "Clicking Restore Session button on {0} dialog", NativeDialog.Kind));
            NativeDialog.PerformAction(NativeDialogConstants.ClickRestoreSessionAction, null);
        }

        /// <summary>
        /// Clicks the Start New Session button.
        /// </summary>
        public void ClickStartNewSessionButton()
        {
            Logger.LogAction(string.Format(CultureInfo.InvariantCulture, "Clicking Start New Session button on {0} dialog", NativeDialog.Kind));
            NativeDialog.PerformAction(NativeDialogConstants.ClickStartNewSessionAction, null);
        }
    }
}
