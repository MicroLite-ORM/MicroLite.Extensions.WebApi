// -----------------------------------------------------------------------
// <copyright file="MicroLiteApiController.cs" company="Project Contributors">
// Copyright 2012 - 2019 Project Contributors
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
    using System;
    using System.Web.Http;
    using MicroLite.Infrastructure;

    /// <summary>
    /// Provides access to a MicroLite ISession in addition to the base ASP.NET WebApi controller.
    /// </summary>
    public abstract class MicroLiteApiController : ApiController, IHaveAsyncSession
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteApiController"/> class with an ISession.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
        protected MicroLiteApiController(IAsyncSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            this.Session = session;
        }

        /// <summary>
        /// Gets or sets the <see cref="ISession"/> for the current HTTP request.
        /// </summary>
        public IAsyncSession Session
        {
            get;
            set;
        }
    }
}