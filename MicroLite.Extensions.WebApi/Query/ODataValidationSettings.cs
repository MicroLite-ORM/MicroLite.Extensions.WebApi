// -----------------------------------------------------------------------
// <copyright file="ODataValidationSettings.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.Query.Validation
{
    /// <summary>
    /// A class which validates the values in <see cref="ODataQueryOptions"/>.
    /// </summary>
    public sealed class ODataValidationSettings
    {
        /// <summary>
        /// Gets or sets the max value allowed in the $top query option.
        /// </summary>
        public int? MaxTop
        {
            get;
            set;
        }

        /// <summary>
        /// Validates the specified query options.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        public void Validate(ODataQueryOptions queryOptions)
        {
            if (this.MaxTop.HasValue && queryOptions.Top != null)
            {
                if (queryOptions.Top.Value > this.MaxTop.Value)
                {
                    throw new ODataException();
                }
            }
        }
    }
}