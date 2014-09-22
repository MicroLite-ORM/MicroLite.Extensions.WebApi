// -----------------------------------------------------------------------
// <copyright file="MicroLiteReadOnlyApiController.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi
{
    using System.Web.Http;
    using MicroLite.Infrastructure;

    /// <summary>
    /// Provides access to a MicroLite IReadOnlySession in addition to the base ASP.NET WebApi controller.
    /// </summary>
    public abstract class MicroLiteReadOnlyApiController : ApiController, IHaveReadOnlySession
    {
        private IReadOnlySession session;

        /// <summary>
        /// Initialises a new instance of the MicroLiteReadOnlyApiController class.
        /// </summary>
        protected MicroLiteReadOnlyApiController()
            : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the MicroLiteReadOnlyApiController class with an IReadOnlySession.
        /// </summary>
        /// <param name="session">The IReadOnlySession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an IReadOnlySession via an IOC container.
        /// </remarks>
        protected MicroLiteReadOnlyApiController(IReadOnlySession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets or sets the <see cref="IReadOnlySession"/> for the current HTTP request.
        /// </summary>
        public IReadOnlySession Session
        {
            get
            {
                return this.session;
            }

            set
            {
                this.session = value;
            }
        }
    }
}
