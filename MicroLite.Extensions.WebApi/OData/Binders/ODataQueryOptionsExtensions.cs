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
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData.Query;

    internal static class ODataQueryOptionsExtensions
    {
        internal static SqlQuery CreateSqlQuery<T>(this ODataQueryOptions queryOptions)
        {
            var objectInfo = ObjectInfo.For(typeof(T));

            var selectFrom = SelectBinder.BindSelect(queryOptions.Select, objectInfo);
            var where = FilterBinder.BindFilter(queryOptions.Filter, objectInfo, selectFrom);
            var ordered = OrderByBinder.BindOrderBy(queryOptions.OrderBy, objectInfo, where);

            return ordered.ToSqlQuery();
        }
    }
}