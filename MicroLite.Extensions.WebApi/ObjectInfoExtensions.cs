// -----------------------------------------------------------------------
// <copyright file="ObjectInfoExtensions.cs" company="MicroLite">
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
namespace MicroLite.Extensions.WebApi
{
    using System.Linq;
    using MicroLite.Mapping;

    internal static class ObjectInfoExtensions
    {
        internal static void Map(this IObjectInfo objectInfo, object source, object destination, bool includeId = false)
        {
            foreach (var propertyName in objectInfo.TableInfo.Columns.Select(c => c.PropertyInfo.Name))
            {
                if (propertyName == objectInfo.TableInfo.IdentifierProperty && !includeId)
                {
                    continue;
                }

                var value = objectInfo.GetPropertyValue(source, propertyName);
                objectInfo.SetPropertyValue(destination, propertyName, value);
            }
        }
    }
}