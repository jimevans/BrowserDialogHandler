﻿// <copyright file="IEDialogManager.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Native.InternetExplorer.Dialogs;
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native.InternetExplorer
{
    /// <summary>
    /// Dialog manager for IE dialogs.
    /// </summary>
    public class IEDialogManager : DialogManager
    {
        /// <inheritdoc />
        public IEDialogManager(Window mainIeWindow, WindowEnumerationMethod childEnumerationMethod)
            : base(mainIeWindow, childEnumerationMethod)
        {
        }

        /// <inheritdoc />
        protected override void RegisterDialogs()
        {
            RegisteredDialogTypes.Add(typeof(IEJavaScriptDialog));
            RegisteredDialogTypes.Add(typeof(IEVBScriptDialog));
            RegisteredDialogTypes.Add(typeof(IELogonDialog));
        }
    }
}
