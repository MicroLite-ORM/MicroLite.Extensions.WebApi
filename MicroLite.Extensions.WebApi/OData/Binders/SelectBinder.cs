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
        /// <summary>
        /// Binds the select query option to the SqlBuilder.
        /// </summary>
        /// <param name="selectQueryOption">The select query option.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the select list for.</param>
        /// <returns>The SqlBuilder after the select and from clauses have been added.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Work in progress, might not be required in the long run but for now we need the type not an instance.")]
        public static IWhereOrOrderBy BindSelect(SelectQueryOption selectQueryOption, IObjectInfo objectInfo)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (selectQueryOption == null || (selectQueryOption.Properties.Count == 1 && selectQueryOption.Properties[0] == "*"))
            {
                return SqlBuilder.Select("*").From(objectInfo.ForType);
            }

            var columnNames = new string[selectQueryOption.Properties.Count];
            int columnCount = 0;

            foreach (var property in selectQueryOption.Properties)
            {
                var column = objectInfo.TableInfo.Columns.SingleOrDefault(c => c.PropertyInfo.Name == property);

                if (column == null)
                {
                    throw new ODataException(string.Format(CultureInfo.InvariantCulture, Messages.InvalidPropertyName, objectInfo.ForType.Name, property));
                }

                columnNames[columnCount++] = column.ColumnName;
            }

            return SqlBuilder.Select(columnNames).From(objectInfo.ForType);
        }
    }
}