// <copyright file="WatchableObjectHandler.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Interfaces;

namespace BrowserDialogHandler
{
    /// <summary>
    /// An abstract base class representing a handler for watchable objects.
    /// </summary>
    public abstract class WatchableObjectHandler
    {
        private bool handleObjectOnce = false;
        private bool handlerEnabled = true;
        private int timesHandled = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchableObjectHandler"/> class.
        /// </summary>
        protected WatchableObjectHandler()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this handler should handle the
        /// specified object only once, or every time the object appears.
        /// </summary>
        internal bool HandleOnce
        {
            get { return this.handleObjectOnce; }
            set { this.handleObjectOnce = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this handler is enabled.
        /// </summary>
        internal bool Enabled
        {
            get { return this.handlerEnabled; }
            set { this.handlerEnabled = value; }
        }

        /// <summary>
        /// Gets or sets the number of times the specified object has been handled.
        /// </summary>
        internal int HandleCount
        {
            get { return this.timesHandled; }
            set { this.timesHandled = value; }
        }

        /// <summary>
        /// Handles the object.
        /// </summary>
        /// <param name="objectToHandle">The object to handle with this <see cref="WatchableObjectHandler"/>.</param>
        internal abstract void HandleObject(object objectToHandle);

        /// <summary>
        /// Resets the handler, enabling it and setting the handled count to 0.
        /// </summary>
        internal void Reset()
        {
            this.handlerEnabled = true;
            this.timesHandled = 0;
        }
    }
}
