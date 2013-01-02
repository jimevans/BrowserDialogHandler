// <copyright file="TryUntilTimeoutExecutor.cs" company="BrowserDialogHandler Project">
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
using System.Threading;

namespace BrowserDialogHandler.UtilityClasses
{
    /// <summary>
    /// Delegate representing code to execute until a timeout.
    /// </summary>
    /// <typeparam name="T">Return type for the delegate.</typeparam>
    /// <returns>A value of type T.</returns>
    public delegate T DoFunc<T>();

    /// <summary>
    /// Delegate for building the exception message.
    /// </summary>
    /// <returns>The string containing the exception message.</returns>
    public delegate string BuildTimeoutExceptionMessage();

    /// <summary>
    /// This class provides an easy way of retrying an action for a given number of seconds.
    /// <example>
    /// The following code shows a basic usage:
    /// <code>
    /// var action = new TryUntilTimeoutExecutor(5);
    /// var result = action.Try(() => false == true);
    /// </code>
    /// </example>
    /// </summary>
    public class TryUntilTimeoutExecutor
    {
        private readonly SimpleTimer timer;
        private readonly TimeSpan timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="TryUntilTimeoutExecutor"/> class.
        /// </summary>
        /// <param name="timeout">The timeout in seconds.</param>
        public TryUntilTimeoutExecutor(TimeSpan timeout)
            : this()
        {
            this.timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TryUntilTimeoutExecutor"/> class.
        /// </summary>
        /// <param name="timer">The timer instance which will be used when executing <see cref="Try{T}(DoFunc{T})"/>.</param>
        public TryUntilTimeoutExecutor(SimpleTimer timer)
            : this()
        {
            this.timer = timer;
        }

        private TryUntilTimeoutExecutor()
        {
            this.SleepTime = TimeSpan.FromMilliseconds(100);
        }

        /// <summary>
        /// Gets or sets the maximum interval between retries of the action.
        /// </summary>
        public TimeSpan SleepTime { get; set; }
        
        /// <summary>
        /// Gets the time out period.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout
        {
            get { return this.timer != null ? this.timer.Timeout : this.timeout; }
        }
        
        /// <summary>
        /// Gets the last exception (thrown by the action) before the time out occurred.
        /// </summary>
        /// <value>The last exception.</value>
        public Exception LastException { get; private set; }
        
        /// <summary>
        /// Gets a value indicating whether a time out occurred.
        /// </summary>
        /// <value><c>true</c> if did time out; otherwise, <c>false</c>.</value>
        public bool DidTimeOut { get; private set; }
        
        /// <summary>
        /// Gets or sets the exception message. If set a <see cref="TimeoutException"/> will be thrown
        /// if the action did time out.
        /// </summary>
        /// <value>The exception message.</value>
        public BuildTimeoutExceptionMessage ExceptionMessage { get; set; }

        /// <summary>
        /// Tries the specified action until the result of the action is not equal to <c>default{T}</c>
        /// or the time out is reached.
        /// </summary>
        /// <typeparam name="T">The result type of the action</typeparam>
        /// <param name="timeout">The timeout before the action is deemed to have failed.</param>
        /// <param name="func">The action.</param>
        /// <returns>The result of the action of <c>default{T}</c> when time out occurred.</returns>
        public static T Try<T>(TimeSpan timeout, DoFunc<T> func)
        {
            var tryFunc = new TryUntilTimeoutExecutor(timeout);
            return tryFunc.Try(func);
        }

        /// <summary>
        /// Tries the specified action until the result of the action is not equal to <c>default{T}</c>
        /// or the time out is reached.
        /// </summary>
        /// <typeparam name="T">The result type of the action</typeparam>
        /// <param name="func">The action.</param>
        /// <returns>The result of the action of <c>default{T}</c> when time out occurred.</returns>
        public T Try<T>(DoFunc<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            var timeoutTimer = this.GetTimer();

            var currentSleepTime = TimeSpan.FromMilliseconds(1);
            do
            {
                this.LastException = null;

                try
                {
                    var result = func.Invoke();
                    if (!result.Equals(default(T)))
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    this.LastException = e;
                }

                this.Sleep(currentSleepTime);

                currentSleepTime += currentSleepTime;
                if (currentSleepTime > this.SleepTime)
                {
                    currentSleepTime = this.SleepTime;
                }
            }
            while (!timeoutTimer.Elapsed);

            this.HandleTimeOut();

            return default(T);
        }

        /// <summary>
        /// Pauses for the specified amount of time between attempts.
        /// </summary>
        /// <param name="sleepTime">The amount of time to pause between attempts.</param>
        protected virtual void Sleep(TimeSpan sleepTime)
        {
            Thread.Sleep(sleepTime);
        }

        private static void ThrowTimeoutException(Exception lastException, string message)
        {
            if (lastException != null)
            {
                throw new Exceptions.DialogHandlerTimeoutException(message, lastException);
            }

            throw new Exceptions.DialogHandlerTimeoutException(message);
        }

        private SimpleTimer GetTimer()
        {
            return this.timer ?? new SimpleTimer(this.Timeout);
        }

        private void HandleTimeOut()
        {
            this.DidTimeOut = true;

            if (this.ExceptionMessage != null)
            {
                ThrowTimeoutException(this.LastException, this.ExceptionMessage.Invoke());
            }
        }
    }
}
