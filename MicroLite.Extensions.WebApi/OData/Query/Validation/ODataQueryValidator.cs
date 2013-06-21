// -----------------------------------------------------------------------
// <copyright file="ODataQueryValidator.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.OData.Query.Validation
{
    /// <summary>
    /// A class which validates ODataQueryOptions based upon the ODataValidationSettings.
    /// </summary>
    public class ODataQueryValidator
    {
        /// <summary>
        /// Validates the specified query options.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="validationSettings">The validation settings.</param>
        /// <exception cref="ODataException">Thrown if the validation fails.</exception>
        public void Validate(ODataQueryOptions queryOptions, ODataValidationSettings validationSettings)
        {
            if (queryOptions.Format != null && (validationSettings.AllowedQueryOptions & AllowedQueryOptions.Format) != AllowedQueryOptions.Format)
            {
                throw new ODataException();
            }
        }
    }
}