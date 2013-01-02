// <copyright file="WatcherFactory.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Interfaces;
using BrowserDialogHandler.Native.InternetExplorer;
using BrowserDialogHandler.Native.Mozilla;
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class WatcherFactory
    {
        /// <summary>
        /// Creates a <see cref="IWatcher"/> for the specified browser type.
        /// </summary>
        /// <param name="browser">The <see cref="BrowserType"/> for which to create the watcher.</param>
        /// <returns>The <see cref="DialogWatcher"/> for the browser type.</returns>
        public static IWatcher CreateDialogWatcher(BrowserType browser)
        {
            INativeDialogManager manager = null;
            switch (browser)
            {
                case BrowserType.InternetExplorer:
                    IList<Window> internetExplorerWindows = WindowFactory.GetWindows((win) => { return win.ClassName == "IEFrame"; });
                    if (internetExplorerWindows.Count == 0)
                    {
                        throw new InvalidOperationException("No instances of Internet Explorer are currently running");
                    }

                    manager = new IEDialogManager(internetExplorerWindows[0], WindowEnumerationMethod.WindowManagementApi);
                    break;

                case BrowserType.Firefox:
                    IList<Window> firefoxWindows = WindowFactory.GetWindows((win) => { return win.ClassName == "MozillaWindowClass"; });
                    if (firefoxWindows.Count == 0)
                    {
                        throw new InvalidOperationException("No instances of Internet Explorer are currently running");
                    }

                    manager = new FFDialogManager(firefoxWindows[0], WindowEnumerationMethod.WindowManagementApi);
                    break;
            }

            if (manager == null)
            {
                throw new NotImplementedException("Watching not implemented for " + browser.ToString());
            }

            return new DialogWatcher(manager, null);
        }
    }
}
