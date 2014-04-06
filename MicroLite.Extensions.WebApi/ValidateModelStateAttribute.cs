// -----------------------------------------------------------------------
// <copyright file="ValidateModelStateAttribute.cs" company="MicroLite">
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
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// An <see cref="ActionFilterAttribute"/> which verifies the model state is valid.
    /// </summary>
    /// <example>
    /// Add to the global filters list for it to apply to every action on every controller unless opted out:
    /// <code>
    /// static void Register(HttpConfiguration config)
    /// {
    ///     config.Filters.Add(new ValidateModelStateAttribute());
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Add to a controller to opt out all actions in a controller:
    /// <code>
    /// [ValidateModelStateAttribute(SkipValidation = true)]
    /// public class CustomerController : MicroLiteApiController { ... }
    /// </code>
    /// </example>
    /// <example>
    /// Add to an individual action to opt out that particular action:
    /// <code>
    /// [ValidateModelStateAttribute(SkipValidation = true)]
    /// public HttpResponseMessage Put(int id, Model model)
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ValidateModelStateAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether to skip validation (false by default).
        /// </summary>
        /// <remarks>
        /// Allows overriding the default behaviour on an individual action/controller if an instance
        /// is already registered in the global filters.
        /// </remarks>
        public bool SkipValidation
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (this.SkipValidation)
            {
                return;
            }

            if (actionContext != null && !actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest,
                    actionContext.ModelState);
            }
        }
    }
}