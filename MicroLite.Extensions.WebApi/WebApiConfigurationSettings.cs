// -----------------------------------------------------------------------
// <copyright file="WebApiConfigurationSettings.cs" company="MicroLite">
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
    /// <summary>
    /// A class containing configuration options for the MicroLite WebApi extension.
    /// </summary>
    public sealed class WebApiConfigurationSettings
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WebApiConfigurationSettings"/> class.
        /// </summary>
        public WebApiConfigurationSettings()
        {
            this.RegisterGlobalMicroLiteSessionAttribute = true;
            this.RegisterGlobalValidateModelNotNullAttribute = true;
            this.RegisterGlobalValidateModelStateAttribute = true;
        }

        /// <summary>
        /// Gets an instance of the settings with the default options set.
        /// </summary>
        public static WebApiConfigurationSettings Default
        {
            get
            {
                return new WebApiConfigurationSettings();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to register a MicroLiteSessionAttribute in the
        /// HttpConfiguration.Filters is one is not already registered (defaults to true).
        /// </summary>
        public bool RegisterGlobalMicroLiteSessionAttribute
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to register a ValidateModelNotNullAttribute in the
        /// HttpConfiguration.Filters is one is not already registered (defaults to true).
        /// </summary>
        public bool RegisterGlobalValidateModelNotNullAttribute
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to register a ValidateModelStateAttribute in the
        /// HttpConfiguration.Filters is one is not already registered (defaults to true).
        /// </summary>
        public bool RegisterGlobalValidateModelStateAttribute
        {
            get;
            set;
        }
    }
}