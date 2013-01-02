// <copyright file="AssistiveTechnologyObject.cs" company="BrowserDialogHandler Project">
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
using System.Linq;
using System.Text;

namespace BrowserDialogHandler.Native.Windows
{
    /// <summary>
    /// Abstract base class for an assistive technology or accessibility object.
    /// </summary>
    internal abstract class AssistiveTechnologyObject
    {
        /// <summary>
        /// Gets the name of this object.
        /// </summary>
        internal abstract string Name { get; }

        /// <summary>
        /// Gets the <see cref="AccessibleRole"/> for this object.
        /// </summary>
        internal abstract AccessibleRole Role { get; }

        /// <summary>
        /// Gets the list of current <see cref="AccessibleState"/> values for this object.
        /// </summary>
        internal abstract IList<AccessibleState> StateSet { get; }

        /// <summary>
        /// Gets the count of the children of this object.
        /// </summary>
        internal abstract int ChildCount { get; }

        /// <summary>
        /// Gets a value indicating whether this object supports actions.
        /// </summary>
        internal abstract bool SupportsActions { get; }

        /// <summary>
        /// Sets focus to this object.
        /// </summary>
        /// <returns><see langword="true"/> if the focus was properly set; otherwise <see langword="false"/>.</returns>
        internal abstract bool SetFocus();

        /// <summary>
        /// Performs an action on this object.
        /// </summary>
        /// <param name="actionIndex">The index of the action to perform.</param>
        internal abstract void DoAction(int actionIndex);

        /// <summary>
        /// Gets the children of this object by the specified <see cref="AccessibleRole"/>.
        /// </summary>
        /// <param name="matchingRole">The <see cref="AccessibleRole"/> of the children to get.</param>
        /// <param name="visibleChildrenOnly"><see langword="true"/> to only search visible children;
        /// <see langword="false"/> to also return hidden children.</param>
        /// <param name="recursiveSearch"><see langword="true"/> to recursively search all children of this object;
        /// <see langword="false"/> to only search immediate children.</param>
        /// <returns>A list of <see cref="AssistiveTechnologyObject"/> that meet the specified criteria.</returns>
        internal abstract IList<AssistiveTechnologyObject> GetChildrenByRole(AccessibleRole matchingRole, bool visibleChildrenOnly, bool recursiveSearch);
    }
}
