// -----------------------------------------------------------------------
// <copyright file="ODataQueryOptionsExtensions.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.OData.Binders
{
    using Mapping;
    using Net.Http.WebApi.OData.Query;

    internal static class ODataQueryOptionsExtensions
    {
        internal static SqlQuery CreateSqlQuery(this ODataQueryOptions queryOptions)
        {
            var objectInfo = ObjectInfo.For(queryOptions.Model.ClrType);

            var whereSqlBuilder = SelectBinder.BindSelect(queryOptions.Select, objectInfo);
            var orderBySqlBuilder = FilterBinder.BindFilter(queryOptions.Filter, objectInfo, whereSqlBuilder);
            orderBySqlBuilder = OrderByBinder.BindOrderBy(queryOptions.OrderBy, objectInfo, orderBySqlBuilder);

            return orderBySqlBuilder.ToSqlQuery();
        }
    }
}