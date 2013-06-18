// -----------------------------------------------------------------------
// <copyright file="TopQueryValidator.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.Query.Validators
{
    using System.Globalization;
    using MicroLite.Logging;

    /// <summary>
    /// A class which validates the $top query option based upon the ODataValidationSettings.
    /// </summary>
    public class TopQueryValidator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Validates the specified query options.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="validationSettings">The validation settings.</param>
        /// <exception cref="ODataException">Thrown if the validation fails.</exception>
        public void Validate(ODataQueryOptions queryOptions, ODataValidationSettings validationSettings)
        {
            if (queryOptions.Top != null && validationSettings.MaxTop.HasValue)
            {
                if (queryOptions.Top.Value > validationSettings.MaxTop.Value)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, Messages.TopValueExceedsMaxAllowed, validationSettings.MaxTop.Value.ToString(CultureInfo.InvariantCulture));
                    Log.TryLogError(message);
                    throw new ODataException(message);
                }
            }
        }
    }
}