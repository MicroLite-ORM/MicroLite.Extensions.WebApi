﻿// -----------------------------------------------------------------------
// <copyright file="AutoManageTransactionAttribute.cs" company="Project Contributors">
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
using System.Data;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using MicroLite.Infrastructure;

namespace MicroLite.Extensions.WebApi
{
    /// <summary>
    /// An action filter attribute which can be applied to a class or method to automatically begin an <see cref="ITransaction"/>
    /// before an action is executed and either commit or roll it back after the action is executed depending on whether an exception occurred.
    /// </summary>
    /// <example>
    /// Add to the global filters list for it to apply to every action on every controller unless opted out:
    /// <code>
    /// static void Register(HttpConfiguration config)
    /// {
    ///     config.Filters.Add(new AutoManageTransactionAttribute());
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Add to a controller to opt out all actions in a controller:
    /// <code>
    /// [AutoManageTransactionAttribute(AutoManageTransaction = false)]
    /// public class CustomerController : MicroLiteApiController { ... }
    /// </code>
    /// </example>
    /// <example>
    /// Add to an individual action to opt out that particular action:
    /// <code>
    /// [AutoManageTransactionAttribute(AutoManageTransaction = false)]
    /// public HttpResponseMessage Put(int id, Model model)
    /// </code>
    /// </example>
    /// <example>
    /// Override the IsolationLevel of the transaction for a specific method (could also be done at controller level).
    /// <code>
    /// [AutoManageTransactionAttribute(IsolationLevel = IsolationLevel.Chaos)]
    /// public HttpResponseMessage Put(int id, Model model)
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AutoManageTransactionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AutoManageTransactionAttribute"/> class using <see cref="IsolationLevel"/>.ReadCommitted.
        /// </summary>
        public AutoManageTransactionAttribute()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to begin a transaction when OnActionExecuting is called
        /// and either commit or roll it back when OnActionExecuted is called depending on whether the ActionExecutedContext has an exception.
        /// </summary>
        /// <remarks>
        /// Allows an individual controller or action to opt-out if an instance of the attribute is registered in the global filters collection.
        /// </remarks>
        public bool AutoManageTransaction { get; set; } = true;

        /// <summary>
        /// Gets or sets the isolation level to be used when a transaction is started.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Called by the ASP.NET WebApi framework after the action method executes.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!AutoManageTransaction)
            {
                return;
            }

            if (actionExecutedContext is null)
            {
                throw new ArgumentNullException(nameof(actionExecutedContext));
            }

            if (actionExecutedContext.ActionContext.ControllerContext.Controller is IHaveSession controller)
            {
                OnActionExecuted(controller.Session, actionExecutedContext.Exception);
                return;
            }

            if (actionExecutedContext.ActionContext.ControllerContext.Controller is IHaveReadOnlySession readOnlyController)
            {
                OnActionExecuted(readOnlyController.Session, actionExecutedContext.Exception);
            }
        }

        /// <summary>
        /// Called by the ASP.NET WebApi framework before the action method executes.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!AutoManageTransaction)
            {
                return;
            }

            if (actionContext is null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (actionContext.ControllerContext.Controller is IHaveSession controller)
            {
                controller.Session.BeginTransaction(IsolationLevel);
                return;
            }

            if (actionContext.ControllerContext.Controller is IHaveReadOnlySession readOnlyController)
            {
                readOnlyController.Session.BeginTransaction(IsolationLevel);
            }
        }

        private static void OnActionExecuted(IReadOnlySession session, Exception exception)
        {
            if (session.CurrentTransaction is null)
            {
                return;
            }

            ITransaction transaction = session.CurrentTransaction;

            try
            {
                if (transaction.IsActive && exception is null)
                {
                    transaction.Commit();
                }
                else if (transaction.IsActive && exception != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
