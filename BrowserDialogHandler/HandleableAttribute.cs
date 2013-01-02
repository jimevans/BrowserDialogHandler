// <copyright file="HandleableAttribute.cs" company="BrowserDialogHandler Project">
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
using System.Text;

namespace BrowserDialogHandler
{
    /// <summary>
    /// Attribute which describes a class that can be handled by a dialog handler.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HandleableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleableAttribute"/> class.
        /// </summary>
        /// <param name="kind">The unique string identifying the class decorated with this attribute.</param>
        public HandleableAttribute(string kind)
        {
            if (!string.IsNullOrEmpty(kind))
            {
                this.Identifier = kind;
            }
        }

        /// <summary>
        /// Gets the identifier for the class decorated with this attribute.
        /// </summary>
        public string Identifier { get; private set; }
    }
}
