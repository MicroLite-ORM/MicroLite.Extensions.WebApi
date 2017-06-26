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
    using Builder;
    using Builder.Syntax.Read;
    using Characters;
    using Mapping;
    using Net.Http.WebApi.OData.Model;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Binders;
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
        /// <param name="whereSqlBuilder">The select from SQL builder.</param>
        /// <returns>The SqlBuilder after the where clause has been added.</returns>
        public static IOrderBy BindFilter(FilterQueryOption filterQueryOption, IObjectInfo objectInfo, IWhereOrOrderBy whereSqlBuilder)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (whereSqlBuilder == null)
            {
                throw new ArgumentNullException(nameof(whereSqlBuilder));
            }

            if (filterQueryOption != null)
            {
                var filterBinder = new FilterBinder(objectInfo);
                filterBinder.Bind(filterQueryOption);
                filterBinder.predicateBuilder.ApplyTo(whereSqlBuilder);
            }

            return whereSqlBuilder;
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.BinaryOperatorNode" />.
        /// </summary>
        /// <param name="binaryOperatorNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.BinaryOperatorNode" /> to bind.</param>
        protected override void Bind(BinaryOperatorNode binaryOperatorNode)
        {
            if (binaryOperatorNode == null)
            {
                throw new ArgumentNullException(nameof(binaryOperatorNode));
            }

            this.predicateBuilder.Append("(");

            this.Bind(binaryOperatorNode.Left);

            // ignore 'eq true' or 'eq false' for method calls
            if (!(binaryOperatorNode.Left.Kind == QueryNodeKind.FunctionCall
                && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal
                && binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                && ((ConstantNode)binaryOperatorNode.Right).EdmType == EdmPrimitiveType.Boolean))
            {
                if (binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                    && ((ConstantNode)binaryOperatorNode.Right).EdmType == null)
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
        protected override void Bind(ConstantNode constantNode)
        {
            if (constantNode == null)
            {
                throw new ArgumentNullException(nameof(constantNode));
            }

            if (constantNode.EdmType == null)
            {
                this.predicateBuilder.Append("NULL");
            }
            else
            {
                this.predicateBuilder.Append(this.sqlCharacters.GetParameterName(0), constantNode.Value);
            }
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.FunctionCallNode" />.
        /// </summary>
        /// <param name="functionCallNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.FunctionCallNode" /> to bind.</param>
        protected override void Bind(FunctionCallNode functionCallNode)
        {
            if (functionCallNode == null)
            {
                throw new ArgumentNullException(nameof(functionCallNode));
            }

            var parameters = functionCallNode.Parameters;

            switch (functionCallNode.Name)
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
                    var name = functionCallNode.Name.StartsWith("to", StringComparison.Ordinal)
                        ? functionCallNode.Name.Substring(2)
                        : functionCallNode.Name;

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

                case "trim":
                    this.predicateBuilder.Append("LTRIM(RTRIM(");
                    this.Bind(parameters[0]);
                    this.predicateBuilder.Append("))");
                    break;

                case "contains":
                    this.Bind(parameters[0]);
                    this.predicateBuilder.Append(
                        " LIKE " + this.sqlCharacters.GetParameterName(0),
                        this.sqlCharacters.LikeWildcard + ((ConstantNode)parameters[1]).Value + this.sqlCharacters.LikeWildcard);
                    break;

                default:
                    throw new NotImplementedException($"The function '{functionCallNode.Name}' is not implemented by this service");
            }
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.PropertyAccessNode" />.
        /// </summary>
        /// <param name="propertyAccessNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.PropertyAccessNode" /> to bind.</param>
        protected override void Bind(PropertyAccessNode propertyAccessNode)
        {
            if (propertyAccessNode == null)
            {
                throw new ArgumentNullException(nameof(propertyAccessNode));
            }

            var column = this.objectInfo.TableInfo.GetColumnInfoForProperty(propertyAccessNode.Property.Name);

            if (column == null)
            {
                throw new InvalidOperationException($"The type '{this.objectInfo.ForType.Name}' does not contain a property named '{propertyAccessNode.Property.Name}'");
            }

            this.predicateBuilder.Append(column.ColumnName);
        }

        /// <summary>
        /// Binds the specified <see cref="T:Net.Http.WebApi.OData.Query.Expressions.UnaryOperatorNode" />.
        /// </summary>
        /// <param name="unaryOperatorNode">The <see cref="T:Net.Http.WebApi.OData.Query.Expressions.UnaryOperatorNode" /> to bind.</param>
        protected override void Bind(UnaryOperatorNode unaryOperatorNode)
        {
            if (unaryOperatorNode == null)
            {
                throw new ArgumentNullException(nameof(unaryOperatorNode));
            }

            this.predicateBuilder.Append(unaryOperatorNode.OperatorKind.ToSqlOperator() + " ");
            this.Bind(unaryOperatorNode.Operand);
        }
    }
}