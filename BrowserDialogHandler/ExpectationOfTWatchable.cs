// <copyright file="ExpectationOfTWatchable.cs" company="BrowserDialogHandler Project">
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
    /// Represents an expectation on an object implementing <see cref="IWatchable"/>
    /// </summary>
    /// <typeparam name="TWatchable">An object implementing <see cref="IWatchable"/>.</typeparam>
    public class Expectation<TWatchable> : Expectation where TWatchable : IWatchable
    {
        /// <summary>
        /// The default timeout for an <see cref="Expectation{TWatchable}"/>.
        /// </summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        private TWatchable expectedObject = default(TWatchable);
        private TimeSpan expectedTimeout;
        private bool timedOut = false;
        private bool expectationSatisfied = false;
        private bool isDisposed = false;
        private Predicate<TWatchable> criteriaMatched = new Predicate<TWatchable>(expectedObject => expectedObject.Exists);

        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation{TWatchable}"/> class.
        /// </summary>
        public Expectation()
            : this(DefaultTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation{TWatchable}"/> class with the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout by which the expectation should be met.</param>
        public Expectation(TimeSpan timeout)
            : this(timeout, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation{TWatchable}"/> class with the specified criteria.
        /// </summary>
        /// <param name="criteria">A method evaluating the object and determining if the criteria of the expectation are met.</param>
        public Expectation(Predicate<TWatchable> criteria)
            : this(DefaultTimeout, criteria)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expectation{TWatchable}"/> class with the specified criteria.
        /// </summary>
        /// <param name="timeout">The timeout by which the expectation should be met.</param>
        /// <param name="criteria">A method evaluating the object and determining if the criteria of the expectation are met.</param>
        public Expectation(TimeSpan timeout, Predicate<TWatchable> criteria)
        {
            if (timeout.TotalSeconds == 0)
            {
                timeout = DefaultTimeout;
            }

            this.expectedTimeout = timeout;
            if (criteria != null)
            {
                this.criteriaMatched = criteria;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the expectation is satisfied.
        /// </summary>
        public override bool IsSatisfied
        {
            get { return this.expectationSatisfied; }
        }

        /// <summary>
        /// Gets a value indicating whether the timeout was reached.
        /// </summary>
        public override bool TimeoutReached
        {
            get { return this.timedOut; }
        }

        /// <summary>
        /// Gets the object being watched for the expectation.
        /// </summary>
        public TWatchable Object
        {
            get
            {
                this.WaitUntilSatisfied();
                return this.expectedObject; 
            }
        }

        /// <summary>
        /// Waits until the expectation is satisfied or the timeout is reached.
        /// </summary>
        public void WaitUntilSatisfied()
        {
            TryUntilTimeoutExecutor functionExecutor = new TryUntilTimeoutExecutor(this.expectedTimeout);
            this.expectationSatisfied = functionExecutor.Try<bool>(this.IsExpectationSatisfied);
            this.timedOut = functionExecutor.DidTimeOut;
        }

        /// <summary>
        /// Resets this <see cref="Expectation{T}"/> to default values.
        /// </summary>
        public void Reset()
        {
            this.expectedObject = default(TWatchable);
            this.expectationSatisfied = false;
            this.timedOut = false;
        }

        /// <summary>
        /// Sets the object to be watched.
        /// </summary>
        /// <param name="objectToSet">The object to be watched.</param>
        internal override void SetObject(object objectToSet)
        {
            // Verify the object is an IWatchable before setting.
            IWatchable watchableObject = objectToSet as IWatchable;
            if (watchableObject != null)
            {
                this.SetObjectInternal((TWatchable)objectToSet);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.expectedObject != null && !this.expectedObject.Equals(default(TWatchable)))
                    {
                        this.expectedObject.Dispose();
                    }

                    this.expectedObject = default(TWatchable);
                    this.isDisposed = true;
                }
            }

            base.Dispose(disposing);
        }

        private void SetObjectInternal(TWatchable objectToSet)
        {
            this.expectedObject = objectToSet;
        }

        private bool IsExpectationSatisfied()
        {
            return this.expectedObject != null && !this.expectedObject.Equals(default(TWatchable)) && this.criteriaMatched(this.expectedObject);
        }
    }
}
