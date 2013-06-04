// -----------------------------------------------------------------------
// <copyright file="SelectBinder.cs" company="MicroLite">
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

    internal static class SelectBinder
    {
        public static IWhereOrOrderBy BindSelectQueryOption<T>(ODataQueryOptions queryOptions)
        {
            string[] columnNames;

            if (queryOptions.Select == null || (queryOptions.Select.Properties.Count == 1 && queryOptions.Select.Properties[0] == "*"))
            {
                columnNames = new string[] { "*" };
            }
            else
            {
                var objectInfo = ObjectInfo.For(typeof(T));

                columnNames = queryOptions
                    .Select
                    .Properties
                    .Select(s => objectInfo.TableInfo.Columns.Single(c => c.PropertyInfo.Name == s).ColumnName)
                    .ToArray();
            }

            return SqlBuilder.Select(string.Join(", ", columnNames)).From(typeof(T));
        }
    }
}