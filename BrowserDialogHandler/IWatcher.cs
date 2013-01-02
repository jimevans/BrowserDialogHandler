// <copyright file="IWatcher.cs" company="BrowserDialogHandler Project">
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
using System.Linq;
using System.Text;

namespace BrowserDialogHandler.Interfaces
{
    /// <summary>
    /// Defines an interface for an object which will watch for watchable objects such as dialogs, InfoBars, and so on.
    /// </summary>
    public interface IWatcher
    {
        /// <summary>
        /// Gets a value indicating the total number of handlers the watcher is tracking.
        /// </summary>
        int TotalHandlerCount { get; }

        /// <summary>
        /// Gets a list of types handled by the current watcher. This only returns handled types where the handler is enabled.
        /// </summary>
        ReadOnlyCollection<Type> ActivelyHandledTypes { get; }

        /// <summary>
        /// Sets a handler for a type of watchable object (e.g., dialog, InfoBar, etc.).
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <param name="action">An <see cref="System.Action&lt;T&gt;"/> delegate to handle the object.</param>
        void SetHandler<TWatchable>(Action<TWatchable> action) where TWatchable : IWatchable;

        /// <summary>
        /// Clears the handler for the given watchable object type (e.g., dialog, InfoBar, etc.).
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        void ClearHandler<TWatchable>() where TWatchable : IWatchable;

        /// <summary>
        /// Resets the handler for the given watchable object type (e.g., dialog, InfoBar, etc.).
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <remarks>ResetHandler will also remove any unmet Expectations for the given dialog type.</remarks>
        void ResetHandler<TWatchable>() where TWatchable : IWatchable;

        /// <summary>
        /// Gets the handler for a type of watchable object (e.g., dialog, InfoBar, etc.).
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <returns>The <see cref="WatchableObjectHandler&lt;TWatchable&gt;"/> object that handles the object type.</returns>
        WatchableObjectHandler<TWatchable> GetHandler<TWatchable>() where TWatchable : IWatchable;

        /// <summary>
        /// Sets an expectation for a watchable object (e.g., dialog, InfoBar, etc.) to appear.
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <returns>An <see cref="BrowserDialogHandler.Expectation&lt;TWatchable&gt;"/> object the user can use to manipulate the object.</returns>
        /// <remarks>Expecting a watchable object to appear will suspend processing of any registered handlers for
        /// that object type. To resume automatic handling, you must call ResetHandler for the object type.</remarks>
        Expectation<TWatchable> Expect<TWatchable>() where TWatchable : IWatchable;

        /// <summary>
        /// Sets an expectation for a watchable object (e.g., dialog, InfoBar, etc.) to appear.
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <param name="timeout">A <see cref="System.TimeSpan"/> structure representing the time within which the expectation should be filled.</param>
        /// <returns>An <see cref="BrowserDialogHandler.Expectation&lt;TWatchable&gt;"/> object the user can use to manipulate the object.</returns>
        /// <remarks>Expecting a watchable object to appear will suspend processing of any registered handlers for
        /// that object type. To resume automatic handling, you must call ResetHandler for the object type.</remarks>
        Expectation<TWatchable> Expect<TWatchable>(TimeSpan timeout) where TWatchable : IWatchable;

        /// <summary>
        /// Sets an expectation for a watchable object (e.g., dialog, InfoBar, etc.) to appear.
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <param name="predicate">A <see cref="System.Predicate&lt;T&gt;"/> defining the criteria for the expectation.</param>
        /// <returns>An <see cref="BrowserDialogHandler.Expectation&lt;TWatchable&gt;"/> object the user can use to manipulate the object.</returns>
        /// <remarks>Expecting a watchable object to appear will suspend processing of any registered handlers for
        /// that object type. To resume automatic handling, you must call ResetHandler for the object type.</remarks>
        Expectation<TWatchable> Expect<TWatchable>(Predicate<TWatchable> predicate) where TWatchable : IWatchable;

        /// <summary>
        /// Sets an expectation for a watchable object (e.g., dialog, InfoBar, etc.) to appear.
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <param name="timeout">A <see cref="System.TimeSpan"/> structure representing the time within which the expectation should be filled.</param>
        /// <param name="predicate">A <see cref="System.Predicate&lt;T&gt;"/> defining the criteria for the expectation.</param>
        /// <returns>An <see cref="BrowserDialogHandler.Expectation&lt;TWatchable&gt;"/> object the user can use to manipulate the object.</returns>
        /// <remarks>Expecting a watchable object to appear will suspend processing of any registered handlers for
        /// that object type. To resume automatic handling, you must call ResetHandler for the object type.</remarks>
        Expectation<TWatchable> Expect<TWatchable>(TimeSpan timeout, Predicate<TWatchable> predicate) where TWatchable : IWatchable;

        /// <summary>
        /// Gets a value indicating whether a watchable object is being expected.
        /// </summary>
        /// <typeparam name="TWatchable">An object implementing the <see cref="BrowserDialogHandler.Interfaces.IWatchable"/> interface.</typeparam>
        /// <returns>true if the watcher is expecting the watchable type; false otherwise.</returns>
        bool IsExpecting<TWatchable>() where TWatchable : IWatchable;
    }
}
