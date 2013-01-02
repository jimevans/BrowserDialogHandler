// <copyright file="FileLocationDialog.cs" company="BrowserDialogHandler Project">
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
    /// Represents a file selection dialog displayed by the OS.
    /// </summary>
    public class FileLocationDialog : Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLocationDialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal FileLocationDialog(INativeDialog nativeDialog)
            : base(nativeDialog)
        {
        }

        /// <summary>
        /// Gets or sets the file name in the dialog.
        /// </summary>
        public string FileName
        {
            get { return NativeDialog.GetProperty(NativeDialogConstants.FileNameProperty).ToString(); }
            set { NativeDialog.PerformAction(NativeDialogConstants.SetFileNameAction, new object[] { value }); }
        }

        /// <summary>
        /// Clicks the Open button on the dialog.
        /// </summary>
        public void ClickOpen()
        {
            NativeDialog.PerformAction(NativeDialogConstants.ClickOpenAction, null);
        }

        /// <summary>
        /// Clicks the Save button on the dialog.
        /// </summary>
        public void ClickSave()
        {
            NativeDialog.PerformAction(NativeDialogConstants.ClickSaveAction, null);
        }

        /// <summary>
        /// Sets the file name in the dialog.
        /// </summary>
        /// <param name="fileName">The file name to set.</param>
        public void SetFileName(string fileName)
        {
            NativeDialog.PerformAction(NativeDialogConstants.SetFileNameAction, new object[] { fileName });
        }
    }
}
