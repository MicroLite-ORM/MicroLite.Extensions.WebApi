namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class OrderByBinderTests
    {
        private enum CustomerStatus
        {
            Pending = 0,
            Active = 1
        }

        [Fact]
        public void BindOrderByThrowsArgumentNullExceptionForNullOrderBySqlBuilder()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy<Customer>(new OrderByQueryOption("$orderby=FirstName"), null));

            Assert.Equal("orderBySqlBuilder", exception.ParamName);
        }

        [Fact]
        public void BindOrderByThrowsODataExceptionForUnspportedPropertyName()
        {
            var exception = Assert.Throws<ODataException>(() => OrderByBinder.BindOrderBy<Customer>(new OrderByQueryOption("$orderby=FirstName"), SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal(string.Format(Messages.InvalidPropertyName, "Customer", "FirstName"), exception.Message);
        }

        public class WhenCallingBindOrderBy
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindOrderBy()
            {
                this.sqlQuery = OrderByBinder.BindOrderBy<Customer>(new OrderByQueryOption("$orderby=Status desc,Name"), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
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

        private class Customer
        {
            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public CustomerStatus Status
            {
                get;
                set;
            }
        }
    }
}