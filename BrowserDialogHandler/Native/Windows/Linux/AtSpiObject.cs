// <copyright file="AtSpiObject.cs" company="BrowserDialogHandler Project">
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
using System.Drawing;
using BrowserDialogHandler.Native.Windows;
    
namespace BrowserDialogHandler.Native.Windows.Linux
{
    /// <summary>
    /// Represents an <see cref="AssistiveTechnologyObject"/> using the AT-SPI assistive technology.
    /// </summary>
    internal class AtSpiObject : AssistiveTechnologyObject
    {
        private IntPtr accessibleObject = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtSpiObject"/> class.
        /// </summary>
        /// <param name="processId">The ID of the process hosting this object.</param>
        /// <param name="windowRect">The rectangle containing this object.</param>
        /// <param name="frameExtents">The <see cref="WindowManagerFrameExtents"/> for the window hosting this object.</param>
        internal AtSpiObject(int processId, Rectangle windowRect, WindowManagerFrameExtents frameExtents)
        {
            Process applicationProcess = Process.GetProcessById(processId);
            string moduleName = applicationProcess.MainModule.ModuleName;
            this.accessibleObject = AtSpi.Instance.MatchTopLevelWindowForApplication(moduleName, windowRect, frameExtents);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtSpiObject"/> class for the specified pointer.
        /// </summary>
        /// <param name="accessibleObjectPointer">A pointer to an already-discovered <see cref="AtSpiObject"/>.</param>
        internal AtSpiObject(IntPtr accessibleObjectPointer)
        {
            this.accessibleObject = accessibleObjectPointer;
        }

        /// <summary>
        /// Gets a value indicating whether this object is valid.
        /// </summary>
        public bool IsValid
        {
            get { return this.Role != AccessibleRole.Invalid; }
        }

        /// <summary>
        /// Gets a value indicating whether this object supports getting its text.
        /// </summary>
        /// <remarks>Should this property be promoted to the base AssistiveTechnologyObject class?</remarks>
        internal bool SupportsText
        {
            get { return AtSpi.Instance.IsTextObject(this.accessibleObject); }
        }

        /// <summary>
        /// Gets the text for this object.
        /// </summary>
        /// <remarks>Should this property be promoted to the base AssistiveTechnologyObject class?</remarks>
        internal string Text
        {
            get
            {
                string textValue = string.Empty;
                if (this.SupportsText)
                {
                    textValue = AtSpi.Instance.GetText(this.accessibleObject);
                }
                else
                {
                    textValue = this.Name;
                }

                return textValue;
            }
        }

        /// <inheritdoc />
        internal override int ChildCount
        {
            get { return AtSpi.Instance.GetChildCount(this.accessibleObject); }
        }

        /// <inheritdoc />
        internal override string Name
        {
            get { return AtSpi.Instance.GetName(this.accessibleObject); }
        }

        /// <inheritdoc />
        internal override AccessibleRole Role
        {
            get { return AtSpi.Instance.GetRole(this.accessibleObject); }
        }

        /// <inheritdoc />
        internal override IList<AccessibleState> StateSet
        {
            get { return AtSpi.Instance.GetStateList(this.accessibleObject); }
        }

        /// <inheritdoc />
        internal override bool SupportsActions
        {
            get { return AtSpi.Instance.IsActionObject(this.accessibleObject); }
        }

        /// <summary>
        /// Sets the text of this object.
        /// </summary>
        /// <param name="text">The text to which to set the text of this object.</param>
        public void SetText(string text)
        {
            AtSpi.Instance.SetText(this.accessibleObject, text);
        }

        /// <summary>
        /// Dereferences this object.
        /// </summary>
        public void UnreferenceObject()
        {
            AtSpi.Instance.UnreferenceAccessibleObject(this.accessibleObject);
        }

        /// <inheritdoc />
        internal override IList<AssistiveTechnologyObject> GetChildrenByRole(AccessibleRole matchingRole, bool visibleChildrenOnly, bool recursiveSearch)
        {
            List<AssistiveTechnologyObject> childObjectList = new List<AssistiveTechnologyObject>();
            IList<IntPtr> childObjectPointerList = AtSpi.Instance.FindChildrenWithRole(this.accessibleObject, matchingRole, visibleChildrenOnly, recursiveSearch);
            foreach (IntPtr childObjectPointer in childObjectPointerList)
            {
                childObjectList.Add(new AtSpiObject(childObjectPointer));
            }

            return childObjectList;
        }

        /// <inheritdoc />
        internal override bool SetFocus()
        {
            return AtSpi.Instance.SetFocus(this.accessibleObject);
        }

        /// <inheritdoc />
        internal override void DoAction(int actionIndex)
        {
            AtSpi.Instance.PerformAction(this.accessibleObject, actionIndex);
        }
    }
}
