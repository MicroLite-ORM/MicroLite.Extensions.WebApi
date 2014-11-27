// -----------------------------------------------------------------------
// <copyright file="MicroLiteApiController.cs" company="MicroLite">
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
    /// Provides access to a MicroLite ISession in addition to the base ASP.NET WebApi controller.
    /// </summary>
    public abstract class MicroLiteApiController : ApiController,
#if NET_4_0
 IHaveSession
#else
 IHaveAsyncSession
#endif
    {
#if NET_4_0
        private ISession session;
#else
        private IAsyncSession session;
#endif

        /// <summary>
        /// Initialises a new instance of the MicroLiteApiController class.
        /// </summary>
        protected MicroLiteApiController()
            : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the MicroLiteApiController class with an ISession.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
#if NET_4_0

        protected MicroLiteApiController(ISession session)
#else

        protected MicroLiteApiController(IAsyncSession session)
#endif
        {
            this.session = session;
        }

        /// <summary>
        /// Gets or sets the <see cref="ISession"/> for the current HTTP request.
        /// </summary>
#if NET_4_0

        public ISession Session
#else

        public IAsyncSession Session
#endif
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