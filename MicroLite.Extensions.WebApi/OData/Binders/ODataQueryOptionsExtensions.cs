﻿// -----------------------------------------------------------------------
// <copyright file="ODataQueryOptionsExtensions.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.OData.Binders
{
    using Net.Http.WebApi.OData.Query;

    internal static class ODataQueryOptionsExtensions
    {
        internal static SqlQuery CreateSqlQuery<T>(this ODataQueryOptions queryOptions)
        {
            var selectFrom = SelectBinder.BindSelectQueryOption<T>(queryOptions);
            var where = FilterBinder.BindFilter<T>(queryOptions.Filter, selectFrom);
            var ordered = OrderByBinder.BindOrderBy<T>(queryOptions.OrderBy, where);

            return ordered.ToSqlQuery();
        }
    }
}