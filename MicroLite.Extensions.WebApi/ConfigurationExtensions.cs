// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using System.Linq;
    using System.Web.Http;
    using MicroLite.Extensions.WebApi;
    using MicroLite.Extensions.WebApi.Filters;
    using MicroLite.Logging;

    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET WebApi using the specified configuration settings.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <param name="settings">The settings used for configuration.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithWebApi(this IConfigureExtensions configureExtensions, WebApiConfigurationSettings settings)
        {
            if (configureExtensions == null)
            {
                throw new ArgumentNullException("configureExtensions");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            System.Diagnostics.Trace.TraceInformation(Messages.LoadingExtension);
            Log.TryLogInfo(Messages.LoadingExtension);
            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            if (settings.RegisterGlobalMicroLiteSessionAttribute
                && !GlobalConfiguration.Configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))))
            {
                Log.TryLogInfo(Messages.RegisteringDefaultMicroLiteSessionActionFilter);
                GlobalConfiguration.Configuration.Filters.Add(new MicroLiteSessionAttribute());
            }

            if (settings.RegisterGlobalValidateModelNotNullAttribute
                && !GlobalConfiguration.Configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute))))
            {
                Log.TryLogInfo(Messages.RegisteringValidateModelNotNullActionFilter);
                GlobalConfiguration.Configuration.Filters.Add(new ValidateModelNotNullAttribute());
            }

            if (settings.RegisterGlobalValidateModelStateAttribute
                && !GlobalConfiguration.Configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))))
            {
                Log.TryLogInfo(Messages.RegisteringValidateModelStateActionFilter);
                GlobalConfiguration.Configuration.Filters.Add(new ValidateModelStateAttribute());
            }

            return configureExtensions;
        }
    }
}