﻿// -----------------------------------------------------------------------
// <copyright file="ODataValidationSettings.cs" company="MicroLite">
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
namespace Net.Http.WebApi.OData.Query.Validation
{
    /// <summary>
    /// A class which validates the values in <see cref="ODataQueryOptions"/>.
    /// </summary>
    public sealed class ODataValidationSettings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ODataValidationSettings"/> class.
        /// </summary>
        public ODataValidationSettings()
        {
            this.AllowedQueryOptions = AllowedQueryOptions.Filter | AllowedQueryOptions.Format | AllowedQueryOptions.InlineCount | AllowedQueryOptions.OrderBy | AllowedQueryOptions.Select | AllowedQueryOptions.Skip | AllowedQueryOptions.Top;
        }

        /// <summary>
        /// Gets or sets the allowed query options.
        /// </summary>
        public AllowedQueryOptions AllowedQueryOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the max value allowed in the $top query option.
        /// </summary>
        public int? MaxTop
        {
            get;
            set;
        }
    }
}