// -----------------------------------------------------------------------
// <copyright file="BinaryOperatorKindExtensions.cs" company="MicroLite">
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
    using Net.Http.WebApi.OData.Query.Expressions;

    internal static class BinaryOperatorKindExtensions
    {
        internal static string ToSqlOperator(this BinaryOperatorKind binaryOperatorKind)
        {
            switch (binaryOperatorKind)
            {
                case BinaryOperatorKind.Add:
                    return "+";

                case BinaryOperatorKind.And:
                    return "AND";

                case BinaryOperatorKind.Divide:
                    return "/";

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

                case BinaryOperatorKind.Modulo:
                    return "%";

                case BinaryOperatorKind.Multiply:
                    return "*";

                case BinaryOperatorKind.NotEqual:
                    return "<>";

                case BinaryOperatorKind.Or:
                    return "OR";

                case BinaryOperatorKind.Subtract:
                    return "-";

                default:
                    throw new NotImplementedException($"The operator '{binaryOperatorKind}' is not implemented by this service");
            }
        }
    }
}