// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Configuration
{
    using System;
    using MicroLite.Extensions.WebApi;
    using MicroLite.Logging;

    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET WebApi.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <exception cref="ArgumentNullException">Thrown if configureExtensions is null.</exception>
        /// <returns>The configure extensions.</returns>
        /// <example>
        /// <code>
        /// Configure
        ///     .Extensions()
        ///     .WithWebApi();
        /// </code>
        /// </example>
        public static IConfigureExtensions WithWebApi(
            this IConfigureExtensions configureExtensions)
        {
            if (configureExtensions == null)
            {
                throw new ArgumentNullException("configureExtensions");
            }

            System.Diagnostics.Trace.TraceInformation(Messages.LoadingExtension);

            if (Log.IsInfo)
            {
                Log.Info(Messages.LoadingExtension);
            }

            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            return configureExtensions;
        }
    }
}