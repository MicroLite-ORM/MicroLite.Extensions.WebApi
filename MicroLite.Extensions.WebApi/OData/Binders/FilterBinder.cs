// -----------------------------------------------------------------------
// <copyright file="FilterBinder.cs" company="MicroLite">
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
    using MicroLite.Builder;
    using MicroLite.Builder.Syntax.Read;
    using MicroLite.Characters;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Expressions;

    /// <summary>
    /// The binder class which can append the $filter by query option.
    /// </summary>
    public sealed class FilterBinder : AbstractFilterBinder
    {
        private readonly IObjectInfo objectInfo;
        private readonly RawWhereBuilder predicateBuilder = new RawWhereBuilder();
        private readonly SqlCharacters sqlCharacters = SqlCharacters.Current;

        private FilterBinder(IObjectInfo objectInfo)
        {
            this.objectInfo = objectInfo;
        }

        /// <summary>
        /// Binds the filter query option to the sql builder.
        /// </summary>
        /// <param name="filterQueryOption">The filter query.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the filter list for.</param>
        /// <param name="selectFromSqlBuilder">The select from SQL builder.</param>
        /// <returns>The SqlBuilder after the where clause has been added.</returns>
        public static IOrderBy BindFilter(FilterQueryOption filterQueryOption, IObjectInfo objectInfo, IWhereOrOrderBy selectFromSqlBuilder)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (selectFromSqlBuilder == null)
            {
                throw new ArgumentNullException("selectFromSqlBuilder");
            }

            if (filterQueryOption != null)
            {
                var filterBinder = new FilterBinder(objectInfo);
                filterBinder.BindFilter(filterQueryOption, selectFromSqlBuilder);
            }

            return selectFromSqlBuilder;
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.BinaryOperatorNode" />.
        /// </summary>
        /// <param name="binaryOperatorNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.BinaryOperatorNode" /> to bind.</param>
        protected override void BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
        {
            if (binaryOperatorNode == null)
            {
                throw new ArgumentNullException("binaryOperatorNode");
            }

            this.predicateBuilder.Append("(");

            this.Bind(binaryOperatorNode.Left);

            // ignore 'eq true' or 'eq false' for method calls
            if (!(binaryOperatorNode.Left.Kind == QueryNodeKind.SingleValueFunctionCall
                && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal
                && binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                && ((ConstantNode)binaryOperatorNode.Right).EdmType == EdmType.Boolean))
            {
                if (binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                    && ((ConstantNode)binaryOperatorNode.Right).EdmType == EdmType.Null)
                {
                    if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal)
                    {
                        this.predicateBuilder.Append(" IS ");
                    }
                    else if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.NotEqual)
                    {
                        this.predicateBuilder.Append(" IS NOT ");
                    }
                }
                else
                {
                    this.predicateBuilder.Append(" " + binaryOperatorNode.OperatorKind.ToSqlOperator() + " ");
                }

                this.Bind(binaryOperatorNode.Right);
            }

            this.predicateBuilder.Append(")");
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.ConstantNode" />.
        /// </summary>
        /// <param name="constantNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.ConstantNode" /> to bind.</param>
        protected override void BindConstantNode(ConstantNode constantNode)
        {
            if (constantNode == null)
            {
                throw new ArgumentNullException("constantNode");
            }

            if (constantNode.EdmType == EdmType.Null)
            {
                this.predicateBuilder.Append("NULL");
            }
            else
            {
                this.predicateBuilder.Append(this.sqlCharacters.GetParameterName(0), constantNode.Value);
            }
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.SingleValueFunctionCallNode" />.
        /// </summary>
        /// <param name="singleValueFunctionCallNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.SingleValueFunctionCallNode" /> to bind.</param>
        protected override void BindSingleValueFunctionCallNode(SingleValueFunctionCallNode singleValueFunctionCallNode)
        {
            if (singleValueFunctionCallNode == null)
            {
                throw new ArgumentNullException("singleValueFunctionCallNode");
            }

            var parameters = singleValueFunctionCallNode.Parameters;

            switch (singleValueFunctionCallNode.Name)
            {
                case "ceiling":
                case "day":
                case "floor":
                case "month":
                case "replace":
                case "round":
                case "substring":
                case "tolower":
                case "toupper":
                case "year":
                    var name = singleValueFunctionCallNode.Name.StartsWith("to", StringComparison.Ordinal)
                        ? singleValueFunctionCallNode.Name.Substring(2)
                        : singleValueFunctionCallNode.Name;

                    this.predicateBuilder.Append(name.ToUpperInvariant() + "(");

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        this.Bind(parameters[i]);

                        if (i < parameters.Count - 1)
                        {
                            this.predicateBuilder.Append(", ");
                        }
                    }

                    this.predicateBuilder.Append(")");
                    break;

                case "endswith":
                    this.Bind(parameters[0]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        this.sqlCharacters.LikeWildcard + ((ConstantNode)parameters[1]).Value);
                    break;

                case "startswith":
                    this.Bind(parameters[0]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        ((ConstantNode)parameters[1]).Value + this.sqlCharacters.LikeWildcard);
                    break;

                case "substringof":
                    this.Bind(parameters[1]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        this.sqlCharacters.LikeWildcard + ((ConstantNode)parameters[0]).Value + this.sqlCharacters.LikeWildcard);
                    break;

                case "trim":
                    this.predicateBuilder.Append("LTRIM(RTRIM(");
                    this.Bind(parameters[0]);
                    this.predicateBuilder.Append("))");
                    break;

                default:
                    throw new ODataException("The function '" + singleValueFunctionCallNode.Name + "' is not supported");
            }
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.SingleValuePropertyAccessNode" />.
        /// </summary>
        /// <param name="singleValuePropertyAccessNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.SingleValuePropertyAccessNode" /> to bind.</param>
        protected override void BindSingleValuePropertyAccessNode(SingleValuePropertyAccessNode singleValuePropertyAccessNode)
        {
            if (singleValuePropertyAccessNode == null)
            {
                throw new ArgumentNullException("singleValuePropertyAccessNode");
            }

            var column = this.objectInfo.TableInfo.GetColumnInfoForProperty(singleValuePropertyAccessNode.PropertyName);

            if (column == null)
            {
                throw new ODataException(string.Format(CultureInfo.InvariantCulture, Messages.InvalidPropertyName, this.objectInfo.ForType.Name, singleValuePropertyAccessNode.PropertyName));
            }

            this.predicateBuilder.Append(column.ColumnName);
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.UnaryOperatorNode" />.
        /// </summary>
        /// <param name="unaryOperatorNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.UnaryOperatorNode" /> to bind.</param>
        protected override void BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode)
        {
            if (unaryOperatorNode == null)
            {
                throw new ArgumentNullException("unaryOperatorNode");
            }

            this.predicateBuilder.Append(unaryOperatorNode.OperatorKind.ToSqlOperator() + " ");
            this.Bind(unaryOperatorNode.Operand);
        }

        private IAndOrOrderBy BindFilter(FilterQueryOption filterQuery, IWhereOrOrderBy selectFromSqlBuilder)
        {
            this.Bind(filterQuery.Expression);

            var where = this.predicateBuilder.ApplyTo(selectFromSqlBuilder);

            return where;
        }
    }
}