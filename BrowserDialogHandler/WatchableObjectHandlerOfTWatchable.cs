// <copyright file="WatchableObjectHandlerOfTWatchable.cs" company="BrowserDialogHandler Project">
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
    /// A handler for objects implementing the <see cref="IWatchable"/> interface.
    /// </summary>
    /// <typeparam name="TWatchable">An object implementing the <see cref="IWatchable"/> interface.</typeparam>
    public sealed class WatchableObjectHandler<TWatchable> : WatchableObjectHandler where TWatchable : IWatchable
    {
        private Action<TWatchable> handlerAction;
        private bool autoDisposeObject = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchableObjectHandler{TWatchable}"/> class.
        /// </summary>
        /// <param name="handlerAction">A method that takes a <see cref="TWatchable"/> object and handles it.</param>
        internal WatchableObjectHandler(Action<TWatchable> handlerAction)
            : this(handlerAction, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchableObjectHandler{TWatchable}"/> class.
        /// </summary>
        /// <param name="handlerAction">A method that takes a <see cref="TWatchable"/> object and handles it.</param>
        /// <param name="automaticallyDisposeWatchedObject"><see langword="true"/> to automatically dispose
        /// of the watched object when handled; otherwise, <see langword="false"/>.</param>
        internal WatchableObjectHandler(Action<TWatchable> handlerAction, bool automaticallyDisposeWatchedObject)
        {
            this.handlerAction = handlerAction;
            this.autoDisposeObject = automaticallyDisposeWatchedObject;
        }

        /// <summary>
        /// Gets the method used to handle the watched object.
        /// </summary>
        internal Action<TWatchable> HandlerAction
        {
            get { return this.handlerAction; }
        }

        /// <inheritdoc />
        internal override void HandleObject(object objectToHandle)
        {
            // Verify the object is an IWatchable before setting.
            IWatchable watchableObject = objectToHandle as IWatchable;
            if (watchableObject != null)
            {
                // Be sure to catch any exceptions in the user-defined handler code.
                TWatchable castObject = (TWatchable)objectToHandle;
                try
                {
                    this.HandleObjectInternal(castObject);
                    this.HandleCount++;
                }
                catch (Exception ex)
                {
                    Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Unexpected exception handling object: {0}", ex.Message));
                }
                finally
                {
                    if (this.autoDisposeObject)
                    {
                        castObject.Dispose();
                    }
                }
            }
        }

        private void HandleObjectInternal(TWatchable objectToHandle)
        {
            this.handlerAction(objectToHandle);
        }
    }
}
