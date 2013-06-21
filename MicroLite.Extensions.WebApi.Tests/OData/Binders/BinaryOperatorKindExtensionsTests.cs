namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using MicroLite.Extensions.WebApi.OData;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Expression;
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
        public void ToSqlOperatorThrowsODataExceptionForBinaryOperatorKindNone()
        {
            Assert.Throws<ODataException>(() => BinaryOperatorKind.None.ToSqlOperator());
        }
    }
}