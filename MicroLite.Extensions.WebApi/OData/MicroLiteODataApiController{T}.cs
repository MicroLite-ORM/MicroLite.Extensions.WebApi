// -----------------------------------------------------------------------
// <copyright file="MicroLiteODataApiController{T}.cs" company="MicroLite">
// Copyright 2012-2013 Trevor Pilley
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
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Validation;

    /// <summary>
    /// A controller which adds support for OData queries to the standard <see cref="MicroLiteApiController&lt;TEntity, TId&gt;"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public abstract class MicroLiteODataApiController<TEntity, TId> : MicroLiteApiController<TEntity, TId>
        where TEntity : class, new()
    {
        private ODataValidationSettings validationSettings;

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteODataApiController{TEntity, TId}"/> class.
        /// </summary>
        protected MicroLiteODataApiController()
        {
        }

        /// <summary>
        /// Gets or the Validation Settings for the OData query.
        /// </summary>
        protected ODataValidationSettings ValidationSettings
        {
            get
            {
                return this.validationSettings ?? (this.validationSettings = new ODataValidationSettings
                {
                    AllowedQueryOptions = AllowedQueryOptions.Filter
                        | AllowedQueryOptions.Format
                        | AllowedQueryOptions.InlineCount
                        | AllowedQueryOptions.OrderBy
                        | AllowedQueryOptions.Select
                        | AllowedQueryOptions.Skip
                        | AllowedQueryOptions.Top,
                    MaxTop = 50
                });
            }
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "In WebApi the convention is to use the HTTP Verb as the method name")]
        protected virtual HttpResponseMessage GetEntityResponse(ODataQueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                throw new ArgumentNullException("queryOptions");
            }

            queryOptions.Validate(this.ValidationSettings);

            var sqlQuery = this.CreateSqlQuery(queryOptions);

            var skip = queryOptions.Skip != null ? queryOptions.Skip.Value : 0;
            var top = queryOptions.Top != null ? queryOptions.Top.Value : this.ValidationSettings.MaxTop;

            var paged = this.Session.Paged<dynamic>(sqlQuery, PagingOptions.SkipTake(skip, top));

            HttpResponseMessage response;

            if (queryOptions.InlineCount != null && queryOptions.InlineCount.InlineCount == InlineCount.AllPages)
            {
                response = this.Request.CreateResponse(HttpStatusCode.OK, new InlineCountCollection<dynamic>(paged.Results, paged.TotalResults));
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