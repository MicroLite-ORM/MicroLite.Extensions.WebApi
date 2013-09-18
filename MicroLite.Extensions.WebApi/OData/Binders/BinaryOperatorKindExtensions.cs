// -----------------------------------------------------------------------
// <copyright file="BinaryOperatorKindExtensions.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query.Expression;

    internal static class BinaryOperatorKindExtensions
    {
        internal static string ToSqlOperator(this BinaryOperatorKind binaryOperator)
        {
            switch (binaryOperator)
            {
                case BinaryOperatorKind.And:
                    return "AND";

                case BinaryOperatorKind.Equal:
                    return "=";

                case BinaryOperatorKind.GreaterThan:
                    return ">";

                case BinaryOperatorKind.GreaterThanOrEqual:
                    return ">=";

                case BinaryOperatorKind.LessThan:
                    return "<";

                case BinaryOperatorKind.LessThanOrEqual:
                    return "<=";

                case BinaryOperatorKind.NotEqual:
                    return "<>";

                case BinaryOperatorKind.Or:
                    return "OR";

                default:
                    throw new ODataException("The operator '" + binaryOperator.ToString() + "' is not supported");
            }
        }
    }
}