// <copyright file="Expectation.cs" company="BrowserDialogHandler Project">
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using BrowserDialogHandler.Interfaces;
using BrowserDialogHandler.UtilityClasses;

namespace BrowserDialogHandler
{
    /// <summary>
    /// Represents an expectation that an object should fulfill.
    /// </summary>
    public abstract class Expectation : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the expectation is satisfied.
        /// </summary>
        public abstract bool IsSatisfied { get; }

        /// <summary>
        /// Gets a value indicating whether the timeout for the expectation was reached.
        /// </summary>
        public abstract bool TimeoutReached { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Sets the object against which the expectation is tested.
        /// </summary>
        /// <param name="objectToSet">The object against which to test the expectation.</param>
        internal abstract void SetObject(object objectToSet);

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
