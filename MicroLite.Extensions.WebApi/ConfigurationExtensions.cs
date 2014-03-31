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
        /// <param name="configuration">The HttpConfiguration (GlobalConfiguration.Configuration if using WebHost, or the HttpConfiguration created by SelfHost).</param>
        /// <param name="settings">The settings used for configuration.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithWebApi(
            this IConfigureExtensions configureExtensions,
            HttpConfiguration configuration,
            WebApiConfigurationSettings settings)
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
            if (Log.IsInfo)
            {
                Log.Info(Messages.LoadingExtension);
            }

            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            if (settings.RegisterGlobalValidateModelNotNullAttribute
                && !configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute))))
            {
                if (Log.IsInfo)
                {
                    Log.Info(Messages.RegisteringValidateModelNotNullActionFilter);
                }

                configuration.Filters.Add(new ValidateModelNotNullAttribute());
            }

            if (settings.RegisterGlobalValidateModelStateAttribute
                && !configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))))
            {
                if (Log.IsInfo)
                {
                    Log.Info(Messages.RegisteringValidateModelStateActionFilter);
                }

                configuration.Filters.Add(new ValidateModelStateAttribute());
            }

            if (settings.RegisterGlobalMicroLiteSessionAttribute
                && !configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))))
            {
                if (Log.IsInfo)
                {
                    Log.Info(Messages.RegisteringDefaultMicroLiteSessionActionFilter);
                }

                configuration.Filters.Add(new MicroLiteSessionAttribute());
            }

            return configureExtensions;
        }
    }
}