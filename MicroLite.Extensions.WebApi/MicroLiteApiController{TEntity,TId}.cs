// -----------------------------------------------------------------------
// <copyright file="MicroLiteApiController{TEntity,TId}.cs" company="Project Contributors">
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MicroLite.Mapping;

namespace MicroLite.Extensions.WebApi
{
    /// <summary>
    /// Provides opt-in CRUD operations in addition to the base ASP.NET WebApi controller.
    /// </summary>
    /// <typeparam name="TEntity">The type of object that the controller deals with.</typeparam>
    /// <typeparam name="TId">The type of identifier for the <typeparamref name="TEntity"/>.</typeparam>
    public abstract class MicroLiteApiController<TEntity, TId> : MicroLiteApiController
        where TEntity : class, new()
    {
        private static readonly IObjectInfo s_entityObjectInfo = Mapping.ObjectInfo.For(typeof(TEntity));

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteApiController{TEntity, TId}"/> class with an ISession.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
        protected MicroLiteApiController(IAsyncSession session)
            : base(session)
        {
            GetEntityResourceUri = (TId id) => new Uri(Url.Link("DefaultApi", new { id }));
        }

        /// <summary>
        /// Gets or sets a function which returns the entity resource URI for the entity with the supplied <typeparamref name="TId"/>.
        /// </summary>
        protected Func<TId, Uri> GetEntityResourceUri { get; set; }

        /// <summary>
        /// Gets the object information for the entity.
        /// </summary>
        protected IObjectInfo ObjectInfo => s_entityObjectInfo;

        /// <summary>
        /// Deletes the <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <param name="id">The id of the <typeparamref name="TEntity"/> to be deleted.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with
        /// 204 (No Content) if the entity is deleted successfully,
        /// or 404 (Not Found) if there is no entity with the specified Id.</returns>
        protected virtual async Task<HttpResponseMessage> DeleteEntityResponseAsync(TId id)
        {
            bool deleted = await Session.Advanced.DeleteAsync(ObjectInfo.ForType, id).ConfigureAwait(false);

            return Request.CreateResponse(deleted ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Gets the <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <param name="id">The id of the <typeparamref name="TEntity"/> to be retrieved.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result
        /// 404 (Not Found) if no entity exists with the specified Identifier or
        /// 200 (OK) if an entity is found.</returns>
        protected virtual async Task<HttpResponseMessage> GetEntityResponseAsync(TId id)
        {
            TEntity entity = await Session.SingleAsync<TEntity>(id).ConfigureAwait(false);

            if (entity is null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/> based upon the values in the specified entity.
        /// </summary>
        /// <param name="entity">The entity containing the values to be created.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result 201 (Created) if the entity is successfully created.</returns>
        protected virtual async Task<HttpResponseMessage> PostEntityResponseAsync(TEntity entity)
        {
            await Session.InsertAsync(entity).ConfigureAwait(false);

            var identifier = (TId)ObjectInfo.GetIdentifierValue(entity);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, entity);
            response.Headers.Location = GetEntityResourceUri(identifier);

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
        /// 204 (NoContent) if the entity was updated successfully.</returns>
        protected virtual async Task<HttpResponseMessage> PutEntityResponseAsync(TId id, TEntity entity)
        {
            TEntity existing = await Session.SingleAsync<TEntity>(id).ConfigureAwait(false);

            if (existing is null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            ObjectInfo.SetIdentifierValue(entity, id);

            bool updated = await Session.UpdateAsync(entity).ConfigureAwait(false);

            return Request.CreateResponse(updated ? HttpStatusCode.NoContent : HttpStatusCode.NotModified);
        }
    }
}
