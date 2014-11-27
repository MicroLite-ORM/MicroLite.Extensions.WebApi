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
    public static class FilterBinder
    {
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
                var filterBinder = new FilterBinderImpl(objectInfo);
                filterBinder.BindFilter(filterQuery, selectFromSqlBuilder);
            }

            return selectFromSqlBuilder;
        }

        private sealed class FilterBinderImpl
        {
            private static readonly string[] ParameterisedFunctions = new[] { "startswith", "endswith", "substringof" };
            private readonly IObjectInfo objectInfo;
            private readonly RawWhereBuilder predicateBuilder = new RawWhereBuilder();
            private readonly SqlCharacters sqlCharacters = SqlCharacters.Current;

            internal FilterBinderImpl(IObjectInfo objectInfo)
            {
                this.objectInfo = objectInfo;
            }

            internal IAndOrOrderBy BindFilter(FilterQueryOption filterQuery, IWhereOrOrderBy selectFromSqlBuilder)
            {
                this.BindFilter(filterQuery);

                var where = this.predicateBuilder.ApplyTo(selectFromSqlBuilder);

                return where;
            }

            private void Bind(QueryNode node)
            {
                var singleValueNode = node as SingleValueNode;

                if (singleValueNode == null)
                {
                    throw new NotSupportedException();
                }

                switch (node.Kind)
                {
                    case QueryNodeKind.BinaryOperator:
                        this.BindBinaryOperatorNode(node as BinaryOperatorNode);
                        break;

                    case QueryNodeKind.Constant:
                        this.BindConstantNode(node as ConstantNode);
                        break;

                    case QueryNodeKind.SingleValuePropertyAccess:
                        this.BindPropertyAccessQueryNode(node as SingleValuePropertyAccessNode);
                        break;

                    case QueryNodeKind.SingleValueFunctionCall:
                        this.BindSingleValueFunctionCallNode(node as SingleValueFunctionCallNode);
                        break;

                    default:
                        throw new NotSupportedException("Nodes of type '" + node.Kind + "' are not supported");
                }
            }

            private void BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
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

            private void BindConstantNode(ConstantNode constantNode)
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

            private void BindFilter(FilterQueryOption filterQuery)
            {
                this.Bind(filterQuery.Expression);
            }

            private void BindPropertyAccessQueryNode(SingleValuePropertyAccessNode singleValuePropertyAccessNode)
            {
                var column = this.objectInfo.TableInfo.GetColumnInfoForProperty(singleValuePropertyAccessNode.PropertyName);

                if (column == null)
                {
                    throw new ODataException(string.Format(CultureInfo.InvariantCulture, Messages.InvalidPropertyName, this.objectInfo.ForType.Name, singleValuePropertyAccessNode.PropertyName));
                }

                this.predicateBuilder.Append(column.ColumnName);
            }

            private void BindSingleValueFunctionCallNode(SingleValueFunctionCallNode singleValueFunctionCallNode)
            {
                var arguments = singleValueFunctionCallNode.Arguments;

                switch (singleValueFunctionCallNode.Name)
                {
                    case "endswith":
                        this.Bind(arguments[0]);
                        this.predicateBuilder.Append(" LIKE " + this.sqlCharacters.GetParameterName(0), this.sqlCharacters.LikeWildcard + ((ConstantNode)arguments[1]).LiteralText);
                        break;

                    case "startswith":
                        this.Bind(arguments[0]);
                        this.predicateBuilder.Append(" LIKE " + this.sqlCharacters.GetParameterName(0), ((ConstantNode)arguments[1]).LiteralText + this.sqlCharacters.LikeWildcard);
                        break;

                    case "substringof":
                        this.Bind(arguments[1]);
                        this.predicateBuilder.Append(" LIKE " + this.sqlCharacters.GetParameterName(0), this.sqlCharacters.LikeWildcard + ((ConstantNode)arguments[0]).LiteralText + this.sqlCharacters.LikeWildcard);
                        break;

                    case "toupper":
                    case "tolower":
                        this.predicateBuilder.Append(singleValueFunctionCallNode.Name.Substring(2).ToUpperInvariant() + "(");
                        this.Bind(arguments[0]);
                        this.predicateBuilder.Append(")");
                        break;

                    case "year":
                    case "month":
                    case "day":
                        this.predicateBuilder.Append(singleValueFunctionCallNode.Name.ToUpperInvariant() + "(");
                        this.Bind(arguments[0]);
                        this.predicateBuilder.Append(")");
                        break;

                    case "replace":
                    case "substring":
                        this.predicateBuilder.Append(singleValueFunctionCallNode.Name.ToUpperInvariant() + "(");
                        this.Bind(arguments[0]);
                        this.predicateBuilder.Append(", " + this.sqlCharacters.GetParameterName(0), ((ConstantNode)arguments[1]).Value);

                        if (arguments.Count > 2)
                        {
                            this.predicateBuilder.Append(", " + this.sqlCharacters.GetParameterName(0), ((ConstantNode)arguments[2]).Value);
                        }

                        this.predicateBuilder.Append(")");
                        break;

                    default:
                        throw new ODataException("The function '" + singleValueFunctionCallNode.Name + "' is not supported");
                }
            }
        }
    }
}