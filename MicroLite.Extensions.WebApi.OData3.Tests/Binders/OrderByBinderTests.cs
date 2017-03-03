namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using System.Net.Http;
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
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$orderby=FirstName"));

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, null, SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BindOrderByThrowsArgumentNullExceptionForNullOrderBySqlBuilder()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$orderby=FirstName"));

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, ObjectInfo.For(typeof(Customer)), null));

            Assert.Equal("orderBySqlBuilder", exception.ParamName);
        }

        [Fact]
        public void BindOrderByThrowsInvalidOperationExceptionForUnspportedPropertyName()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$orderby=FirstName"));

            var exception = Assert.Throws<InvalidOperationException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("The type 'Customer' does not contain a property named 'FirstName'", exception.Message);
        }

        public class WhenCallingBindOrderBy
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindOrderBy()
            {
                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$orderby=Status desc,Name"));

                this.sqlQuery = OrderByBinder.BindOrderBy(
                    queryOptions.OrderBy,
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