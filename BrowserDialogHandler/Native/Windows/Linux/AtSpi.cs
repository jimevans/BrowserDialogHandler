// <copyright file="AtSpi.cs" company="BrowserDialogHandler Project">
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
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using BrowserDialogHandler.Native.Windows;
    
namespace BrowserDialogHandler.Native.Windows.Linux
{
    /// <summary>
    /// Wraps the AT-SPI assistive technology.
    /// </summary>
    internal class AtSpi
    {
        private static AtSpi instance = null;
        private static object lockObject = new object();
        private static List<IntPtr> referencedObjectCache = new List<IntPtr>();

        #region Constructor / destructor
        /// <summary>
        /// Prevents a default instance of the <see cref="AtSpi"/> class from being created.
        /// </summary>
        private AtSpi()
        {
            // Constructor is private, as implemented using Singleton pattern.
            // Only connect to the SPI registry once.
            int result = SPI_init();
            if (result != 0)
            {
                throw new Exception(string.Format("Error initializing AT-SPI. Error code returned was {0}", result.ToString()));
            }
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="AtSpi"/> class.
        /// </summary>
        ~AtSpi()
        {
            // Dereference all of the Accessible objects in the cache
            if (referencedObjectCache.Count > 0)
            {
                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "Found {0} referenced AT-SPI objects. Cleaning up references", referencedObjectCache.Count.ToString()));
                foreach (IntPtr referencedObjectPointer in referencedObjectCache)
                {
                    Accessible_unref(referencedObjectPointer);
                }
            }

            // Destructor/Finalizer used to insure SPI registry disconnect
            // only once.
            int result = SPI_exit();
            if (result != 0)
            {
                Logger.LogInfo(string.Format(CultureInfo.InvariantCulture, "{0} leaks occurred in AT-SPI code", result));
            }
        }
        #endregion

        private enum AccessibleCoordType
        {
            Screen,
            Window
        }

        private enum AccessibleComponentLayer
        {
            Invalid,
            Background,
            Canvas,
            Widget,
            Mdi,
            Popup,
            Overlay,
            Window
        }

        #region Static properties
        /// <summary>
        /// Gets the singleton instance of the AtSpi class.
        /// </summary>
        internal static AtSpi Instance
        {
            get
            {
                // Make the instance property minimally thread safe and lazy loaded.
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new AtSpi();
                    }

                    return instance;
                }
            }
        }
        #endregion

        #region Instance methods
        /// <summary>
        /// Gets the count of the number of children for the Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>The count of children.</returns>
        internal int GetChildCount(IntPtr obj)
        {
            int childCount = -1;
            if (obj != IntPtr.Zero)
            {
                childCount = Accessible_getChildCount(obj);
            }

            return childCount;
        }

        /// <summary>
        /// Gets the name of the Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>The object name.</returns>
        internal string GetName(IntPtr obj)
        {
            string name = string.Empty;
            if (obj != IntPtr.Zero)
            {
                IntPtr namePointer = Accessible_getName(obj);
                if (namePointer != IntPtr.Zero)
                {
                    name = Marshal.PtrToStringAuto(namePointer);
                    SPI_freeString(namePointer);
                }
            }

            return name;
        }

        /// <summary>
        /// Gets the role of the Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>An AccessibleRole value describing the role of the object.</returns>
        internal AccessibleRole GetRole(IntPtr obj)
        {
            AccessibleRole role = AccessibleRole.Invalid;
            if (obj != IntPtr.Zero)
            {
                role = Accessible_getRole(obj);
            }

            return role;
        }

        /// <summary>
        /// Gets a list of the AccessibleState values currently set on the Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>An IList containing all of the set (true) states on the Accessible object.</returns>
        internal IList<AccessibleState> GetStateList(IntPtr obj)
        {
            List<AccessibleState> stateList = new List<AccessibleState>();
            if (obj != IntPtr.Zero)
            {
                // An AccessibleStateSet is returned as an array of name/value pairs
                // where the name is the state, and the value is true or false for
                // that state. However, the API does not allow us to iterate through
                // the states in the set. So, we loop through all available states, 
                // adding those that are true (returned by AccessibleStateSet_contains).
                IntPtr stateSet = Accessible_getStateSet(obj);
                for (AccessibleState candidateState = AccessibleState.Invalid; candidateState < AccessibleState.LastDefined; candidateState++)
                {
                    if (AccessibleStateSet_contains(stateSet, candidateState))
                    {
                        stateList.Add(candidateState);
                    }
                }

                AccessibleStateSet_unref(stateSet);
            }

            return stateList;
        }

        /// <summary>
        /// Gets whether an Accessible object supports a separate text property.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>True if the object supports a separate text property; False otherwise.</returns>
        internal bool IsTextObject(IntPtr obj)
        {
            bool isText = false;
            if (obj != IntPtr.Zero)
            {
                isText = Accessible_isText(obj);
            }

            return isText;
        }

        /// <summary>
        /// Gets whether an Accessible object supports actions.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>True if the object supports actions; False otherwise.</returns>
        internal bool IsActionObject(IntPtr obj)
        {
            bool isAction = false;
            if (obj != IntPtr.Zero)
            {
                isAction = Accessible_isAction(obj);
            }

            return isAction;
        }
        
        /// <summary>
        /// Performs an action on the Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <param name="actionIndex">The zero-based index of the action to perform.</param>
        internal void PerformAction(IntPtr obj, int actionIndex)
        {
            // N.B. We don't need a null pointer check here;
            // IsActionObject() has one already.
            if (this.IsActionObject(obj))
            {
                IntPtr action = Accessible_getAction(obj);
                int actionCount = AccessibleAction_getNActions(action);
                if (actionIndex >= 0 && actionIndex < actionCount)
                {
                    AccessibleAction_doAction(action, actionIndex);
                }

                AccessibleAction_unref(action);
            }
        }

        /// <summary>
        /// Sets focus to an Accessible object
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns><see langword="true"/> if focus was successfully set; otherwise, <see langword="false"/>.</returns>
        internal bool SetFocus(IntPtr obj)
        {
            bool focusWasSet = false;
            if (obj != IntPtr.Zero)
            {
                if (Accessible_isComponent(obj))
                {
                    IntPtr component = Accessible_getComponent(obj);
                    focusWasSet = AccessibleComponent_grabFocus(component);
                    AccessibleComponent_unref(component);
                }
            }

            return focusWasSet;
        }
        
        /// <summary>
        /// Gets the entire text contents of an Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <returns>The text of the object.</returns>
        internal string GetText(IntPtr obj)
        {
            string text = string.Empty;
            if (obj != IntPtr.Zero)
            {
                if (Accessible_isText(obj))
                {
                    IntPtr textObject = Accessible_getText(obj);
                    int length = AccessibleText_getCharacterCount(textObject);
                    if (length > 0)
                    {
                        IntPtr textPointer = AccessibleText_getText(textObject, 0, length);
                        if (textPointer != IntPtr.Zero)
                        {
                            text = Marshal.PtrToStringAnsi(textPointer);
                            SPI_freeString(textPointer);
                        }
                    }

                    AccessibleText_unref(textObject);
                }
            }

            return text;
        }
        
        /// <summary>
        /// Sets the text contents of an Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        /// <param name="text">The text value to set.</param>
        /// <remarks>This method replaces the current text. If the text is not editable, nothing happens.</remarks>
        internal void SetText(IntPtr obj, string text)
        {
            if (obj != IntPtr.Zero)
            {
                if (Accessible_isEditableText(obj))
                {
                    IntPtr editableText = Accessible_getEditableText(obj);
                    AccessibleEditableText_setTextContents(editableText, text);
                    AccessibleEditableText_unref(editableText);
                }
            }
        }

        /// <summary>
        /// Cleans up a reference to an Accessible object.
        /// </summary>
        /// <param name="obj">A pointer to the Accessible object.</param>
        internal void UnreferenceAccessibleObject(IntPtr obj)
        {
            if (referencedObjectCache.Contains(obj))
            {
                referencedObjectCache.Remove(obj);
                Accessible_unref(obj);
            }
        }

        /// <summary>
        /// Gets a list of child Accessible objects that match the desired AccessibleRole.
        /// </summary>
        /// <param name="parent">A pointer to the Accessible object on which to find children.</param>
        /// <param name="desiredRole">An AccessibleRole value describing the type of child to find.</param>
        /// <param name="visibleOnly"><see langword="true"/> to only find visible children; <see langword="false"/> to include invisible children.</param>
        /// <param name="recursive">True to search all children in the object tree; False to search only immediate children.</param>
        /// <returns>A list containing pointers to the Accessible objects matching the desired role.</returns>
        internal IList<IntPtr> FindChildrenWithRole(IntPtr parent, AccessibleRole desiredRole, bool visibleOnly, bool recursive)
        {
            List<IntPtr> childList = new List<IntPtr>();
            int childCount = Accessible_getChildCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                IntPtr childPointer = Accessible_getChildAtIndex(parent, i);
                AccessibleRole childRole = Accessible_getRole(childPointer);
                IList<AccessibleState> stateList = this.GetStateList(childPointer);

                // If the child object has the desired role and visibility, add it to both the cached object
                // list and the returned object list.
                if ((childRole == desiredRole || desiredRole == AccessibleRole.AnyRole) && (!visibleOnly || stateList.Contains(AccessibleState.Visible)))
                {
                    referencedObjectCache.Add(childPointer);
                    childList.Add(childPointer);
                }

                // Call the function recursively to find all children at any depth.
                if (recursive)
                {
                    childList.AddRange(this.FindChildrenWithRole(childPointer, desiredRole, visibleOnly, recursive));
                }

                // If the object didn't match the desired role, we can unreference it.
                if (!childList.Contains(childPointer))
                {
                    Accessible_unref(childPointer);
                }
            }

            return childList;
        }

        /// <summary>
        /// Matches a top-level window to the application to the window having the specified dimensions.
        /// </summary>
        /// <param name="applicationName">The name of the application as registered with the AT-SPI registry.</param>
        /// <param name="windowRect">A <see cref="System.Drawing.Rectangle"/> representing the window on the screen.</param>
        /// <param name="frameExtents">A WindowManagerFrameExtents structure containing the size (in pixels) of the
        /// decorations placed on the window by the window manager.</param>
        /// <returns>A <see cref="System.IntPtr"/> value that points to the Accessible object representing the top-level window.</returns>
        internal IntPtr MatchTopLevelWindowForApplication(string applicationName, Rectangle windowRect, WindowManagerFrameExtents frameExtents)
        {
            IntPtr windowPointer = IntPtr.Zero;
            IList<IntPtr> topLevelWindows = this.GetTopLevelWindowsForApplication(applicationName);
            foreach (IntPtr candidateWindow in topLevelWindows)
            {
                if (Accessible_isComponent(candidateWindow))
                {
                    // Out variables for API call
                    int x = 0;
                    int y = 0;
                    int width = 0;
                    int height = 0;
                    int relativeX = 0;
                    int relativeY = 0;

                    IntPtr component = Accessible_getComponent(candidateWindow);
                    AccessibleComponent_getExtents(component, out x, out y, out width, out height, AccessibleCoordType.Screen);
                    AccessibleComponent_getPosition(component, out relativeX, out relativeY, AccessibleCoordType.Window);
                    AccessibleComponentLayer layer = AccessibleComponent_getLayer(component);
                    Rectangle componentRectangle = new Rectangle(x, y, width, height);
                    AccessibleComponent_unref(component);

                    // If the component layer is Window, this indicates that the window
                    // rect as passed in by the X server does not include the window frame
                    // as drawn by the window manager. If this is the case, adjust the rectangle
                    // so the window can be found.
                    if (layer == AccessibleComponentLayer.Window)
                    {
                        windowRect.X = windowRect.X - frameExtents.Left;
                        windowRect.Y = windowRect.Y - frameExtents.Top;
                        windowRect.Width = windowRect.Width + frameExtents.Left + frameExtents.Right;
                        windowRect.Height = windowRect.Height + frameExtents.Top + frameExtents.Bottom;
                    }

                    // If we found the window, add its pointer to the referenced objects
                    // cache and remove it from the list of available windows (so it 
                    // does not get unreferenced.
                    if (windowRect.Equals(componentRectangle))
                    {
                        referencedObjectCache.Add(candidateWindow);
                        topLevelWindows.Remove(candidateWindow);
                        windowPointer = candidateWindow;
                        break;
                    }
                }
            }

            // Unreference every window not matching as the top-level window
            // for the application.
            foreach (IntPtr referencedWindow in topLevelWindows)
            {
                Accessible_unref(referencedWindow);
            }

            return windowPointer;
        }
        #endregion

        #region AT-SPI native methods (accessibility api)
        [DllImport("libcspi.so.0")]
        private static extern int SPI_init();

        [DllImport("libcspi.so.0")]
        private static extern int SPI_exit();

        [DllImport("libcspi.so.0")]
        private static extern int SPI_getDesktopCount();

        [DllImport("libcspi.so.0")]
        private static extern IntPtr SPI_getDesktop(int index);

        [DllImport("libcspi.so.0")]
        private static extern void SPI_freeString(IntPtr s);

        [DllImport("libcspi.so.0")]
        private static extern void Accessible_unref(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getAction(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern int Accessible_getChildCount(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getChildAtIndex(IntPtr obj, int index);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getComponent(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getEditableText(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getName(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern AccessibleRole Accessible_getRole(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getStateSet(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr Accessible_getText(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool Accessible_isAction(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool Accessible_isApplication(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool Accessible_isComponent(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool Accessible_isEditableText(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool Accessible_isText(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleAction_unref(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool AccessibleAction_doAction(IntPtr obj, int index);

        [DllImport("libcspi.so.0")]
        private static extern int AccessibleAction_getNActions(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleComponent_unref(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleComponent_getExtents(IntPtr obj, out int x, out int y, out int width, out int height, AccessibleCoordType ctype);

        [DllImport("libcspi.so.0")]
        private static extern AccessibleComponentLayer AccessibleComponent_getLayer(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleComponent_getPosition(IntPtr obj, out int x, out int y, AccessibleCoordType ctype);

        [DllImport("libcspi.so.0")]
        private static extern bool AccessibleComponent_grabFocus(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleEditableText_unref(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool AccessibleEditableText_setTextContents(IntPtr obj, string newContents);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleStateSet_unref(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern bool AccessibleStateSet_contains(IntPtr obj, AccessibleState index);

        [DllImport("libcspi.so.0")]
        private static extern void AccessibleText_unref(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern int AccessibleText_getCharacterCount(IntPtr obj);

        [DllImport("libcspi.so.0")]
        private static extern IntPtr AccessibleText_getText(IntPtr obj, int startOffset, int endOffset);
        #endregion

        #region Private methods
        /// <summary>
        /// Gets a pointer to the Accessible object representing an application
        /// </summary>
        /// <param name="appName">The name of the application as registered with the AT-SPI registry.</param>
        /// <returns>A pointer to the accessibility object for the application.</returns>
        private IntPtr GetApplication(string appName)
        {
            IntPtr returnPointer = IntPtr.Zero;

            // Loop through each desktop on the system
            int desktopCount = SPI_getDesktopCount();
            for (int desktopIndex = 0; desktopIndex < desktopCount; desktopIndex++)
            {
                // Applications should be direct children of the desktop
                IntPtr desktop = SPI_getDesktop(desktopIndex);
                int childCount = Accessible_getChildCount(desktop);
                for (int childIndex = 0; childIndex < childCount; childIndex++)
                {
                    IntPtr appPointer = Accessible_getChildAtIndex(desktop, childIndex);
                    if (appPointer != IntPtr.Zero)
                    {
                        IntPtr namePointer = Accessible_getName(appPointer);
                        string name = Marshal.PtrToStringAnsi(namePointer);
                        SPI_freeString(namePointer);

                        // The application name passed in is likely to be the full path
                        // and file name of the executable. The Accessible application
                        // name is just the exe name, possible with mixed case (e.g. 'Firefox').
                        // Flatten out the case, and look for a match via contains.
                        bool childIsApplication = Accessible_isApplication(appPointer);
                        if (childIsApplication && appName.ToLower().Contains(name.ToLower()))
                        {
                            returnPointer = appPointer;
                            break;
                        }
                        else
                        {
                            // If this is not a matching application, we can free
                            // the pointer
                            Accessible_unref(appPointer);
                        }
                    }
                }

                // Free the desktop pointer, and exit if we found the application.
                Accessible_unref(desktop);
                if (returnPointer != IntPtr.Zero)
                {
                    break;
                }
            }

            return returnPointer;
        }
        
        /// <summary>
        /// Gets Accessible objects representing the top level windows for an application.
        /// </summary>
        /// <param name="applicationName">The name of the application as registered with the AT-SPI registry.</param>
        /// <returns>A List of pointers to the Accessible objects.</returns>
        private IList<IntPtr> GetTopLevelWindowsForApplication(string applicationName)
        {
            List<IntPtr> windowList = new List<IntPtr>();
            IntPtr application = this.GetApplication(applicationName);
            if (Accessible_isApplication(application))
            {
                int childCount = Accessible_getChildCount(application);
                for (int i = 0; i < childCount; i++)
                {
                    // A top-level window should be a direct child of the application
                    // object. Accessible objects with roles of Dialog, Window, Alert
                    // or Frame are top-level windows for the application.
                    IntPtr childPointer = Accessible_getChildAtIndex(application, i);
                    AccessibleRole childRole = Accessible_getRole(childPointer);
                    if (childRole == AccessibleRole.Dialog || 
                        childRole == AccessibleRole.Window || 
                        childRole == AccessibleRole.Alert || 
                        childRole == AccessibleRole.Frame)
                    {
                        windowList.Add(childPointer);
                    }
                }
            }

            // We receive one and only one application pointer from GetApplication(),
            // so it can be unref'ed here.
            Accessible_unref(application);
            return windowList;
        }
        #endregion
    }
}
