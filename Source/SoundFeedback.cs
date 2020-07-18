//---------------------------------------------------------------------------- 
//
// <copyright file="SoundFeedback.cs" company="Aurelitec" project="HTMtied">
//    Copyright (C) 2011-2015 Aurelitec. All rights reserved. http://www.aurelitec.com/htmtied/
// </copyright> 
// 
// Description: Provides methods that show success or failure to the user through sounds.
//
//---------------------------------------------------------------------------

namespace HTMtied
{
    using System;
    using System.IO;
    using System.Media;

    /// <summary>
    /// Provides methods that show success or failure to the user through sounds.
    /// </summary>
    public static class SoundFeedback
    {
        /// <summary>
        /// Plays an error sound to let the user know the program failed.
        /// </summary>
        public static void PlayFailure()
        {
            SystemSounds.Hand.Play();
        }

        /// <summary>
        /// Plays the Windows system "tada.wav" sound to let the user know the operation was successful.
        /// </summary>
        public static void PlaySuccess()
        {
            try
            {
                string tadaFile = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"media\tada.wav");
                new SoundPlayer(tadaFile).PlaySync();
            }
            catch
            {
                // If an exception occurred while playing the TADA sound, play a simple beep instead
                SystemSounds.Beep.Play();
            }
        }
    }
}
