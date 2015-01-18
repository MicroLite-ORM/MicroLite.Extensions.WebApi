namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using MicroLite.Extensions.WebApi.OData.Binders;
    using Net.Http.WebApi.OData.Query.Expressions;
    using Xunit;

    public class UnaryOperatorKindExtensionsTests
    {
        [Fact]
        public void ToSqlOperatorReturnsNotForUnaryOperatorKindNot()
        {
            Assert.Equal("NOT", UnaryOperatorKind.Not.ToSqlOperator());
        }
    }
}