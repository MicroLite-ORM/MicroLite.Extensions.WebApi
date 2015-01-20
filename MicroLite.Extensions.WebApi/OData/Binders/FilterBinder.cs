// -----------------------------------------------------------------------
// <copyright file="FilterBinder.cs" company="MicroLite">
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
        private static readonly string[] ParameterisedFunctions = new[] { "startswith", "endswith", "substringof" };
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
        /// <param name="filterQuery">The filter query.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the filter list for.</param>
        /// <param name="selectFromSqlBuilder">The select from SQL builder.</param>
        /// <returns>The SqlBuilder after the where clause has been added.</returns>
        public static IOrderBy BindFilter(FilterQueryOption filterQuery, IObjectInfo objectInfo, IWhereOrOrderBy selectFromSqlBuilder)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (selectFromSqlBuilder == null)
            {
                throw new ArgumentNullException("selectFromSqlBuilder");
            }

            if (filterQuery != null)
            {
                var filterBinder = new FilterBinder(objectInfo);
                filterBinder.BindFilter(filterQuery, selectFromSqlBuilder);
            }

            return selectFromSqlBuilder;
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.BinaryOperatorNode" />.
        /// </summary>
        /// <param name="binaryOperatorNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.BinaryOperatorNode" /> to bind.</param>
        protected override void BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
        {
            this.predicateBuilder.Append("(");

            this.Bind(binaryOperatorNode.Left);

            if (binaryOperatorNode.Left.Kind != QueryNodeKind.SingleValueFunctionCall
                || (binaryOperatorNode.Left.Kind == QueryNodeKind.SingleValueFunctionCall && !ParameterisedFunctions.Contains(((SingleValueFunctionCallNode)binaryOperatorNode.Left).Name)))
            {
                if (binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                    && ((ConstantNode)binaryOperatorNode.Right).Value == null)
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
            if (constantNode.Value == null)
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
            var arguments = singleValueFunctionCallNode.Arguments;

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

                    for (int i = 0; i < arguments.Count; i++)
                    {
                        this.Bind(arguments[i]);

                        if (i < arguments.Count - 1)
                        {
                            this.predicateBuilder.Append(", ");
                        }
                    }

                    this.predicateBuilder.Append(")");
                    break;

                case "endswith":
                    this.Bind(arguments[0]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        this.sqlCharacters.LikeWildcard + ((ConstantNode)arguments[1]).LiteralText);
                    break;

                case "startswith":
                    this.Bind(arguments[0]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        ((ConstantNode)arguments[1]).LiteralText + this.sqlCharacters.LikeWildcard);
                    break;

                case "substringof":
                    this.Bind(arguments[1]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        this.sqlCharacters.LikeWildcard + ((ConstantNode)arguments[0]).LiteralText + this.sqlCharacters.LikeWildcard);
                    break;

                case "trim":
                    this.predicateBuilder.Append("LTRIM(RTRIM(");
                    this.Bind(arguments[0]);
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