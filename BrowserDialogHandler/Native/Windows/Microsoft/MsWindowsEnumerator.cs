// <copyright file="MsWindowsEnumerator.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native.Windows.Microsoft
{
    /// <summary>
    /// Class for enumerating windows on Microsoft Windows operating systems.
    /// </summary>
    internal class MsWindowsEnumerator
    {
        /// <summary>
        /// Gets a list of top-level windows matching the specified criteria.
        /// </summary>
        /// <param name="constraint">A <see cref="WindowCriteriaConstraint"/> delegate constraining the criteria for the windows to find.</param>
        /// <returns>A list of Window objects representing the top-level windows on the screen.</returns>
        /// <remarks>Using a window constraint of "w => true" returns all top-level windows.</remarks>
        internal IList<Window> GetWindows(WindowCriteriaConstraint constraint)
        {
            var windows = new List<Window>();

            MsWindowsNativeMethods.EnumWindows(
                (hwnd, lParam) =>
                {
                    Window window = new MsWindowsWindow(hwnd);
                    if (constraint == null || constraint(window))
                    {
                        windows.Add(window);
                    }

                    return true;
                }, 
                IntPtr.Zero);

            return windows;
        }

        /// <summary>
        /// Get all child windows for the specific windows handle meeting the specified criteria. 
        /// </summary>
        /// <param name="hwnd">A <see cref="System.IntPtr"/> value representing the handle to the parent window.</param>
        /// <param name="constraint">A <see cref="WindowCriteriaConstraint"/> delegate constraining the criteria for the child windows to find.</param>
        /// <returns>A list of Window objects representing the child windows on the screen.</returns>
        internal IList<Window> GetChildWindows(IntPtr hwnd, WindowCriteriaConstraint constraint)
        {
            var childWindows = new List<Window>();

            MsWindowsNativeMethods.EnumChildWindows(
                hwnd,
                (childHwnd, lParam) =>
                {
                    MsWindowsWindow childWindow = new MsWindowsWindow(childHwnd, lParam);
                    if (constraint == null || constraint(childWindow))
                    {
                        childWindows.Add(childWindow);
                    }

                    return true;
                },
                hwnd);

            return childWindows;
        }
    }
}
