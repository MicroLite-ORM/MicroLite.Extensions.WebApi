﻿// -----------------------------------------------------------------------
// <copyright file="FormatQueryOption.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Extensions.WebApi.Query
{
    using System.Net.Http.Headers;

    /// <summary>
    /// A class containing deserialised values from the $format query option.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{RawValue}")]
    public sealed class FormatQueryOption
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FormatQueryOption"/> class.
        /// </summary>
        /// <param name="rawValue">The raw request value.</param>
        /// <exception cref="ODataException">Thrown if the raw value is invalid.</exception>
        public FormatQueryOption(string rawValue)
        {
            this.RawValue = rawValue;

            var pieces = rawValue.Split('=');

            switch (pieces[1].ToUpperInvariant())
            {
                case "ATOM":
                    this.MediaTypeHeaderValue = new MediaTypeHeaderValue("application/atom+xml");
                    break;

                case "JSON":
                    this.MediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");
                    break;

                case "XML":
                    this.MediaTypeHeaderValue = new MediaTypeHeaderValue("application/xml");
                    break;

                default:
                    this.MediaTypeHeaderValue = new MediaTypeHeaderValue(pieces[1]);
                    break;
            }
        }

        /// <summary>
        /// Gets the media type header value.
        /// </summary>
        public MediaTypeHeaderValue MediaTypeHeaderValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw request value.
        /// </summary>
        public string RawValue
        {
            get;
            private set;
        }
    }
}