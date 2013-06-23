// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
namespace MicroLite.Configuration
{
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
        private static ILog log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET WebApi registering a MicroLiteSessionAttribute configured with default values in GlobalConfiguration.Configuration.Filters if one has not already been registered.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithWebApi(this IConfigureExtensions configureExtensions)
        {
            return WithWebApi(configureExtensions, WebApiConfigurationSettings.Default);
        }

        /// <summary>
        /// Configures the MicroLite ORM Framework extensions for ASP.NET WebApi optionally registering a MicroLiteSessionAttribute configured with default values in GlobalConfiguration.Configuration.Filters if one has not already been registered.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <param name="webApiConfig">The settings used for configuration.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithWebApi(this IConfigureExtensions configureExtensions, WebApiConfigurationSettings webApiConfig)
        {
            System.Diagnostics.Trace.TraceInformation(Messages.LoadingExtension);
            log.TryLogInfo(Messages.LoadingExtension);
            MicroLiteSessionAttribute.SessionFactories = Configure.SessionFactories;

            if (webApiConfig.RegisterGlobalMicroLiteSessionAttribute
                && !GlobalConfiguration.Configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))))
            {
                log.TryLogInfo(Messages.RegisteringDefaultMicroLiteSessionActionFilter);
                GlobalConfiguration.Configuration.Filters.Add(new MicroLiteSessionAttribute());
            }

            if (webApiConfig.RegisterGlobalValidateModelNotNullAttribute
                && !GlobalConfiguration.Configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute))))
            {
                log.TryLogInfo(Messages.RegisteringValidateModelNotNullActionFilter);
                GlobalConfiguration.Configuration.Filters.Add(new ValidateModelNotNullAttribute());
            }

            if (webApiConfig.RegisterGlobalValidateModelStateAttribute
                && !GlobalConfiguration.Configuration.Filters.Any(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))))
            {
                log.TryLogInfo(Messages.RegisteringValidateModelStateActionFilter);
                GlobalConfiguration.Configuration.Filters.Add(new ValidateModelStateAttribute());
            }

            return configureExtensions;
        }
    }
}