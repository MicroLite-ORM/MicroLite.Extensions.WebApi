// -----------------------------------------------------------------------
// <copyright file="ODataQueryOptionExtensions.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.Query.Binders
{
    internal static class ODataQueryOptionExtensions
    {
        internal static SqlQuery CreateSqlQuery<T>(this ODataQueryOptions queryOptions)
        {
            var sqlBuilder = SelectBinder.BindSelectQueryOption<T>(queryOptions);
            OrderByBinder.BindOrderBy<T>(sqlBuilder, queryOptions.OrderBy);

            return sqlBuilder.ToSqlQuery();
        }
    }
}