// -----------------------------------------------------------------------
// <copyright file="SelectBinder.cs" company="MicroLite">
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
    /// The binder class which can append the $select query option.
    /// </summary>
    public static class SelectBinder
    {
        private static readonly string[] AllColumnNames = new string[] { "*" };

        /// <summary>
        /// Binds the select query option to the SqlBuilder.
        /// </summary>
        /// <typeparam name="T">The type of class being queried.</typeparam>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The SqlBuilder after the select and from clauses have been added.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Work in progress, might not be required in the long run but for now we need the type not an instance.")]
        public static IWhereOrOrderBy BindSelectQueryOption<T>(ODataQueryOptions queryOptions)
        {
            if (queryOptions == null)
            {
                throw new ArgumentNullException("queryOptions");
            }

            string[] columnNames;

            if (queryOptions.Select == null || (queryOptions.Select.Properties.Count == 1 && queryOptions.Select.Properties[0] == "*"))
            {
                columnNames = AllColumnNames;
            }
            else
            {
                var objectInfo = ObjectInfo.For(typeof(T));

                columnNames = new string[queryOptions.Select.Properties.Count];
                int columnCount = 0;

                foreach (var property in queryOptions.Select.Properties)
                {
                    var column = objectInfo.TableInfo.Columns.SingleOrDefault(c => c.PropertyInfo.Name == property);

                    if (column == null)
                    {
                        throw new ODataException(string.Format(CultureInfo.InvariantCulture, Messages.InvalidPropertyName, objectInfo.ForType.Name, property));
                    }

                    columnNames[columnCount++] = column.ColumnName;
                }
            }

            return SqlBuilder.Select(columnNames).From(typeof(T));
        }
    }
}