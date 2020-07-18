//---------------------------------------------------------------------------- 
//
// <copyright file="Program.cs" company="Aurelitec" project="HTMtied">
//    Copyright (C) 2011-2015 Aurelitec. All rights reserved. http://www.aurelitec.com/htmtied/
// </copyright> 
// 
// Description: The main program class.
//
//---------------------------------------------------------------------------

namespace HTMtied
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the main program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // First of all, get the list of urls (as text or URL files) from the command line or Clipboard
            ReadOnlyCollection<NamedUrl> urls = UrlGetter.GetUrls();
            if ((urls != null) && (urls.Count > 0))
            {
                // Clean-up any files left over in the program's temporary directory, from previous runs
                // of the program.
                UrlConverter.CleanUpProgramTempDirectory();
                try
                {
                    UrlConverter.MakeLinkFiles(urls);
                    SoundFeedback.PlaySuccess();
                    return;
                }
                catch (Exception)
                {
                    SoundFeedback.PlayFailure();
                    ////System.Windows.Forms.MessageBox.Show(Ex.Message);
                }
            }

            SoundFeedback.PlayFailure();
        }
    }
}
