namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using Net.Http.WebApi.OData.Query.Expressions;
    using Xunit;

    public class BinaryOperatorKindExtensionsTests
    {
        [Fact]
        public void ToSqlOperatorReturnsAndForBinaryOperatorKindAnd()
        {
            Assert.Equal("AND", BinaryOperatorKind.And.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsEqualsForBinaryOperatorKindEqual()
        {
            Assert.Equal("=", BinaryOperatorKind.Equal.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsForwardSlashForBinaryOperatorKindDivide()
        {
            Assert.Equal("/", BinaryOperatorKind.Divide.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsGreaterThanForBinaryOperatorKindGreaterThan()
        {
            Assert.Equal(">", BinaryOperatorKind.GreaterThan.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsGreaterThanOrEqualForBinaryOperatorKindGreaterThanOrEqual()
        {
            Assert.Equal(">=", BinaryOperatorKind.GreaterThanOrEqual.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsLessThanForBinaryOperatorKindLessThan()
        {
            Assert.Equal("<", BinaryOperatorKind.LessThan.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsLessThanOrEqualForBinaryOperatorKindLessThanOrEqual()
        {
            Assert.Equal("<=", BinaryOperatorKind.LessThanOrEqual.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsMinusForBinaryOperatorKindSubtract()
        {
            Assert.Equal("-", BinaryOperatorKind.Subtract.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsNotEqualForBinaryOperatorKindNotEqual()
        {
            Assert.Equal("<>", BinaryOperatorKind.NotEqual.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsOrForBinaryOperatorKindOr()
        {
            Assert.Equal("OR", BinaryOperatorKind.Or.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsPercentForBinaryOperatorKindModulo()
        {
            Assert.Equal("%", BinaryOperatorKind.Modulo.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsPlusForBinaryOperatorKindAdd()
        {
            Assert.Equal("+", BinaryOperatorKind.Add.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorReturnsStarForBinaryOperatorKindMultiply()
        {
            Assert.Equal("*", BinaryOperatorKind.Multiply.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorThrowsNotImplementedExceptionForBinaryOperatorKindNone()
        {
            var exception = Assert.Throws<NotImplementedException>(() => BinaryOperatorKind.None.ToSqlOperator());

            Assert.Equal("The operator 'None' is not implemented by this service", exception.Message);
        }
    }
}