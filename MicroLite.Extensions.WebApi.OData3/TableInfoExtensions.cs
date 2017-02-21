// -----------------------------------------------------------------------
// <copyright file="TableInfoExtensions.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi.OData
{
    using MicroLite.Mapping;

    internal static class TableInfoExtensions
    {
        internal static ColumnInfo GetColumnInfoForProperty(this TableInfo tableInfo, string propertyName)
        {
            for (int i = 0; i < tableInfo.Columns.Count; i++)
            {
                var column = tableInfo.Columns[i];

                if (column.PropertyInfo.Name == propertyName)
                {
                    return column;
                }
            }

            return null;
        }
    }
}