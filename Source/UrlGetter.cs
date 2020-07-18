//---------------------------------------------------------------------------- 
//
// <copyright file="URLGetter.cs" company="Aurelitec" project="HTMtied">
//    Copyright (C) 2011-2015 Aurelitec. All rights reserved. http://www.aurelitec.com/htmtied/
// </copyright> 
// 
// Description: Provides a method to extract the URL send to the program as text or URL file, 
// through the command line or the Clipboard.
//
//---------------------------------------------------------------------------

namespace HTMtied
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Provides a method to extract the URL sent to the program as text or URL file, through the command line or
    /// the Clipboard.
    /// </summary>
    public static class UrlGetter
    {
        /// <summary>
        /// Tries to extract the URL send to the program as text or URL file, through the command line or
        /// the Clipboard.
        /// </summary>
        /// <returns>The URL string, or an empty string if no valid URL in sent to the program.</returns>
        public static ReadOnlyCollection<NamedUrl> GetUrls()
        {
            List<NamedUrl> urls = new List<NamedUrl>();

            // First try: if we have at least one command line argument, maybe they are URL or WEBSITE files.
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length >= 2)
            {
                for (int i = 1; i < commandLineArgs.Length; i++)
                {
                    string argument = commandLineArgs[i];
                    if (!argument[0].Equals('/'))
                    {
                        NamedUrl url = UrlGetter.GetUrlFromUrlFile(argument);
                        if (url != null)
                        {
                            urls.Add(url);
                        }
                    }
                }

                if (urls.Count > 0)
                {
                    return urls.AsReadOnly();
                }
            }

            // Second try: determine whether the Clipboard contains one or more URL as text line(s).
            if (Clipboard.ContainsText())
            {
                // Retrieve the text data from the Clipboard in Text or UnicodeText format.
                string clipText = Clipboard.GetText();

                // Split in lines
                string[] potentialUrls = clipText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string potentialUrl in potentialUrls)
                {
                    // Add the named url only if it is valid
                    NamedUrl url = new NamedUrl(potentialUrl, string.Empty);
                    if (!string.IsNullOrEmpty(url.Name))
                    {
                        urls.Add(url);
                    }
                }

                return urls.AsReadOnly();
            }

            // Third try: determine whether the Clipboard contains one or more URLs as a URL or WEBSITE files
            if (Clipboard.ContainsFileDropList())
            {
                // Retrieve the file names from the Clipboard.
                StringCollection files = Clipboard.GetFileDropList();

                if ((files != null) && (files.Count > 0))
                {
                    foreach (string file in files)
                    {
                        urls.Add(UrlGetter.GetUrlFromUrlFile(file));
                    }

                    return urls.AsReadOnly();
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts the URL part from a .URL or .WEBSITE file.
        /// </summary>
        /// <param name="urlFile">The .URL or .WEBSITE file name.</param>
        /// <returns>The URL as string, or an empty string if the file is not valid.</returns>
        private static NamedUrl GetUrlFromUrlFile(string urlFile)
        {
            if (File.Exists(urlFile) &&
                (Path.GetExtension(urlFile).Equals(".URL", StringComparison.OrdinalIgnoreCase) ||
                 Path.GetExtension(urlFile).Equals(".WEBSITE", StringComparison.OrdinalIgnoreCase)))
            {
                StringBuilder urlBuilder = new StringBuilder(1000);
                if (NativeMethods.GetPrivateProfileString(
                    "InternetShortcut",
                    "URL",
                    string.Empty,
                    urlBuilder,
                    urlBuilder.Capacity,
                    urlFile) > 0)
                {
                    return new NamedUrl(urlBuilder.ToString(), Path.GetFileNameWithoutExtension(urlFile));
                }
            }

            return null;
        }

        /// <summary>
        /// Contains required Native Windows API method definitions.
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern uint GetPrivateProfileString(
               string lpAppName,
               string lpKeyName,
               string lpDefault,
               StringBuilder lpReturnedString,
               int nSize,
               string lpFileName);
        }
    }
}
