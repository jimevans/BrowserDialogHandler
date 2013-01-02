// <copyright file="DialogHandlerTimeoutException.cs" company="BrowserDialogHandler Project">
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
using System.Runtime.Serialization;

namespace BrowserDialogHandler.Exceptions
{
    /// <summary>
    /// Thrown if waiting for a webpage or element times out.
    /// </summary>
    [Serializable]
    public class DialogHandlerTimeoutException : Exception
    {
        /// <inheritdoc />
        public DialogHandlerTimeoutException(string value)
            : base("Timeout while " + value)
        {
        }

        /// <inheritdoc />
        public DialogHandlerTimeoutException(string value, Exception innerException)
            : base("Timeout while " + value, innerException)
        {
        }

        /// <inheritdoc />
        public DialogHandlerTimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}