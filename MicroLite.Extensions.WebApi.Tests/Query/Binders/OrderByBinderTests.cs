namespace MicroLite.Extensions.WebApi.Tests.Query.Binders
{
    using System;
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Query;
    using MicroLite.Extensions.WebApi.Query.Binders;
    using MicroLite.Query;
    using Xunit;

    public class OrderByBinderTests
    {
        private enum CustomerStatus
        {
            Pending = 0,
            Active = 1
        }

        public class WhenCallingBindOrderBy
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindOrderBy()
            {
                this.sqlQuery = OrderByBinder.BindOrderBy<Customer>(
                    SqlBuilder.Select("*").From(typeof(Customer)),
                    new OrderByQueryOption("$orderby=Status desc,Name")).ToSqlQuery();
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