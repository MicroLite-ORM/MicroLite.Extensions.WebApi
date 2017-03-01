namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class OrderByBinderTests
    {
        [Fact]
        public void BindOrderByThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(new OrderByQueryOption("$orderby=FirstName"), null, SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BindOrderByThrowsArgumentNullExceptionForNullOrderBySqlBuilder()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(new OrderByQueryOption("$orderby=FirstName"), ObjectInfo.For(typeof(Customer)), null));

            Assert.Equal("orderBySqlBuilder", exception.ParamName);
        }

        [Fact]
        public void BindOrderByThrowsInvalidOperationExceptionForUnspportedPropertyName()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => OrderByBinder.BindOrderBy(new OrderByQueryOption("$orderby=FirstName"), ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("The type 'Customer' does not contain a property named 'FirstName'", exception.Message);
        }

        public class WhenCallingBindOrderBy
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindOrderBy()
            {
                this.sqlQuery = OrderByBinder.BindOrderBy(
                    new OrderByQueryOption("$orderby=Status desc,Name"),
                    ObjectInfo.For(typeof(Customer)),
                    SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheColumnNamesForTheSpecifiedPropertiesShouldBeSetInTheOrderByClause()
            {
                var expected = SqlBuilder
                    .Select("*")
                    .From(typeof(Customer))
                    .OrderByDescending("CustomerStatusId")
                    .OrderByAscending("Name")
                    .ToSqlQuery();

                Assert.Equal(expected, this.sqlQuery);
            }
        }
    }
}