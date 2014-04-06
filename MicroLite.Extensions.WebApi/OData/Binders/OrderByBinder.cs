// -----------------------------------------------------------------------
// <copyright file="OrderByBinder.cs" company="MicroLite">
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
    using System;
    using System.Globalization;
    using System.Linq;
    using MicroLite.Builder;
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
        /// <param name="orderByQuery">The order by query.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the order by list for.</param>
        /// <param name="orderBySqlBuilder">The order by SQL builder.</param>
        /// <returns>The SqlBuilder after the order by clause has been added.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Work in progress, might not be required in the long run but for now we need the type not an instance.")]
        public static IOrderBy BindOrderBy(OrderByQueryOption orderByQuery, IObjectInfo objectInfo, IOrderBy orderBySqlBuilder)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (orderBySqlBuilder == null)
            {
                throw new ArgumentNullException("orderBySqlBuilder");
            }

            if (orderByQuery != null)
            {
                foreach (var property in orderByQuery.Properties)
                {
                    var column = objectInfo.TableInfo.Columns.SingleOrDefault(c => c.PropertyInfo.Name == property.Name);

                    if (column == null)
                    {
                        throw new ODataException(string.Format(CultureInfo.InvariantCulture, Messages.InvalidPropertyName, objectInfo.ForType.Name, property.Name));
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