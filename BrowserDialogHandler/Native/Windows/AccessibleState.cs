// <copyright file="AccessibleState.cs" company="BrowserDialogHandler Project">
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
    /// Values representing the accessible state of an object.
    /// </summary>
    public enum AccessibleState
    {
        /// <summary>
        /// Invalid state.
        /// </summary>
        Invalid,

        /// <summary>
        /// Active state
        /// </summary>
        Active,

        /// <summary>
        /// Armed state
        /// </summary>
        Armed,

        /// <summary>
        /// Busy state
        /// </summary>
        Busy,

        /// <summary>
        /// Checked state
        /// </summary>
        Checked,

        /// <summary>
        /// Collapsed state
        /// </summary>
        Collapsed,

        /// <summary>
        /// Defunct state
        /// </summary>
        Defunct,

        /// <summary>
        /// Editable state
        /// </summary>
        Editable,

        /// <summary>
        /// Enabled state
        /// </summary>
        Enabled,

        /// <summary>
        /// Expandable state
        /// </summary>
        Expandable,

        /// <summary>
        /// Expanded state
        /// </summary>
        Expanded,

        /// <summary>
        /// Focusable state
        /// </summary>
        Focusable,

        /// <summary>
        /// Focused state
        /// </summary>
        Focused,

        /// <summary>
        /// Horizontal state
        /// </summary>
        Horizontal,

        /// <summary>
        /// Icon state
        /// </summary>
        Iconified,

        /// <summary>
        /// Modal state
        /// </summary>
        Modal,

        /// <summary>
        /// Multiline state
        /// </summary>
        MultiLine,

        /// <summary>
        /// Multiply selectable state
        /// </summary>
        MultiSelectable,

        /// <summary>
        /// Opaque state
        /// </summary>
        Opaque,

        /// <summary>
        /// Pressed state
        /// </summary>
        Pressed,

        /// <summary>
        /// Resizable state
        /// </summary>
        Resizable,

        /// <summary>
        /// Selectable state
        /// </summary>
        Selectable,

        /// <summary>
        /// Selected state
        /// </summary>
        Selected,

        /// <summary>
        /// Sensitive state
        /// </summary>
        Sensitive,

        /// <summary>
        /// Showing state
        /// </summary>
        Showing,

        /// <summary>
        /// Single line state
        /// </summary>
        SingleLine,

        /// <summary>
        /// Stale state
        /// </summary>
        Stale,

        /// <summary>
        /// Transient state
        /// </summary>
        Transient,

        /// <summary>
        /// Vertical state
        /// </summary>
        Vertical,

        /// <summary>
        /// Visible state
        /// </summary>
        Visible,

        /// <summary>
        /// Manages descendants state
        /// </summary>
        ManagesDescendants,

        /// <summary>
        /// Indeterminate state
        /// </summary>
        Indeterminate,

        /// <summary>
        /// Truncated state
        /// </summary>
        Truncated,

        /// <summary>
        /// Required state
        /// </summary>
        Required,

        /// <summary>
        /// Invalid entry state
        /// </summary>
        InvalidEntry,

        /// <summary>
        /// Supports auto-completion state
        /// </summary>
        SupportsAutoCompletion,

        /// <summary>
        /// Selectable text state
        /// </summary>
        SelectableText,

        /// <summary>
        /// Is default state
        /// </summary>
        IsDefault,

        /// <summary>
        /// Visited state
        /// </summary>
        Visited,

        /// <summary>
        /// Protected state
        /// </summary>
        Protected,

        /// <summary>
        /// Last defined state
        /// </summary>
        LastDefined = Protected
    }
}
