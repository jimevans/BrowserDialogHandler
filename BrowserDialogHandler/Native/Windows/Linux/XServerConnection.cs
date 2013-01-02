// <copyright file="XServerConnection.cs" company="BrowserDialogHandler Project">
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

namespace BrowserDialogHandler.Native.Windows.Linux
{
    /// <summary>
    /// Wraps a connection to an X server.
    /// </summary>
    internal class XServerConnection : IDisposable
    {
        private IntPtr x11Display = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="XServerConnection"/> class.
        /// </summary>
        internal XServerConnection()
        {
            this.x11Display = X11WindowsNativeMethods.OpenServerConnection();
        }
        
        /// <summary>
        /// Gets the pointer to this display.
        /// </summary>
        internal IntPtr Display
        {
            get { return this.x11Display; }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            try
            {
                if (this.x11Display != IntPtr.Zero)
                {
                    X11WindowsNativeMethods.CloseServerConnection(this.x11Display);
                    this.x11Display = IntPtr.Zero;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
