// <copyright file="DialogWatcher.cs" company="BrowserDialogHandler Project">
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using BrowserDialogHandler.Dialogs;
using BrowserDialogHandler.Exceptions;
using BrowserDialogHandler.Interfaces;
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler
{
    /// <summary>
    /// Represents the implementation of a dialog watcher.
    /// </summary>
    internal class DialogWatcher : IWatcher
    {
        #region Private members
        private Dictionary<Type, WatchableObjectHandler> handlers;
        private Dictionary<Type, Expectation> pendingExpectations;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWatcher"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="INativeDialogManager"/> implementation of a manager for dialogs.</param>
        /// <param name="watchableType">The <see cref="Type"/> of the object to watch for.</param>
        internal DialogWatcher(INativeDialogManager manager, Type watchableType)
        {
            manager.DialogFound += new EventHandler<NativeDialogFoundEventArgs>(this.DialogFoundHandler);
        }

        #region IWatcher Members
        /// <inheritdoc/>
        public int TotalHandlerCount
        {
            get
            {
                int count = 0;
                if (this.handlers != null)
                {
                    count = this.handlers.Count;
                }

                return count;
            }
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<Type> ActivelyHandledTypes
        {
            get
            {
                List<Type> handledTypeList = new List<Type>();
                if (this.handlers != null)
                {
                    foreach (Type handledType in this.handlers.Keys)
                    {
                        if (this.handlers[handledType].Enabled)
                        {
                            handledTypeList.Add(handledType);
                        }
                    }
                }

                return handledTypeList.AsReadOnly();
            }
        }

        /// <inheritdoc/>
        public void SetHandler<TWatchable>(Action<TWatchable> action) where TWatchable : IWatchable
        {
            if (this.handlers == null)
            {
                this.handlers = new Dictionary<Type, WatchableObjectHandler>();
            }

            WatchableObjectHandler handler = this.GetExistingHandler(typeof(TWatchable));
            if (handler != null)
            {
                this.ClearHandler<TWatchable>();
            }

            WatchableObjectHandler<TWatchable> newHandler = new WatchableObjectHandler<TWatchable>(action);

            // If there exists an expectation, disable the handler.
            Expectation existingExpectation = this.GetExistingExpectation(typeof(TWatchable));
            if (existingExpectation != null)
            {
                newHandler.Enabled = false;
            }

            this.handlers.Add(typeof(TWatchable), newHandler);
            Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Handler set for watchable type {0}.", typeof(TWatchable).Name));
        }

        /// <inheritdoc/>
        public void ClearHandler<TWatchable>() where TWatchable : IWatchable
        {
            WatchableObjectHandler existingHandler = this.GetExistingHandler(typeof(TWatchable));
            if (existingHandler != null)
            {
                this.handlers.Remove(typeof(TWatchable));
                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Handler cleared for watchable type {0}.", typeof(TWatchable).Name));
            }
        }

        /// <inheritdoc/>
        public void ResetHandler<TWatchable>() where TWatchable : IWatchable
        {
            // Remove any expectations for the TWatchable type, if any exist.
            Expectation existingExpectation = this.GetExistingExpectation(typeof(TWatchable));
            if (existingExpectation != null)
            {
                this.pendingExpectations.Remove(typeof(TWatchable));
            }

            WatchableObjectHandler existingHandler = this.GetExistingHandler(typeof(TWatchable));
            if (existingHandler != null)
            {
                existingHandler.Reset();
                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Handler reset for watchable type {0}.", typeof(TWatchable).Name));
            }
        }

        /// <inheritdoc/>
        public WatchableObjectHandler<TWatchable> GetHandler<TWatchable>() where TWatchable : IWatchable
        {
            WatchableObjectHandler<TWatchable> handler = this.GetExistingHandler(typeof(TWatchable)) as WatchableObjectHandler<TWatchable>;
            return handler;
        }

        /// <inheritdoc/>
        public Expectation<TWatchable> Expect<TWatchable>() where TWatchable : IWatchable
        {
            return this.Expect<TWatchable>(Expectation<TWatchable>.DefaultTimeout, null);
        }

        /// <inheritdoc/>
        public Expectation<TWatchable> Expect<TWatchable>(TimeSpan timeout) where TWatchable : IWatchable
        {
            return this.Expect<TWatchable>(timeout, null);
        }

        /// <inheritdoc/>
        public Expectation<TWatchable> Expect<TWatchable>(Predicate<TWatchable> predicate) where TWatchable : IWatchable
        {
            return this.Expect<TWatchable>(Expectation<TWatchable>.DefaultTimeout, predicate);
        }

        /// <inheritdoc/>
        public Expectation<TWatchable> Expect<TWatchable>(TimeSpan timeout, Predicate<TWatchable> predicate) where TWatchable : IWatchable
        {
            if (this.pendingExpectations == null)
            {
                this.pendingExpectations = new Dictionary<Type, Expectation>();
            }

            // Expectations, by definition, take precedence over handlers. If we set an
            // expectation for a given dialog type, we must disable the handler for the
            // dialog type, if any has been defined. At present, it is up to the user to
            // re-enable the dialog handler by calling ResetHandler().
            Expectation<TWatchable> expectation = this.GetExistingExpectation(typeof(TWatchable)) as Expectation<TWatchable>;
            if (expectation == null)
            {
                expectation = new Expectation<TWatchable>(timeout, predicate);
                this.pendingExpectations.Add(typeof(TWatchable), expectation);
                WatchableObjectHandler existingHandler = this.GetExistingHandler(typeof(TWatchable));
                if (existingHandler != null)
                {
                    // Logger.LogInfo("Handler for watchable type {0} disabled and Expectation set", typeof(TWatchable).Name);
                    Console.WriteLine("Handler for watchable type {0} disabled and Expectation set", typeof(TWatchable).Name);
                    existingHandler.Enabled = false;
                }
            }

            return expectation;
        }

        /// <inheritdoc/>
        public bool IsExpecting<TWatchable>() where TWatchable : IWatchable
        {
            return this.GetExistingExpectation(typeof(TWatchable)) != null;
        }
        #endregion

        private void DialogFoundHandler(object sender, NativeDialogFoundEventArgs e)
        {
            Dialog dialogInstance = DialogFactory.CreateDialog(e.NativeDialog);
            if (dialogInstance == null)
            {
                throw new DialogHandlerException(string.Format("Could not find Dialog instance for {0}", e.NativeDialog.Kind));
            }

            this.HandleDialogOrFulfillExpectation(dialogInstance);
        }

        private void HandleDialogOrFulfillExpectation(Dialog watchableObject)
        {
            Type dialogType = watchableObject.GetType();
            Expectation existingExpectation = this.GetExistingExpectation(dialogType);
            WatchableObjectHandler existingHandler = this.GetExistingHandler(dialogType);
            if (existingExpectation != null && !existingExpectation.TimeoutReached)
            {
                // Once the expectation is met, we can remove it from the list of
                // pending expectations.
                Logger.LogAction(string.Format(CultureInfo.InvariantCulture, "{0} dialog found meeting expectation", watchableObject.NativeDialog.Kind));
                existingExpectation.SetObject(watchableObject);
                this.pendingExpectations.Remove(dialogType);
            }
            else if (existingHandler != null && existingHandler.Enabled)
            {
                // Handle the dialog with the handler. N.B. WatchableObjectHandler will
                // dispose of the IWatchable object by default. It also will catch any
                // exceptions from poorly written handler code.
                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Handling {0} dialog with handler", watchableObject.NativeDialog.Kind));
                existingHandler.HandleObject(watchableObject);
                if (existingHandler.HandleOnce)
                {
                    this.handlers.Remove(dialogType);
                }
            }
        }

        private WatchableObjectHandler GetExistingHandler(Type dialogType)
        {
            WatchableObjectHandler existingHandler = null;
            if (this.handlers != null && this.handlers.ContainsKey(dialogType))
            {
                existingHandler = this.handlers[dialogType];
            }

            return existingHandler;
        }

        private Expectation GetExistingExpectation(Type dialogType)
        {
            Expectation existingExpectation = null;
            if (this.pendingExpectations != null && this.pendingExpectations.ContainsKey(dialogType))
            {
                existingExpectation = this.pendingExpectations[dialogType];
            }

            return existingExpectation;
        }
    }
}
