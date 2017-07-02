// -----------------------------------------------------------------------
// <copyright file="MicroLiteODataApiController{T}.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.OData
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Binders;
    using Builder;
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
        private static SqlQuery entityCountQuery;

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteODataApiController{TEntity, TId}"/> class.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
        protected MicroLiteODataApiController(IAsyncSession session)
            : base(session)
        {
            this.ValidationSettings = new ODataValidationSettings
            {
                AllowedArithmeticOperators = AllowedArithmeticOperators.All,
                AllowedFunctions = AllowedFunctions.Ceiling
                    | AllowedFunctions.Contains
                    | AllowedFunctions.Day
                    | AllowedFunctions.EndsWith
                    | AllowedFunctions.Floor
                    | AllowedFunctions.Month
                    | AllowedFunctions.Replace
                    | AllowedFunctions.Round
                    | AllowedFunctions.StartsWith
                    | AllowedFunctions.Substring
                    | AllowedFunctions.ToLower
                    | AllowedFunctions.ToUpper
                    | AllowedFunctions.Trim
                    | AllowedFunctions.Year,
                AllowedLogicalOperators = AllowedLogicalOperators.All & ~AllowedLogicalOperators.Has,
                AllowedQueryOptions = AllowedQueryOptions.Count
                    | AllowedQueryOptions.Filter
                    | AllowedQueryOptions.Format
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
        /// Creates the SQL query to count the number if entities in the Entity Set.
        /// </summary>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateCountSqlQuery()
        {
            if (entityCountQuery == null)
            {
                entityCountQuery = SqlBuilder.Select()
                    .Count(ObjectInfo.TableInfo.IdentifierColumn.ColumnName)
                    .From(ObjectInfo.ForType)
                    .ToSqlQuery();
            }

            return entityCountQuery;
        }

        /// <summary>
        /// Creates the SQL query to query entities in the Entity Set based upon the values in the specified ODataQueryOptions.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSqlQuery(ODataQueryOptions queryOptions)
        {
            var sqlQuery = queryOptions.CreateSqlQuery();

            return sqlQuery;
        }

        /// <summary>
        /// Gets the entity count response.
        /// </summary>
        /// <returns>The an <see cref="HttpResponseMessage"/> containing the entity count.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It will be a Web API method")]
        protected virtual async Task<HttpResponseMessage> GetCountResponseAsync()
        {
            var sqlQuery = this.CreateCountSqlQuery();

            var count = await this.Session.Advanced.ExecuteScalarAsync<long>(sqlQuery);

            return this.Request.CreateODataResponse(count.ToString());
        }

        /// <summary>
        /// Gets the entity results based upon the OData query.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The whole point to this method is that it returns the object!")]
        protected virtual async Task<HttpResponseMessage> GetEntityResponseAsync(ODataQueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                throw new ArgumentNullException(nameof(queryOptions));
            }

            queryOptions.Validate(this.ValidationSettings);

            var sqlQuery = this.CreateSqlQuery(queryOptions);

            var skip = queryOptions.Skip ?? 0;
            var top = queryOptions.Top ?? this.ValidationSettings.MaxTop;

            var paged = await this.Session.PagedAsync<dynamic>(sqlQuery, PagingOptions.SkipTake(skip, top));

            Uri context = null;
            int? count = queryOptions.Count ? paged.TotalResults : default(int?);
            Uri nextLink = paged.MoreResultsAvailable ? queryOptions.NextLink(skip, paged.ResultsPerPage) : null;

            var responseContent = new ODataResponseContent(context, paged.Results, count, nextLink);

            var response = this.Request.CreateODataResponse(HttpStatusCode.OK, responseContent);

            if (queryOptions.Format != null)
            {
                response.Content.Headers.ContentType = queryOptions.Format.MediaTypeHeaderValue;
            }

            return response;
        }
    }
}