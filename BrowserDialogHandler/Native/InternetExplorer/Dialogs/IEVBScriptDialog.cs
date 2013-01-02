// <copyright file="IEVBScriptDialog.cs" company="BrowserDialogHandler Project">
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
using BrowserDialogHandler.Dialogs;
using BrowserDialogHandler.Native.Windows;

namespace BrowserDialogHandler.Native.InternetExplorer.Dialogs
{
    /// <summary>
    /// Represents a dialog created via VBScript, which only IE supports.
    /// </summary>
    internal class IEVBScriptDialog : NativeDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IEVBScriptDialog"/> class.
        /// </summary>
        public IEVBScriptDialog()
        {
            this.Kind = NativeDialogConstants.VBScriptOKOnlyDialog;
        }

        /// <inheritdoc />
        public override object GetProperty(string propertyId)
        {
            object propertyValue = null;
            if (propertyId == NativeDialogConstants.TitleProperty)
            {
                propertyValue = this.DialogWindow.Text;
            }
            else if (propertyId == NativeDialogConstants.MessageProperty)
            {
                IList<Window> staticLabel = this.DialogWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.Label, true) && w.ItemId == 0xFFFF);
                propertyValue = staticLabel[0].Text;
                WindowFactory.DisposeWindows(staticLabel);
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid property name '{0}'", propertyId), "propertyId");
            }

            return propertyValue;
        }

        /// <inheritdoc />
        public override void PerformAction(string actionId, object[] args)
        {
            int buttonId = this.GetButtonId(this.Kind, actionId);
            if (buttonId > 0)
            {
                IList<Window> buttons = this.DialogWindow.GetChildWindows(b => b.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, true) && b.ItemId == buttonId);
                if (buttons.Count > 0)
                {
                    buttons[0].Press();
                }

                WindowFactory.DisposeWindows(buttons);
                this.WaitForWindowToDisappear();
            }
            else
            {
                throw new ArgumentException(string.Format("Invalid action name '{0}' for dialog type {1}", actionId, Kind), "actionId");
            }
        }

        /// <inheritdoc />
        public override bool WindowIsDialogInstance(Window candidateWindow)
        {
            bool windowIsDialog = candidateWindow.Text.ToLower().Contains("vbscript");
            if (windowIsDialog)
            {
                IList<Window> buttons = candidateWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.PushButton, true));
                IList<Window> staticLabel = candidateWindow.GetChildWindows(w => w.ClassName == WindowFactory.GetWindowClassForRole(AccessibleRole.Label, true) && w.ItemId == 0xFFFF);
                if (buttons.Count == 1 && buttons[0].ItemId == 2 && staticLabel.Count == 1)
                {
                    this.Kind = NativeDialogConstants.VBScriptOKOnlyDialog;
                    windowIsDialog = true;
                }
                else if (buttons.Count == 2 && staticLabel.Count == 1)
                {
                    this.Kind = NativeDialogConstants.VBScriptOKCancelDialog;
                    if (buttons[0].ItemId == 4 && buttons[1].ItemId == 2)
                    {
                        this.Kind = NativeDialogConstants.VBScriptRetryCancelDialog;
                    }
                    else if (buttons[0].ItemId == 6 && buttons[1].ItemId == 7)
                    {
                        this.Kind = NativeDialogConstants.VBScriptYesNoDialog;
                    }

                    windowIsDialog = true;
                }
                else if (buttons.Count == 3 && staticLabel.Count == 1)
                {
                    this.Kind = NativeDialogConstants.VBScriptYesNoCancelDialog;
                    if (buttons[0].ItemId == 3 && buttons[1].ItemId == 4 && buttons[2].ItemId == 5)
                    {
                        this.Kind = NativeDialogConstants.VBScriptAbortRetryIgnoreDialog;
                    }

                    windowIsDialog = true;
                }

                WindowFactory.DisposeWindows(buttons);
                WindowFactory.DisposeWindows(staticLabel);
            }

            return windowIsDialog;
        }

        private int GetButtonId(string dialogKind, string actionId)
        {
            int buttonId = 0;
            if (dialogKind == NativeDialogConstants.VBScriptOKOnlyDialog)
            {
                buttonId = 2;
            }
            else if (dialogKind == NativeDialogConstants.VBScriptOKCancelDialog)
            {
                if (actionId == NativeDialogConstants.ClickOkAction)
                {
                    buttonId = 1;
                }
                else if (actionId == NativeDialogConstants.ClickCancelAction)
                {
                    buttonId = 2;
                }
            }
            else if (dialogKind == NativeDialogConstants.VBScriptYesNoDialog)
            {
                if (actionId == NativeDialogConstants.ClickYesAction)
                {
                    buttonId = 6;
                }
                else if (actionId == NativeDialogConstants.ClickNoAction)
                {
                    buttonId = 7;
                }
            }
            else if (dialogKind == NativeDialogConstants.VBScriptRetryCancelDialog)
            {
                if (actionId == NativeDialogConstants.ClickRetryAction) 
                {
                    buttonId = 4;
                }
                else if (actionId == NativeDialogConstants.ClickCancelAction)
                {
                    buttonId = 2;
                }
            }
            else if (dialogKind == NativeDialogConstants.VBScriptYesNoCancelDialog)
            {
                if (actionId == NativeDialogConstants.ClickYesAction) 
                {
                    buttonId = 6;
                }
                else if (actionId == NativeDialogConstants.ClickNoAction)
                {
                     buttonId = 7;
               }
                else if (actionId == NativeDialogConstants.ClickCancelAction)
                {
                     buttonId = 2;
               }
            }
            else if (dialogKind == NativeDialogConstants.VBScriptAbortRetryIgnoreDialog)
            {
                if (actionId == NativeDialogConstants.ClickAbortAction) 
                {
                    buttonId = 3;
                }
                else if (actionId == NativeDialogConstants.ClickRetryAction)
                {
                    buttonId = 4;
                }
                else if (actionId == NativeDialogConstants.ClickIgnoreAction)
                {
                    buttonId = 5;
                }
            }

            return buttonId;
        }
    }
}
