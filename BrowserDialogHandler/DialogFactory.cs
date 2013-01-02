// <copyright file="DialogFactory.cs" company="BrowserDialogHandler Project">
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
using System.Reflection;
using System.Text;
using BrowserDialogHandler.Dialogs;
using BrowserDialogHandler.Exceptions;
using BrowserDialogHandler.Interfaces;
using BrowserDialogHandler.Native;

namespace BrowserDialogHandler
{
    /// <summary>
    /// A factory class constructing classes representing dialogs.
    /// </summary>
    internal static class DialogFactory
    {
        private static Dictionary<string, ConstructorInfo> dialogConstructors = new Dictionary<string, ConstructorInfo>();

        static DialogFactory()
        {
            Type[] exportedTypes = Assembly.GetExecutingAssembly().GetExportedTypes();
            foreach (Type exportedType in exportedTypes)
            {
                if (exportedType.IsSubclassOf(typeof(Dialog)))
                {
                    RegisterDialogType(exportedType, true);
                }
            }
        }

        /// <summary>
        /// Registers a <see cref="Type"/> to represent a dialog.
        /// </summary>
        /// <param name="dialogType">The <see cref="Type"/> representing the dialog.</param>
        public static void RegisterDialogType(Type dialogType)
        {
            RegisterDialogType(dialogType, false);
        }

        /// <summary>
        /// Creates a <see cref="Dialog"/> wrapper for the specified <see cref="INativeDialog"/>.
        /// </summary>
        /// <param name="nativeDialog">The <see cref="INativeDialog"/> to wrap.</param>
        /// <returns>The wrapped <see cref="Dialog"/>.</returns>
        internal static Dialog CreateDialog(INativeDialog nativeDialog)
        {
            Dialog dialogInstance = null;
            if (dialogConstructors.ContainsKey(nativeDialog.Kind))
            {
                dialogInstance = dialogConstructors[nativeDialog.Kind].Invoke(new object[] { nativeDialog }) as Dialog;
            }

            return dialogInstance;
        }

        /// <summary>
        /// Registers a <see cref="Type"/> to represent a dialog.
        /// </summary>
        /// <param name="dialogType">The <see cref="Type"/> representing the dialog.</param>
        /// <param name="precheckedForDialogSubclass"><see langword="true"/> if the <see cref="Type"/> being
        /// registered has already been verified to be a subclass of <see cref="Dialog"/>; otherwise, <see langword="false"/>.</param>
        private static void RegisterDialogType(Type dialogType, bool precheckedForDialogSubclass)
        {
            // External or user-defined Types need to be checked that they descend from Dialog. 
            if (!precheckedForDialogSubclass && !dialogType.IsSubclassOf(typeof(Dialog)))
            {
                throw new DialogHandlerException(string.Format("Type '{0}' is not descended from Dialog", dialogType.Name));
            }

            // Register classes that have an attribute of Handleable, with a unique Identifier.
            object[] attributes = dialogType.GetCustomAttributes(typeof(HandleableAttribute), false);
            if (attributes.Length > 0)
            {
                HandleableAttribute attribute = attributes[0] as HandleableAttribute;
                if (attribute != null && !string.IsNullOrEmpty(attribute.Identifier))
                {
                    ConstructorInfo ctor = dialogType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(INativeDialog) }, null);
                    if (dialogConstructors.ContainsKey(attribute.Identifier))
                    {
                        throw new DialogHandlerException(string.Format("Duplicate dialog kind for '{0}'", attribute.Identifier));
                    }

                    if (ctor == null)
                    {
                        throw new DialogHandlerException(string.Format("No constructor with parameter INativeDialog found for '{0}'", attribute.Identifier));
                    }

                    dialogConstructors.Add(attribute.Identifier, ctor);
                }
            }
        }
    }
}
