// <copyright file="INativeDialog.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native
{
    /// <summary>
    /// Interface describing a native dialog, with behavior unique to a specific operating system or browser.
    /// </summary>
    internal interface INativeDialog : IDisposable
    {
        /// <summary>
        /// Event raised when the dialog is dismissed.
        /// </summary>
        event EventHandler DialogDismissed;
 
       /// <summary>
        /// Gets or sets the <see cref="BrowserDialogHandler.Native.Windows.Window"/> object representing the physical operating system window of the dialog.
        /// </summary>
        Window DialogWindow { get; set; }

        /// <summary>
        /// Gets a code that identifies the kind of dialog that this object represents, such as "AlertDialog", "ConfirmDialog",
        /// "IESecurityDialog", "FireFoxWidgetDialog".
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Gets a property of the dialog such as its title, message text, or other contents.
        /// </summary>
        /// <param name="propertyId">A <see cref="System.String"/> representing the property to get the value of.</param>
        /// <returns>A <see cref="System.Object"/> that contains the property value.</returns>
        object GetProperty(string propertyId);

        /// <summary>
        /// Performs some action such as clicking on a particular button or entering some text into a field.
        /// </summary>
        /// <param name="actionId">A <see cref="System.String"/> representing the action to execute.</param>
        /// <param name="args">An array of <see cref="System.Object"/> representing the parameters to the actions.</param>
        void PerformAction(string actionId, object[] args);

        /// <summary>
        /// Performs the default action to dismiss the dialog such as closing or canceling it.
        /// </summary>
        void Dismiss();

        /// <summary>
        /// Gets a value indicating whether the <see cref="BrowserDialogHandler.Native.Windows.Window"/> object is an instance of this dialog.
        /// </summary>
        /// <param name="candidateWindow">The <see cref="BrowserDialogHandler.Native.Windows.Window"/> object to test.</param>
        /// <returns>true if the <see cref="BrowserDialogHandler.Native.Windows.Window"/> is an instance of this dialog; false if it is not.</returns>
        bool WindowIsDialogInstance(Window candidateWindow);
    }
}
