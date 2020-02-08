// -----------------------------------------------------------------------
// <copyright file="ValidateModelNotNullAttribute.cs" company="Project Contributors">
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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MicroLite.Extensions.WebApi
{
    /// <summary>
    /// An <see cref="ActionFilterAttribute"/> which verifies the parameters passed to the controller action are not null.
    /// </summary>
    /// <example>
    /// Add to the global filters list for it to apply to every action on every controller unless opted out:
    /// <code>
    /// static void Register(HttpConfiguration config)
    /// {
    ///     config.Filters.Add(new ValidateModelNotNullAttribute());
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Add to a controller to opt out all actions in a controller:
    /// <code>
    /// [ValidateModelNotNullAttribute(SkipValidation = true)]
    /// public class CustomerController : MicroLiteApiController { ... }
    /// </code>
    /// </example>
    /// <example>
    /// Add to an individual action to opt out that particular action:
    /// <code>
    /// [ValidateModelNotNullAttribute(SkipValidation = true)]
    /// public HttpResponseMessage Put(int id, Model model)
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ValidateModelNotNullAttribute : ActionFilterAttribute
    {
        private static readonly Func<Dictionary<string, object>, bool> s_containsNull = args => args.ContainsValue(null);

        /// <summary>
        /// Gets or sets a value indicating whether to skip validation (false by default).
        /// </summary>
        /// <remarks>
        /// Allows overriding the default behaviour on an individual action/controller if an instance
        /// is already registered in the global filters.
        /// </remarks>
        public bool SkipValidation { get; set; }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (SkipValidation)
            {
                return;
            }

            if (actionContext != null && s_containsNull(actionContext.ActionArguments))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The argument must not be null");
            }
        }
    }
}
