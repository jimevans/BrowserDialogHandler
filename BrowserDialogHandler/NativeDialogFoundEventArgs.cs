// <copyright file="NativeDialogFoundEventArgs.cs" company="BrowserDialogHandler Project">
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

namespace BrowserDialogHandler.Native
{
    /// <summary>
    /// The <see cref="EventArgs"/> object returned when a dialog is found.
    /// </summary>
    public class NativeDialogFoundEventArgs : EventArgs
    {
        private INativeDialog nativeDialog = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeDialogFoundEventArgs"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> which was found.</param>
        internal NativeDialogFoundEventArgs(INativeDialog nativeDialog)
        {
            this.nativeDialog = nativeDialog;
        }

        /// <summary>
        /// Gets the <see cref="INativeDialog"/> that was found.
        /// </summary>
        internal INativeDialog NativeDialog
        {
            get { return this.nativeDialog; }
        }
    }
}
