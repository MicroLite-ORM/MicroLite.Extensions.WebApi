// -----------------------------------------------------------------------
// <copyright file="MicroLiteSessionAttribute.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// An action filter attribute which can be applied to a class or method to supply a <see cref="MicroLiteApiController"/>
    /// with a new <see cref="ISession"/> when an action is executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class MicroLiteSessionAttribute : ActionFilterAttribute
    {
        private readonly string connectionName;

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteSessionAttribute"/> class.
        /// </summary>
        public MicroLiteSessionAttribute()
            : this(null)
        {
        }

        /// <summary>s
        /// Initialises a new instance of the <see cref="MicroLiteSessionAttribute"/> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection to manage the session for.</param>
        public MicroLiteSessionAttribute(string connectionName)
        {
            this.AutoManageTransaction = true;
            this.connectionName = connectionName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to begin a transaction when OnActionExecuting is called
        /// and either commit or roll it back when OnActionExecuted is called depending on whether the ActionExecutedContext has an exception.
        /// </summary>
        public bool AutoManageTransaction
        {
            get;
            set;
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
        /// Gets or sets the isolation level to be used when a transaction is started.
        /// </summary>
        public IsolationLevel? IsolationLevel
        {
            get;
            set;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is only called the WebApi framework & the ActionExecutingContext should never be null.")]
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = (MicroLiteApiController)actionExecutedContext.ActionContext.ControllerContext.Controller;

            if (controller.Session != null)
            {
                if (this.AutoManageTransaction && controller.Session.Transaction != null)
                {
                    if (actionExecutedContext.Exception != null)
                    {
                        if (!controller.Session.Transaction.WasRolledBack)
                        {
                            controller.Session.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        if (controller.Session.Transaction.IsActive)
                        {
                            controller.Session.Transaction.Commit();
                        }
                    }
                }

                controller.Session.Dispose();
            }
        }

        /// <summary>
        /// Called by the ASP.NET WebApi framework before the action method executes.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This method is only called the WebApi framework & the ActionExecutingContext should never be null.")]
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var controller = actionContext.ControllerContext.Controller as MicroLiteApiController;

            if (controller == null)
            {
                throw new NotSupportedException(ExceptionMessages.ControllerNotMicroLiteController);
            }

            var sessionFactory = this.FindSessionFactoryForSpecifiedConnection();

            controller.Session = sessionFactory.OpenSession();

            if (this.AutoManageTransaction)
            {
                if (this.IsolationLevel.HasValue)
                {
                    controller.Session.BeginTransaction(this.IsolationLevel.Value);
                }
                else
                {
                    controller.Session.BeginTransaction();
                }
            }
        }

        private ISessionFactory FindSessionFactoryForSpecifiedConnection()
        {
            if (SessionFactories == null)
            {
                throw new MicroLiteException(ExceptionMessages.NoSessionFactoriesSet);
            }

            if (this.connectionName == null && SessionFactories.Count() > 1)
            {
                throw new MicroLiteException(ExceptionMessages.NoConnectionNameMultipleSessionFactories);
            }

            var sessionFactory =
                SessionFactories.SingleOrDefault(x => this.connectionName == null || x.ConnectionName == this.connectionName);

            if (sessionFactory == null)
            {
                throw new MicroLiteException(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.NoSessionFactoryFoundForConnectionName, this.connectionName));
            }

            return sessionFactory;
        }
    }
}