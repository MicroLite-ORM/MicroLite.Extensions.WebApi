namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using System.Net.Http;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData.Model;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class OrderByBinderTests
    {
        public OrderByBinderTests()
        {
            TestHelper.EnsureEDM();
        }

        [Fact]
        public void BindOrderByThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/api/Customers?$orderby=Name"),
                EntityDataModel.Current.EntitySets["Customers"]);

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, null, SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BindOrderByThrowsArgumentNullExceptionForNullOrderBySqlBuilder()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/api/Customers?$orderby=Name"),
                EntityDataModel.Current.EntitySets["Customers"]);

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, ObjectInfo.For(typeof(Customer)), null));

            Assert.Equal("orderBySqlBuilder", exception.ParamName);
        }

        public class WhenCallingBindOrderBy_WithAnOrderByQueryOption
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindOrderBy_WithAnOrderByQueryOption()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        "http://services.microlite.org/api/Customers?$orderby=Status desc,Name"),
                    EntityDataModel.Current.EntitySets["Customers"]);

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

        public class WhenCallingBindOrderBy_WithoutAnOrderByQueryOption
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindOrderBy_WithoutAnOrderByQueryOption()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        "http://services.microlite.org/api/Customers"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.sqlQuery = OrderByBinder.BindOrderBy(
                    queryOptions.OrderBy,
                    ObjectInfo.For(typeof(Customer)),
                    SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void TheQueryShouldBeSortedByTheIdAscending()
            {
                var expected = SqlBuilder
                    .Select("*")
                    .From(typeof(Customer))
                    .OrderByAscending("Id")
                    .ToSqlQuery();

                Assert.Equal(expected, this.sqlQuery);
            }
        }
    }
}