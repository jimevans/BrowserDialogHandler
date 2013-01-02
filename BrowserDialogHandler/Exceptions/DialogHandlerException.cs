// <copyright file="DialogHandlerException.cs" company="BrowserDialogHandler Project">
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
using System.Globalization;
using System.Runtime.Serialization;

namespace BrowserDialogHandler.Exceptions
{
    /// <summary>
    /// Base class for Exceptions thrown by DialogHandler.
    /// </summary>
    [Serializable]
    public class DialogHandlerException : Exception
    {
        /// <inheritdoc />
        public DialogHandlerException()
        {
        }

        /// <inheritdoc />
        public DialogHandlerException(string message)
            : base(message)
        {
            Logger.LogDebug(string.Format(CultureInfo.InvariantCulture, "Exception: {0}, {1}\n{2}", this.GetType().Name, message, this.StackTrace != null ? this.StackTrace : string.Empty));
        }

        /// <inheritdoc />
        public DialogHandlerException(string message, Exception innerexception)
            : base(message, innerexception)
        {
            Logger.LogDebug(string.Format(CultureInfo.InvariantCulture, "Exception: {0}, {1}\nInner: {2}\n{3}\n{4}", this.GetType().Name, message, innerexception.Message, innerexception.Source, this.StackTrace != null ? this.StackTrace : string.Empty));
        }

        /// <inheritdoc />
        public DialogHandlerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}