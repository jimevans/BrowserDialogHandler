// <copyright file="AccessibleRole.cs" company="BrowserDialogHandler Project">
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
    /// Values representing the role an accessible object contains.
    /// </summary>
    public enum AccessibleRole
    {
        /// <summary>
        /// Invalid role
        /// </summary>
        Invalid,

        /// <summary>
        /// Object's role is an accelerator label
        /// </summary>
        AcceleratorLabel,

        /// <summary>
        /// Object's role is an alert
        /// </summary>
        Alert,

        /// <summary>
        /// Object's role is an animation
        /// </summary>
        Animation,

        /// <summary>
        /// Object's role is an arrow
        /// </summary>
        Arrow,

        /// <summary>
        /// Object's role is a calendar
        /// </summary>
        Calendar,

        /// <summary>
        /// Object's role is a canvas
        /// </summary>
        Canvas,

        /// <summary>
        /// Object's role is a check box
        /// </summary>
        CheckBox,

        /// <summary>
        /// Object's role is a check menu item
        /// </summary>
        CheckMenuItem,

        /// <summary>
        /// Object's role is a color picker
        /// </summary>
        ColorChooser,

        /// <summary>
        /// Object's role is a column header
        /// </summary>
        ColumnHeader,

        /// <summary>
        /// Object's role is a combo box
        /// </summary>
        ComboBox,

        /// <summary>
        /// Object's role is a data editor
        /// </summary>
        DateEditor,

        /// <summary>
        /// Object's role is a desktop icon
        /// </summary>
        DesktopIcon,

        /// <summary>
        /// Object's role is a desktop frame
        /// </summary>
        DesktopFrame,

        /// <summary>
        /// Object's role is a dial
        /// </summary>
        Dial,

        /// <summary>
        /// Object's role is a dialog
        /// </summary>
        Dialog,

        /// <summary>
        /// Object's role is a directory pane
        /// </summary>
        DirectoryPane,

        /// <summary>
        /// Object's role is a drawing area
        /// </summary>
        DrawingArea,

        /// <summary>
        /// Object's role is a file picker
        /// </summary>
        FileChooser,

        /// <summary>
        /// Object's role is a filler
        /// </summary>
        Filler,

        /// <summary>
        /// Object's role is a font picker
        /// </summary>
        FontChooser,

        /// <summary>
        /// Object's role is a frame
        /// </summary>
        Frame,

        /// <summary>
        /// Object's role is a glass pane
        /// </summary>
        GlassPane,

        /// <summary>
        /// Object's role is a HTML container
        /// </summary>
        HtmlContainer,

        /// <summary>
        /// Object's role is an icon
        /// </summary>
        Icon,

        /// <summary>
        /// Object's role is an image
        /// </summary>
        Image,

        /// <summary>
        /// Object's role is an internal frame
        /// </summary>
        InternalFrame,

        /// <summary>
        /// Object's role is a label
        /// </summary>
        Label,

        /// <summary>
        /// Object's role is a layered pane
        /// </summary>
        LayeredPane,

        /// <summary>
        /// Object's role is a list
        /// </summary>
        List,

        /// <summary>
        /// Object's role is a list item
        /// </summary>
        ListItem,

        /// <summary>
        /// Object's role is a menu
        /// </summary>
        Menu,

        /// <summary>
        /// Object's role is a menu bar
        /// </summary>
        MenuBar,

        /// <summary>
        /// Object's role is a menu item
        /// </summary>
        MenuItem,

        /// <summary>
        /// Object's role is a option pane
        /// </summary>
        OptionPane,

        /// <summary>
        /// Object's role is a page tab
        /// </summary>
        PageTab,

        /// <summary>
        /// Object's role is a page tab list
        /// </summary>
        PageTabList,

        /// <summary>
        /// Object's role is a panel
        /// </summary>
        Panel,

        /// <summary>
        /// Object's role is a password text control
        /// </summary>
        PasswordText,

        /// <summary>
        /// Object's role is a popup menu
        /// </summary>
        PopupMenu,

        /// <summary>
        /// Object's role is a progress bar
        /// </summary>
        ProgressBar,

        /// <summary>
        /// Object's role is a push button
        /// </summary>
        PushButton,

        /// <summary>
        /// Object's role is a radio button
        /// </summary>
        RadioButton,

        /// <summary>
        /// Object's role is a radio menu item
        /// </summary>
        RadioMenuItem,

        /// <summary>
        /// Object's role is a root pane
        /// </summary>
        RootPane,

        /// <summary>
        /// Object's role is a row header
        /// </summary>
        RowHeader,

        /// <summary>
        /// Object's role is a scroll bar
        /// </summary>
        ScrollBar,

        /// <summary>
        /// Object's role is a scroll pane
        /// </summary>
        ScrollPane,

        /// <summary>
        /// Object's role is a separator
        /// </summary>
        Separator,

        /// <summary>
        /// Object's role is a slider
        /// </summary>
        Slider,

        /// <summary>
        /// Object's role is a spin button
        /// </summary>
        SpinButton,

        /// <summary>
        /// Object's role is a split pane
        /// </summary>
        SplitPane,

        /// <summary>
        /// Object's role is a status bar
        /// </summary>
        StatusBar,

        /// <summary>
        /// Object's role is a table
        /// </summary>
        Table,

        /// <summary>
        /// Object's role is a table cell
        /// </summary>
        TableCell,

        /// <summary>
        /// Object's role is a table column header
        /// </summary>
        TableColumnHeader,

        /// <summary>
        /// Object's role is a table row header
        /// </summary>
        TableRowHeader,

        /// <summary>
        /// Object's role is a tear-off menu item
        /// </summary>
        TearoffMenuItem,

        /// <summary>
        /// Object's role is a terminal window
        /// </summary>
        Terminal,

        /// <summary>
        /// Object's role is a text item
        /// </summary>
        Text,

        /// <summary>
        /// Object's role is a toggle button
        /// </summary>
        ToggleButton,

        /// <summary>
        /// Object's role is a tool bar
        /// </summary>
        Toolbar,

        /// <summary>
        /// Object's role is a tool tip
        /// </summary>
        ToolTip,

        /// <summary>
        /// Object's role is a tree
        /// </summary>
        Tree,

        /// <summary>
        /// Object's role is a tree table
        /// </summary>
        TreeTable,

        /// <summary>
        /// Object's role is unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Object's role is a view port
        /// </summary>
        ViewPort,

        /// <summary>
        /// Object's role is a window
        /// </summary>
        Window,

        /// <summary>
        /// Object's role is an extended value
        /// </summary>
        Extended,

        /// <summary>
        /// Object's role is a header
        /// </summary>
        Header,

        /// <summary>
        /// Object's role is a footer
        /// </summary>
        Footer,

        /// <summary>
        /// Object's role is a paragraph
        /// </summary>
        Paragraph,

        /// <summary>
        /// Object's role is a ruler
        /// </summary>
        Ruler,

        /// <summary>
        /// Object's role is an application
        /// </summary>
        Application,

        /// <summary>
        /// Object's role is an AutoComplete entry
        /// </summary>
        AutoComplete,

        /// <summary>
        /// Object's role is an edit bar
        /// </summary>
        EditBar,

        /// <summary>
        /// Object's role is embedded
        /// </summary>
        Embedded,

        /// <summary>
        /// Object's role is an entry
        /// </summary>
        Entry,

        /// <summary>
        /// Object's role is a chart
        /// </summary>
        Chart,

        /// <summary>
        /// Object's role is a caption
        /// </summary>
        Caption,

        /// <summary>
        /// Object's role is a document frame
        /// </summary>
        DocumentFrame,

        /// <summary>
        /// Object's role is a page tab list
        /// </summary>
        Heading,

        /// <summary>
        /// Object's role is a page tab list
        /// </summary>
        Page,

        /// <summary>
        /// Object's role is a page tab list
        /// </summary>
        Section,

        /// <summary>
        /// Object's role is a form
        /// </summary>
        Form,

        /// <summary>
        /// Object's role is a title bar
        /// </summary>
        TitleBar = Form,

        /// <summary>
        /// Object's role is a redundant object
        /// </summary>
        RedundantObject,

        /// <summary>
        /// Object's role is a link
        /// </summary>
        Link,

        /// <summary>
        /// Object's role is an input method window
        /// </summary>
        InputMethodWindow,

        /// <summary>
        /// This is the last defined role
        /// </summary>
        LastDefined = InputMethodWindow,

        /// <summary>
        /// Object has any role
        /// </summary>
        AnyRole
    }
}
