﻿// -----------------------------------------------------------------------
// <copyright file="MicroLiteApiController<TEntity, TId>.cs" company="MicroLite">
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
    using System.Net;
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Filters;
    using MicroLite.Mapping;

    /// <summary>
    /// Provides access to a MicroLite ISession in addition to the base ASP.NET WebApi controller.
    /// </summary>
    /// <typeparam name="TEntity">The type of object that the controller deals with.</typeparam>
    /// <typeparam name="TId">The type of identifier for the <typeparamref name="TEntity"/>.</typeparam>
    public abstract class MicroLiteApiController<TEntity, TId> : MicroLiteApiController
        where TEntity : class, new()
    {
        private static IObjectInfo objectInfo = ObjectInfo.For(typeof(TEntity));

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteApiController{TEntity, TId}"/> class.
        /// </summary>
        protected MicroLiteApiController()
        {
            this.GetEntityResourceUri = (TId id) =>
            {
                var entityUri = this.Url.Link(
                    "DefaultApi",
                    new
                    {
                        id = id
                    });

                return new Uri(entityUri);
            };
        }

        /// <summary>
        /// Gets or sets a function which returns the entity resource URI for the entity with the supplied <typeparamref name="TId"/>.
        /// </summary>
        protected Func<TId, Uri> GetEntityResourceUri
        {
            get;
            set;
        }

        /// <summary>
        /// Deletes the <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <param name="id">The id of the <typeparamref name="TEntity"/> to be deleted.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with
        /// 204 (No Content) if the entity is deleted successfully,
        /// or 404 (Not Found) if there is no entity with the specified Id.</returns>
        /// <remarks><![CDATA[http://www.odata.org/documentation/odata-v3-documentation/odata-core/#1034_Delete_an_Entity]]></remarks>
        public virtual HttpResponseMessage Delete(TId id)
        {
            HttpResponseMessage response;

            var deleted = this.Session.Advanced.Delete(objectInfo.ForType, id);

            if (!deleted)
            {
                response = this.Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                response = this.Request.CreateResponse(HttpStatusCode.NoContent);
            }

            return response;
        }

        /// <summary>
        /// Gets the <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <param name="id">The id of the <typeparamref name="TEntity"/> to be retrieved.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "In WebApi the convention is to use the HTTP Verb as the method name")]
        public virtual HttpResponseMessage Get(TId id)
        {
            HttpResponseMessage response;

            var entity = this.Session.Single<TEntity>(id);

            if (entity == null)
            {
                response = this.Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                response = this.Request.CreateResponse(HttpStatusCode.OK, entity);
            }

            return response;
        }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/> based upon the values in the specified entity.
        /// </summary>
        /// <param name="entity">The entity containing the values to be created.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result 201 (Created) if the entity is successfully created.</returns>
        /// <remarks><![CDATA[http://www.odata.org/documentation/odata-v3-documentation/odata-core/#1032_Create_an_Entity]]></remarks>
        [ValidateModelNotNull]
        [ValidateModelState]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The method is returning the response, the framework will be responsible for its disposal")]
        public virtual HttpResponseMessage Post(TEntity entity)
        {
            this.Session.Insert(entity);

            var identifier = (TId)objectInfo.GetIdentifierValue(entity);

            var response = this.Request.CreateResponse(HttpStatusCode.Created, entity);
            response.Headers.Location = this.GetEntityResourceUri(identifier);

            return response;
        }

        /// <summary>
        /// Updates an existing <typeparamref name="TEntity"/> with the values in the specified entity.
        /// </summary>
        /// <param name="id">The identifier of the entity to update.</param>
        /// <param name="entity">The entity containing the values to be updated.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result
        /// 404 (Not Found) if no entity was found with the specified Id to update,
        /// 304 (Not Modified) if there were no changes or
        /// 200 (OK) if the entity was updated successfully.</returns>
        /// <remarks><![CDATA[http://www.odata.org/documentation/odata-v3-documentation/odata-core/#1033_Update_an_Entity]]></remarks>
        [ValidateModelNotNull]
        [ValidateModelState]
        public virtual HttpResponseMessage Put(TId id, TEntity entity)
        {
            HttpResponseMessage response;

            var existing = this.Session.Single<TEntity>(id);

            if (existing == null)
            {
                response = this.Request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                objectInfo.Map(entity, existing, includeId: false);

                var updated = this.Session.Update(existing);

                if (!updated)
                {
                    response = this.Request.CreateResponse(HttpStatusCode.NotModified);
                }
                else
                {
                    response = this.Request.CreateResponse(HttpStatusCode.OK);
                }
            }

            return response;
        }
    }
}