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
    using Net.Http.WebApi.OData.Model;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Binders;

    /// <summary>
    /// The binder class which can append the $select query option.
    /// </summary>
    public sealed class SelectBinder : AbstractSelectExpandBinder
    {
        private readonly string[] columnNames;
        private readonly IObjectInfo objectInfo;
        private int columnCount;

        private SelectBinder(IObjectInfo objectInfo, string[] columnNames)
        {
            this.objectInfo = objectInfo;
            this.columnNames = columnNames;
        }

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

            if (selectQueryOption == null)
            {
                return SqlBuilder.Select("*").From(objectInfo.ForType);
            }

            var columnNames = new string[selectQueryOption.Properties.Count];

            var selectBinder = new SelectBinder(objectInfo, columnNames);
            selectBinder.Bind(selectQueryOption);

            return SqlBuilder.Select(columnNames).From(objectInfo.ForType);
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Model.EdmProperty" />.
        /// </summary>
        /// <param name="edmProperty">The <see cref="T:Net.Http.WebApi.OData.Model.EdmProperty" /> to bind.</param>
        protected override void Bind(EdmProperty edmProperty)
        {
            if (edmProperty == null)
            {
                throw new ArgumentNullException(nameof(edmProperty));
            }

            var column = this.objectInfo.TableInfo.GetColumnInfoForProperty(edmProperty.Name);

            this.columnNames[this.columnCount++] = column.ColumnName;
        }
    }
}