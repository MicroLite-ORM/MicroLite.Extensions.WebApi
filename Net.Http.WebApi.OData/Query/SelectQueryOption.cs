﻿// -----------------------------------------------------------------------
// <copyright file="SelectQueryOption.cs" company="MicroLite">
// Copyright 2012-2013 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace Net.Http.WebApi.OData.Query
{
    using System.Collections.Generic;

    /// <summary>
    /// A class containing deserialised values from the $select query option.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{RawValue}")]
    public sealed class SelectQueryOption
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SelectQueryOption"/> class.
        /// </summary>
        /// <param name="rawValue">The raw request value.</param>
        public SelectQueryOption(string rawValue)
        {
            this.RawValue = rawValue;

            var pieces = rawValue.Split('=');
            var properties = pieces[1].Split(',');
            this.Properties = properties;
        }

        /// <summary>
        /// Gets the properties to be included in the query.
        /// </summary>
        public IList<string> Properties
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