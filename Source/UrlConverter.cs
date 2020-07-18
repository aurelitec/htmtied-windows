//---------------------------------------------------------------------------- 
//
// <copyright file="URLConverter.cs" company="Aurelitec" project="HTMtied">
//    Copyright (C) 2011-2015 Aurelitec. All rights reserved. http://www.aurelitec.com/htmtied/
// </copyright> 
// 
// Description: Provides a method to convert a URL string to a HTML Link File and copy the file
// to the Clipboard.
//
//---------------------------------------------------------------------------

namespace HTMtied
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Provides a method to convert a URL string to a HTML Link File and copy the file to the Clipboard.
    /// </summary>
    public static class UrlConverter
    {
        /// <summary>
        /// Cleans-up any files left over in the program's temporary directory, from previous runs of the program.
        /// </summary>
        public static void CleanUpProgramTempDirectory()
        {
            try
            {
                string[] tempLinkFiles = Directory.GetFiles(
                    UrlConverter.GetProgramTempDirectory(),
                    Properties.Resources.StringLinkFileNameWildcard);
                foreach (string tempLinkFile in tempLinkFiles)
                {
                    File.Delete(tempLinkFile);
                }
            }
            catch
            {
                // Clean-up operation exceptions should be invisible to the user, so ignore all exceptions.
            }
        }

        /// <summary>
        /// Creates the Link.HTML file using the given url string and cuts it to the Clipboard.
        /// </summary>
        /// <param name="urls">The url strings.</param>
        public static void MakeLinkFiles(ReadOnlyCollection<NamedUrl> urls)
        {
            // Get the target folder where to save the Link HTML files
            bool isDirectMode = false;
            string targetFolder = UrlConverter.GetTargetFolder(out isDirectMode);

            List<string> filesToClipboard = new List<string>();

            foreach (NamedUrl url in urls)
            {
                // Get the pathname of the Link.HTML file that will be created and moved to the Clipboard.
                string linkHTMLFile = UrlConverter.GetLinkHTMLFileLocation(targetFolder, url);

                // Create the contents of the link.html file by inserting the actual URL.
                string linkHTMLContents =
                    Properties.Resources.StringLinkFileContents.Replace(Properties.Resources.StringInsertURLHere, url.Address);

                // Create the Link.HTML file and write its contents.
                File.WriteAllText(linkHTMLFile, linkHTMLContents);

                if (!isDirectMode)
                {
                    filesToClipboard.Add(linkHTMLFile);
                }
            }

            if (!isDirectMode)
            {
                // If not Direct Mode, move (cut) the all Link.HTML files to the Clipboard.
                UrlConverter.MoveFileToClipboard(filesToClipboard);
            }
        }

        /// <summary>
        /// Get the target folder where to save the HTML link files. If Direct Mode, the target folder is in the /DIRECT=folder
        /// parameter. If not Direct Mode, the target folder is the program's temporary directory (and make sure it is created).
        /// Normally it should be located at "C:\Users\Aurelian\AppData\Local\Temp\HTMtied".
        /// </summary>
        /// <param name="isDirectMode">True if we are in Direct Mode, false otherwise.</param>
        /// <returns>The target folder pathname.</returns>
        private static string GetTargetFolder(out bool isDirectMode)
        {
            string targetFolder = string.Empty;

            // See if the command line contains the /DIRECT argument
            string directArg = Array.Find<string>(Environment.GetCommandLineArgs(), arg => arg.StartsWith(Properties.Resources.StringDirectParameter, StringComparison.OrdinalIgnoreCase));
            isDirectMode = !string.IsNullOrEmpty(directArg);
            if (isDirectMode)
            {
                // Get the target folder part from the /DIRECT=folder argument
                string[] parts = directArg.Split(new char[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    targetFolder = parts[1];

                    // Try to create the Target folder if it does not exist
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    // Return a correct and existing target folder for the Direct Mode
                    if (Directory.Exists(targetFolder))
                    {
                        return targetFolder;
                    }
                }

                // We are in Direct Mode, but the target folder parameter is invalid, throw an exception
                throw new DirectoryNotFoundException();
            }
            else
            {
                targetFolder = UrlConverter.GetProgramTempDirectory();
            }

            return targetFolder;
        }

        /// <summary>
        /// Get the program's temporary directory (and make sure it is created). Normally it should be located
        /// at "C:\Users\Aurelian\AppData\Local\Temp\HTMtied".
        /// </summary>
        /// <returns>The pathname of the program's temporary directory.</returns>
        private static string GetProgramTempDirectory()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Application.ProductName);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            return tempPath;
        }

        /// <summary>
        /// Get the temporary HTML Link file pathname that should be created for the specified URL. Keeps trying until
        /// an non-existing file pathname is generated.
        /// </summary>
        /// <param name="targetFolder">The target folder where to save the link html file.</param>
        /// <param name="uri">An URL string.</param>
        /// <returns>The pathname of the temporary HTML Link file.</returns>
        private static string GetLinkHTMLFileLocation(string targetFolder, NamedUrl uri)
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                string indexPart = i == 0 ? string.Empty : " " + i.ToString(CultureInfo.InvariantCulture);

                string linkHTMLFile = Path.Combine(
                        targetFolder,
                        string.Format(CultureInfo.InvariantCulture, Properties.Resources.StringLinkFileName, uri.Name, indexPart));
                
                // Return only if the file does not already exist
                if (!File.Exists(linkHTMLFile))
                {
                    return linkHTMLFile;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Moves the specified file to the Windows Clipboard.
        /// </summary>
        /// <param name="linkHTMLfiles">The pathnames of the files to move.</param>
        private static void MoveFileToClipboard(List<string> linkHTMLfiles)
        {
            byte[] moveEffect = new byte[] { 2, 0, 0, 0 };
            MemoryStream dropEffect = new MemoryStream();
            dropEffect.Write(moveEffect, 0, moveEffect.Length);

            DataObject data = new DataObject();
            StringCollection filePaths = new StringCollection();
            filePaths.AddRange(linkHTMLfiles.ToArray());
            data.SetFileDropList(filePaths);
            data.SetData("Preferred DropEffect", dropEffect);

            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);
        }
    }
}
