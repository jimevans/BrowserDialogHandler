BrowserDialogHandler
====================

This project is for a prototype of a cross-platform dialog handling library for browsers
In theory, it would allow a user to set up handlers for all of the common types of browser-
spawned dialogs, including login dialogs, JavaScript dialogs, and file selection dialogs.
It would be ideal for use with WebDriver, but it requires the user to know the handle to
the main browser window, which WebDriver does not expose.

This is only a proof of concept, not a finished project. Furthermore, it is not likely to
receive many updates. It is provided here, as source code, so that someone more ambitious
may fork and complete.

In the current code base, the way to use this would be the following:

    // The current impelementation of CreateDialogWatcher is incredibly
    // naive, and would need rewriting to take criteria for which instance
    // of the browser to watch for alerts from.
    IWatcher watcher = WatcherFactory.CreateDialogWatcher(BrowserType.InternetExplorer);
    
    // Set a handler to handle all alert dialogs, clicking the OK button.
    watcher.SetHandler<AlertDialog>((alert) => { alert.ClickOkButton(); });
    
    // Set an expectation that a confirm dialog will appear within 30 seconds.
    Expectation<ConfirmDialog> expect = watcher.Expect<ConfirmDialog>(TimeSpan.FromSeconds(30));

    // Wait for the dialog to appear, and click the cancel button.
    expect.WaitUntilSatisfied();
    if (expect.IsSatisfied)
    {
        expect.Object.ClickCancelButton();
    }
