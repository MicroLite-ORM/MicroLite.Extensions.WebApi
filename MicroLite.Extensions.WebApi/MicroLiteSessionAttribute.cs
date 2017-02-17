// -----------------------------------------------------------------------
// <copyright file="MicroLiteSessionAttribute.cs" company="MicroLite">
// Copyright 2012 - 2017 Project Contributors
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
        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteSessionAttribute"/> class for the specified connection name.
        /// </summary>
        /// <param name="connectionName">Name of the connection to manage the session for.</param>
        public MicroLiteSessionAttribute(string connectionName)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            this.ConnectionName = connectionName;
        }

        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        public string ConnectionName
        {
            get;
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
                throw new ArgumentNullException(nameof(actionExecutedContext));
            }

            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as IHaveAsyncSession;

            if (controller != null)
            {
                OnActionExecuted(controller.Session);
                return;
            }

            var readOnlyController = actionExecutedContext.ActionContext.ControllerContext.Controller as IHaveAsyncReadOnlySession;

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
                throw new ArgumentNullException(nameof(actionContext));
            }

            var controller = actionContext.ControllerContext.Controller as IHaveAsyncSession;

            if (controller != null)
            {
                var sessionFactory = this.FindSessionFactoryForSpecifiedConnection();

                controller.Session = sessionFactory.OpenAsyncSession();

                return;
            }

            var readOnlyController = actionContext.ControllerContext.Controller as IHaveAsyncReadOnlySession;

            if (readOnlyController != null)
            {
                var sessionFactory = this.FindSessionFactoryForSpecifiedConnection();

                readOnlyController.Session = sessionFactory.OpenAsyncReadOnlySession();

                return;
            }
        }

        private static void OnActionExecuted(IAsyncReadOnlySession session)
        {
            session?.Dispose();
        }

        private ISessionFactory FindSessionFactoryForSpecifiedConnection()
        {
            if (SessionFactories == null)
            {
                throw new MicroLiteException(Messages.NoSessionFactoriesSet);
            }

            var sessionFactory =
                SessionFactories.SingleOrDefault(x => this.ConnectionName == null || x.ConnectionName == this.ConnectionName);

            if (sessionFactory == null)
            {
                throw new MicroLiteException(string.Format(CultureInfo.InvariantCulture, Messages.NoSessionFactoryFoundForConnectionName, this.ConnectionName));
            }

            return sessionFactory;
        }
    }
}