//---------------------------------------------------------------------------- 
//
// <copyright file="NamedUrl.cs" company="Aurelitec" project="HTMtied">
//    Copyright (C) 2011-2015 Aurelitec. All rights reserved. http://www.aurelitec.com/htmtied/
// </copyright> 
// 
// Description: Represents a Uri that also has a name.
//
//---------------------------------------------------------------------------

namespace HTMtied
{
    using System;

    /// <summary>
    /// Represents a Url that has an address and a name.
    /// </summary>
    public class NamedUrl
    {
        /// <summary>
        /// Initializes a new instance of the NamedUrl class.
        /// </summary>
        /// <param name="address">The address string.</param>
        /// <param name="name">The name string.</param>
        public NamedUrl(string address, string name)
        {
            this.Address = address;
            this.Name = name;

            // If the name if null or empty, set it to the Host part of the Url address
            if (string.IsNullOrEmpty(this.Name))
            {
                if (Uri.IsWellFormedUriString(address, UriKind.Absolute))
                {
                    Uri uri = new Uri(address);
                    this.Name = uri.Host;
                }
            }
        }

        /// <summary>
        /// Gets the Uri part.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Gets the Name part.
        /// </summary>
        public string Name { get; private set; }
    }
}
