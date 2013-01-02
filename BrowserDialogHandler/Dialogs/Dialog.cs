// <copyright file="Dialog.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Interfaces;
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler.Dialogs
{
    /// <summary>
    /// Abstract class representing a dialog displayed by a browser.
    /// </summary>
    public abstract class Dialog : IWatchable
    {
        private INativeDialog nativeDialogImpl = null;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dialog"/> class.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> representing the dialog.</param>
        internal Dialog(INativeDialog nativeDialog)
        {
            this.nativeDialogImpl = nativeDialog;
        }

        /// <inheritdoc />
        public virtual bool Exists
        {
            get { return this.nativeDialogImpl != null && this.nativeDialogImpl.DialogWindow != null && this.nativeDialogImpl.DialogWindow.Exists; }
        }

        /// <summary>
        /// Gets the native dialog implementation of the dialog.
        /// </summary>
        internal INativeDialog NativeDialog
        {
            get { return this.nativeDialogImpl; }
        }

        /// <inheritdoc />
        public virtual void DoDefaultAction()
        {
            this.nativeDialogImpl.Dismiss();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc />
        protected void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.nativeDialogImpl != null)
                    {
                        this.nativeDialogImpl.Dispose();
                    }
                }

                this.nativeDialogImpl = null;
                this.isDisposed = true;
            }
        }
    }
}
