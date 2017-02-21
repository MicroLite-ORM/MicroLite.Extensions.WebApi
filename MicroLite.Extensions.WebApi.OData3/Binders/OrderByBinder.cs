// -----------------------------------------------------------------------
// <copyright file="OrderByBinder.cs" company="MicroLite">
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
    using System;
    using System.Globalization;
    using MicroLite.Builder.Syntax.Read;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;

    /// <summary>
    /// The binder class which can append the $order by query option.
    /// </summary>
    public static class OrderByBinder
    {
        /// <summary>
        /// Binds the order by query option to the sql builder.
        /// </summary>
        /// <param name="orderByQueryOption">The order by query.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the order by list for.</param>
        /// <param name="orderBySqlBuilder">The order by SQL builder.</param>
        /// <returns>The SqlBuilder after the order by clause has been added.</returns>
        public static IOrderBy BindOrderBy(OrderByQueryOption orderByQueryOption, IObjectInfo objectInfo, IOrderBy orderBySqlBuilder)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (orderBySqlBuilder == null)
            {
                throw new ArgumentNullException(nameof(orderBySqlBuilder));
            }

            if (orderByQueryOption != null)
            {
                for (int i = 0; i < orderByQueryOption.Properties.Count; i++)
                {
                    var property = orderByQueryOption.Properties[i];
                    var column = objectInfo.TableInfo.GetColumnInfoForProperty(property.Name);

                    if (column == null)
                    {
                        throw new ODataException($"The type {objectInfo.ForType.Name} does not have a property called {property.Name}");
                    }

                    var columnName = column.ColumnName;

                    if (property.Direction == OrderByDirection.Ascending)
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