// -----------------------------------------------------------------------
// <copyright file="UnaryOperatorKindExtensions.cs" company="MicroLite">
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

    internal static class UnaryOperatorKindExtensions
    {
        internal static string ToSqlOperator(this UnaryOperatorKind unaryOperatorKind)
        {
            switch (unaryOperatorKind)
            {
                case UnaryOperatorKind.Not:
                    return "NOT";

                default:
                    throw new NotImplementedException($"The operator '{unaryOperatorKind}' is not implemented by this service");
            }
        }
    }
}