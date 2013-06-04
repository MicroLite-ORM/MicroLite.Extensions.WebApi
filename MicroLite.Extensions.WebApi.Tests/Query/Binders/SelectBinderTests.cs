namespace MicroLite.Extensions.WebApi.Tests.Query.Binders
{
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Query;
    using MicroLite.Extensions.WebApi.Query.Binders;
    using MicroLite.Query;
    using Xunit;

    public class SelectBinderTests
    {
        private enum CustomerStatus
        {
            Pending = 0,
            Active = 1
        }

        public class WhenCallingBindSelectQueryOptionAndNoPropertiesHaveBeenSpecified
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindSelectQueryOptionAndNoPropertiesHaveBeenSpecified()
            {
                var httpRequestMessage = new HttpRequestMessage(
                       HttpMethod.Get,
                       "http://localhost/api");

                var queryOptions = new ODataQueryOptions(httpRequestMessage);

                this.sqlQuery = SelectBinder.BindSelectQueryOption<Customer>(queryOptions).ToSqlQuery();
            }

            [Fact]
            public void AllPropertiesOnTheMappedTypeShouldBeIncluded()
            {
                var expected = SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery();

                Assert.Equal(expected, this.sqlQuery);
            }
        }

        public class WhenCallingBindSelectQueryOptionAndSpecificPropertiesHaveBeenSpecified
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindSelectQueryOptionAndSpecificPropertiesHaveBeenSpecified()
            {
                var httpRequestMessage = new HttpRequestMessage(
                       HttpMethod.Get,
                       "http://localhost/api?$select=Name,Status");

                var queryOptions = new ODataQueryOptions(httpRequestMessage);

                this.sqlQuery = SelectBinder.BindSelectQueryOption<Customer>(queryOptions).ToSqlQuery();
            }

            [Fact]
            public void TheColumnNamesForTheSpecifiedPropertiesShouldBeTheOnlyOnesInTheSelectList()
            {
                var expected = SqlBuilder.Select("Name", "CustomerStatusId").From(typeof(Customer)).ToSqlQuery();

                Assert.Equal(expected, this.sqlQuery);
            }
        }

        public class WhenCallingBindSelectQueryOptionAndStarHasBeenSpecified
        {
            private readonly SqlQuery sqlQuery;

            public WhenCallingBindSelectQueryOptionAndStarHasBeenSpecified()
            {
                var httpRequestMessage = new HttpRequestMessage(
                       HttpMethod.Get,
                       "http://localhost/api?$select=*");

                var queryOptions = new ODataQueryOptions(httpRequestMessage);

                this.sqlQuery = SelectBinder.BindSelectQueryOption<Customer>(queryOptions).ToSqlQuery();
            }

            [Fact]
            public void AllPropertiesOnTheMappedTypeShouldBeIncluded()
            {
                var expected = SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery();

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