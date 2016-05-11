// -----------------------------------------------------------------------
// <copyright file="MicroLiteODataApiController{T}.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.OData
{
    using System;
    using System.Net;
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Validators;

    /// <summary>
    /// A controller which adds support for OData queries to the standard <see cref="MicroLiteApiController&lt;TEntity, TId&gt;"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public abstract class MicroLiteODataApiController<TEntity, TId> : MicroLiteApiController<TEntity, TId>
        where TEntity : class, new()
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteODataApiController{TEntity, TId}"/> class.
        /// </summary>
        protected MicroLiteODataApiController()
            : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteODataApiController{TEntity, TId}"/> class.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
#if NET40

        protected MicroLiteODataApiController(ISession session)
#else

        protected MicroLiteODataApiController(IAsyncSession session)
#endif
            : base(session)
        {
            this.ValidationSettings = new ODataValidationSettings
            {
                AllowedArithmeticOperators = AllowedArithmeticOperators.All,
                AllowedFunctions = AllowedFunctions.Ceiling
                    | AllowedFunctions.Day
                    | AllowedFunctions.EndsWith
                    | AllowedFunctions.Floor
                    | AllowedFunctions.Month
                    | AllowedFunctions.Replace
                    | AllowedFunctions.Round
                    | AllowedFunctions.StartsWith
                    | AllowedFunctions.Substring
                    | AllowedFunctions.SubstringOf
                    | AllowedFunctions.ToLower
                    | AllowedFunctions.ToUpper
                    | AllowedFunctions.Trim
                    | AllowedFunctions.Year,
                AllowedLogicalOperators = AllowedLogicalOperators.All,
                AllowedQueryOptions = AllowedQueryOptions.Filter
                    | AllowedQueryOptions.Format
                    | AllowedQueryOptions.InlineCount
                    | AllowedQueryOptions.OrderBy
                    | AllowedQueryOptions.Select
                    | AllowedQueryOptions.Skip
                    | AllowedQueryOptions.Top,
                MaxTop = 50
            };
        }

        /// <summary>
        /// Gets or sets the Validation Settings for the OData query.
        /// </summary>
        protected ODataValidationSettings ValidationSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the SQL query to use based upon the values in the specified ODataQueryOptions.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSqlQuery(ODataQueryOptions queryOptions)
        {
            var sqlQuery = queryOptions.CreateSqlQuery<TEntity>();

            return sqlQuery;
        }

        /// <summary>
        /// Gets the entity results based upon the OData query.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The whole point to this method is that it returns the object!")]
#if NET40
        protected virtual HttpResponseMessage GetEntityResponse(ODataQueryOptions queryOptions)
#else
        protected virtual async System.Threading.Tasks.Task<HttpResponseMessage> GetEntityResponseAsync(ODataQueryOptions queryOptions)
#endif
        {
            if (queryOptions == null)
            {
                throw new ArgumentNullException("queryOptions");
            }

            queryOptions.Validate(this.ValidationSettings);

            var sqlQuery = this.CreateSqlQuery(queryOptions);

            var skip = queryOptions.Skip != null ? queryOptions.Skip.Value : 0;
            var top = queryOptions.Top != null ? queryOptions.Top.Value : this.ValidationSettings.MaxTop;
#if NET40
            var paged = this.Session.Paged<dynamic>(sqlQuery, PagingOptions.SkipTake(skip, top));
#else
            var paged = await this.Session.PagedAsync<dynamic>(sqlQuery, PagingOptions.SkipTake(skip, top));
#endif

            HttpResponseMessage response;

            if (queryOptions.InlineCount != null && queryOptions.InlineCount.InlineCount == InlineCount.AllPages)
            {
                response = this.Request.CreateResponse(HttpStatusCode.OK, new InlineCount<dynamic>(paged.Results, paged.TotalResults));
            }
            else
            {
                response = this.Request.CreateResponse(HttpStatusCode.OK, paged.Results);
            }

            if (queryOptions.Format != null)
            {
                response.Content.Headers.ContentType = queryOptions.Format.MediaTypeHeaderValue;
            }

            return response;
        }
    }
}