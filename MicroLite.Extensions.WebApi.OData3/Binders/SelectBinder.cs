// -----------------------------------------------------------------------
// <copyright file="SelectBinder.cs" company="MicroLite">
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
    using Builder;
    using Builder.Syntax.Read;
    using Mapping;
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
        public static IWhereOrOrderBy BindSelect(SelectExpandQueryOption selectQueryOption, IObjectInfo objectInfo)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (selectQueryOption == null || (selectQueryOption.Properties.Count == 1 && selectQueryOption.Properties[0].Name == "*"))
            {
                return SqlBuilder.Select("*").From(objectInfo.ForType);
            }

            var columnNames = new string[selectQueryOption.Properties.Count];
            int columnCount = 0;

            for (int i = 0; i < selectQueryOption.Properties.Count; i++)
            {
                var property = selectQueryOption.Properties[i];
                var column = objectInfo.TableInfo.GetColumnInfoForProperty(property.Name);

                if (column == null)
                {
                    throw new InvalidOperationException($"The type '{objectInfo.ForType.Name}' does not contain a property named '{property.Name}'");
                }

                columnNames[columnCount++] = column.ColumnName;
            }

            return SqlBuilder.Select(columnNames).From(objectInfo.ForType);
        }
    }
}