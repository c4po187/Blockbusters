#region Prerequisites

using System;
using System.Runtime.InteropServices;

#endregion

namespace BlockBusters {

    #region Macros

    /// <summary>
    /// Set of Macros detailing the available buttons
    /// outputs for the Message Box.
    /// </summary>
    public static class MessageBoxButtonOutput {
        public const uint IDOK = 1,
                          IDCANCEL = 2,
                          IDABORT = 3,
                          IDRETRY = 4,
                          IDIGNORE = 5,
                          IDYES = 6,
                          IDNO = 7,
                          IDTRYAGAIN = 10,
                          IDCONTINUE = 11;
    }

    /// <summary>
    /// Set of Macros detailing the available components
    /// for the Message Box.
    /// </summary>
    public static class MessageBoxType {
        public const uint MB_ABORTRETRYIGNORE = 0x00000002,
                          MB_CANCELTRYCONTINUE = 0x00000006,
                          MB_HELP = 0x00004000,
                          MB_OK = 0x00000000,
                          MB_OKCANCEL = 0x00000001,
                          MB_RETRYCANCEL = 0x00000005,
                          MB_YESNO = 0x00000004,
                          MB_YESNOCANCEL = 0x00000003,
                          MB_ICONEXCLAMATION = 0x00000030,
                          MB_ICONWARNING = 0x00000030,
                          MB_ICONINFORMATION = 0x00000040,
                          MB_ICONASTERISK = 0x00000040,
                          MB_ICONQUESTION = 0x00000020,
                          MB_ICONSTOP = 0x00000010,
                          MB_ICONERROR = 0x00000010,
                          MB_ICONHAND = 0x00000010,
                          MB_DEFBUTTON1 = 0x00000000,
                          MB_DEFBUTTON2 = 0x00000100,
                          MB_DEFBUTTON3 = 0x00000200,
                          MB_DEFBUTTON4 = 0x00000300,
                          MB_APPLMODAL = 0x00000000,
                          MB_SYSTEMMODAL = 0x00001000,
                          MB_TASKMODAL = 0x00002000,
                          MB_DEFAULT_DESKTOP_ONLY = 0x00020000,
                          MB_RIGHT = 0x00080000,
                          MB_RTLREADING = 0x00100000,
                          MB_SETFOREGROUND = 0x00010000,
                          MB_TOPMOST = 0x00040000,
                          MB_SERVICE_NOTIFICATION = 0x00200000;
    }

    #endregion

    #region Objects

    /// <summary>
    /// A class that provides indirect access to function calls from the
    /// Windows Application Programming Interface.
    /// </summary>
    public class _WINAPI {

        /// <summary>
        /// Displays a modal dialog box that contains a system icon, a set of buttons, 
        /// and a brief application-specific message, such as status or error information.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the owner window of the message box to be created. 
        /// If this parameter is NULL, the message box has no owner window.
        /// </param>
        /// <param name="text">
        /// The message to be displayed. If the string consists of more than one line, 
        /// you can separate the lines using a carriage return and/or linefeed character between each line.
        /// </param>
        /// <param name="caption">
        /// The dialog box title. If this parameter is NULL, the default title is Error.
        /// </param>
        /// <param name="type">
        /// The contents and behavior of the dialog box. 
        /// This parameter can be a combination of flags (OR'd).
        /// </param>
        /// <returns>
        /// An integer value that indicates which button the user clicked.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);
    }

    #endregion
}
