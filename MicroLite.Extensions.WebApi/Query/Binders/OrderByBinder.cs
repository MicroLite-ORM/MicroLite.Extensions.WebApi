// -----------------------------------------------------------------------
// <copyright file="OrderByBinder.cs" company="MicroLite">
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
    using System.Linq;
    using MicroLite.Mapping;
    using MicroLite.Query;

    internal static class OrderByBinder
    {
        internal static IOrderBy BindOrderBy<T>(OrderByQueryOption orderByQuery, IOrderBy orderBySqlBuilder)
        {
            if (orderByQuery != null)
            {
                var objectInfo = ObjectInfo.For(typeof(T));

                foreach (var orderByProperty in orderByQuery.Properties)
                {
                    var columnName = objectInfo.TableInfo.Columns.Single(c => c.PropertyInfo.Name == orderByProperty.Name).ColumnName;

                    if (orderByProperty.Direction == OrderByDirection.Ascending)
                    {
                        orderBySqlBuilder.OrderByAscending(columnName);
                    }
                    else
                    {
                        orderBySqlBuilder.OrderByDescending(columnName);
                    }
                }
            }

            return orderBySqlBuilder;
        }
    }
}