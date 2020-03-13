// -----------------------------------------------------------------------
// <copyright file="MicroLiteApiController.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Web.Http;
using MicroLite.Infrastructure;

namespace MicroLite.Extensions.WebApi
{
    /// <summary>
    /// Provides access to a MicroLite ISession in addition to the base ASP.NET WebApi controller.
    /// </summary>
    public abstract class MicroLiteApiController : ApiController, IHaveSession
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteApiController"/> class with an ISession.
        /// </summary>
        /// <param name="session">The <see cref="ISession"/> for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an <see cref="ISession"/> via an IOC container.
        /// </remarks>
        protected MicroLiteApiController(ISession session)
            => Session = session ?? throw new ArgumentNullException(nameof(session));

        /// <summary>
        /// Gets the <see cref="ISession"/> for the current HTTP request.
        /// </summary>
        public ISession Session { get; }
    }
}
