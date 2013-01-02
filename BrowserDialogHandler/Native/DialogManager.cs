// <copyright file="DialogManager.cs" company="BrowserDialogHandler Project">
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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native
{
    /// <summary>
    /// Represents a manager for dialogs.
    /// </summary>
    public abstract class DialogManager : INativeDialogManager
    {
        private static object lockObject = new object();

        private readonly Thread watcherThread;
        private List<IntPtr> dialogHandleList = new List<IntPtr>();
        private List<Type> registeredDialogTypeList = new List<Type>();
        private bool keepRunning = true;
        private Window mainWindow = null;
        private bool useWindowManagementApi = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogManager"/> class.
        /// </summary>
        /// <param name="monitoredWindow">A <see cref="BrowserDialogHandler.Native.Windows.Window"/> object representing the window to be monitored for dialogs.</param>
        /// <param name="childEnumerationMethod">A <see cref="BrowserDialogHandler.Native.Windows.WindowEnumerationMethod"/> value representing the method used to enumerate child windows on the dialog.</param>
        /// <remarks>Protected constructor to prevent instantiation of abstract base class.</remarks>
        protected DialogManager(Window monitoredWindow, WindowEnumerationMethod childEnumerationMethod)
        {
            this.mainWindow = monitoredWindow;
            this.useWindowManagementApi = childEnumerationMethod == WindowEnumerationMethod.WindowManagementApi;
            this.RegisterDialogs();
            this.dialogHandleList.Add(this.mainWindow.Handle);
            this.watcherThread = new Thread(this.Start);
            this.watcherThread.Start();
        }

        /// <inheritdoc />
        public event EventHandler<NativeDialogFoundEventArgs> DialogFound;

        /// <summary>
        /// Gets the list of registered dialog types.
        /// </summary>
        protected List<Type> RegisteredDialogTypes
        {
            get { return this.registeredDialogTypeList; }
        }

        private bool IsRunning
        {
            get { return this.watcherThread.IsAlive; }
        }

        /// <summary>
        /// Releases all managed and unmanaged resources used by this class.
        /// </summary>
        public void Dispose()
        {
            lock (lockObject)
            {
                this.keepRunning = false;
            }

            if (this.IsRunning)
            {
                this.watcherThread.Join();
            }
        }

        /// <summary>
        /// Registers the dialogs with this <see cref="DialogManager"/>.
        /// </summary>
        protected abstract void RegisterDialogs();

        /// <summary>
        /// Raises the <see cref="DialogFound"/> event when a dialog matching one of the types registered with the manager is found.
        /// </summary>
        /// <param name="e">A <see cref="NativeDialogFoundEventArgs"/> containing the information about the event.</param>
        protected void OnDialogFound(NativeDialogFoundEventArgs e)
        {
            if (this.DialogFound != null)
            {
                try
                {
                    Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Rasing dialog found event: {0}", e.NativeDialog.Kind));
                    this.DialogFound(this, e);
                }
                catch (Exception ex)
                {
                    Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Exception found handling DialogFound event: {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// Starts this <see cref="DialogManager"/>. Called by the constructor to start watching popups
        /// on a separate thread.
        /// </summary>
        private void Start()
        {
            while (this.keepRunning)
            {
                if (this.mainWindow.Exists)
                {
                    IList<Window> windows = WindowFactory.GetWindows(win => true, this.useWindowManagementApi);

                    foreach (var activeWindow in windows)
                    {
                        if (activeWindow.IsDialogWindowFor(this.mainWindow))
                        {
                            INativeDialog dialog;
                            if (this.TryMatchWindow(activeWindow, out dialog))
                            {
                                // If we have a matching window, an INativeDialog is created from it,
                                // so perform the following steps: (1) add the window handle to the 
                                // dialog handle list so the DialogFound event does not continue to 
                                // fire on subsequent loops, (2) attach to the DialogDismissed event
                                // to allow logging for when the dialog is handled, and (3) fire the
                                // DialogFound event.
                                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Handling dialog: {0}", dialog.Kind));
                                this.dialogHandleList.Add(activeWindow.Handle);
                                dialog.DialogDismissed += new EventHandler(this.DialogDismissedHandler);
                                this.OnDialogFound(new NativeDialogFoundEventArgs(dialog));
                            }
                        }
                        else
                        {
                            activeWindow.Dispose();
                        }
                    }

                    if (!this.keepRunning)
                    {
                        return;
                    }

                    // Keep DialogWatcher responsive during 1 second sleep period
                    var count = 0;
                    while (this.keepRunning && count < 5)
                    {
                        Thread.Sleep(200);
                        count++;
                    }
                }
                else
                {
                    this.keepRunning = false;
                }
            }
        }

        private void DialogDismissedHandler(object sender, EventArgs e)
        {
            // CONSIDER: Do we want to bubble this event up, so that the
            // INativeDialogManager object has events for DialogFound and
            // DialogDismissed?
            INativeDialog dialog = sender as INativeDialog;
            if (dialog != null && this.dialogHandleList.Contains(dialog.DialogWindow.Handle))
            {
                this.dialogHandleList.Remove(dialog.DialogWindow.Handle);
                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Successfully handled dialog: {0}", dialog.Kind));
            }
        }

        private bool TryMatchWindow(Window activeWindow, out INativeDialog dialog)
        {
            bool windowMatched = false;
            dialog = null;
            if (!this.dialogHandleList.Contains(activeWindow.Handle))
            {
                foreach (Type knownWindowType in this.registeredDialogTypeList)
                {
                    ConstructorInfo ctor = knownWindowType.GetConstructor(Type.EmptyTypes);
                    INativeDialog candidateDialog = ctor.Invoke(null) as INativeDialog;
                    if (candidateDialog != null)
                    {
                        if (candidateDialog.WindowIsDialogInstance(activeWindow))
                        {
                            dialog = candidateDialog;
                            dialog.DialogWindow = activeWindow;
                            windowMatched = true;
                            break;
                        }
                        else
                        {
                            candidateDialog.Dispose();
                        }
                    }
                }
            }

            return windowMatched;
        }
    }
}
