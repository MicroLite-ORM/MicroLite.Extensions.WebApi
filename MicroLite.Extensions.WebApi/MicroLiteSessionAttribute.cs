// -----------------------------------------------------------------------
// <copyright file="MicroLiteSessionAttribute.cs" company="MicroLite">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using MicroLite.Infrastructure;

    /// <summary>
    /// An action filter attribute which can be applied to a class or method to supply a <see cref="MicroLiteApiController"/>
    /// with a new <see cref="ISession"/> or <see cref="IReadOnlySession"/> before an action is executed.
    /// </summary>
    /// <example>
    /// Add to the global filters list for it to apply to every action on every controller unless opted out
    /// (only applies if all controllers use the same session factory):
    /// <code>
    /// static void Register(HttpConfiguration config)
    /// {
    ///     config.Filters.Add(new MicroLiteSessionAttribute("ConnectionName"));
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Add to a controller to override the connection used all actions in a controller:
    /// <code>
    /// [MicroLiteSessionAttribute("ConnectionName")]
    /// public class CustomerController : MicroLiteApiController { ... }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class MicroLiteSessionAttribute : ActionFilterAttribute
    {
        private readonly string connectionName;

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteSessionAttribute"/> class for the specified connection name.
        /// </summary>
        /// <param name="connectionName">Name of the connection to manage the session for.</param>
        public MicroLiteSessionAttribute(string connectionName)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            this.connectionName = connectionName;
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return this.connectionName;
            }
        }

        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        internal static IEnumerable<ISessionFactory> SessionFactories
        {
            get;
            set;
        }

        /// <summary>
        /// Called by the ASP.NET WebApi framework after the action method executes.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
            {
                throw new ArgumentNullException("actionExecutedContext");
            }

#if NET_4_0
            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as IHaveSession;
#else
            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as IHaveAsyncSession;
#endif

            if (controller != null)
            {
                OnActionExecuted(controller.Session);
                return;
            }

#if NET_4_0
            var readOnlyController = actionExecutedContext.ActionContext.ControllerContext.Controller as IHaveReadOnlySession;
#else
            var readOnlyController = actionExecutedContext.ActionContext.ControllerContext.Controller as IHaveAsyncReadOnlySession;
#endif

            if (readOnlyController != null)
            {
                OnActionExecuted(readOnlyController.Session);
                return;
            }
        }

        /// <summary>
        /// Called by the ASP.NET WebApi framework before the action method executes.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

#if NET_4_0
            var controller = actionContext.ControllerContext.Controller as IHaveSession;
#else
            var controller = actionContext.ControllerContext.Controller as IHaveAsyncSession;
#endif

            if (controller != null)
            {
                var sessionFactory = this.FindSessionFactoryForSpecifiedConnection();

#if NET_4_0
                controller.Session = sessionFactory.OpenSession();
#else
                controller.Session = sessionFactory.OpenAsyncSession();
#endif
                return;
            }

#if NET_4_0
            var readOnlyController = actionContext.ControllerContext.Controller as IHaveReadOnlySession;
#else
            var readOnlyController = actionContext.ControllerContext.Controller as IHaveAsyncReadOnlySession;
#endif

            if (readOnlyController != null)
            {
                var sessionFactory = this.FindSessionFactoryForSpecifiedConnection();

#if NET_4_0
                readOnlyController.Session = sessionFactory.OpenReadOnlySession();
#else
                readOnlyController.Session = sessionFactory.OpenAsyncReadOnlySession();
#endif
                return;
            }
        }

#if NET_4_0

        private static void OnActionExecuted(IReadOnlySession session)
#else

        private static void OnActionExecuted(IAsyncReadOnlySession session)
#endif
        {
            if (session != null)
            {
                session.Dispose();
            }
        }

        private ISessionFactory FindSessionFactoryForSpecifiedConnection()
        {
            if (SessionFactories == null)
            {
                throw new MicroLiteException(Messages.NoSessionFactoriesSet);
            }

            var sessionFactory =
                SessionFactories.SingleOrDefault(x => this.connectionName == null || x.ConnectionName == this.connectionName);

            if (sessionFactory == null)
            {
                throw new MicroLiteException(string.Format(CultureInfo.InvariantCulture, Messages.NoSessionFactoryFoundForConnectionName, this.connectionName));
            }

            return sessionFactory;
        }
    }
}