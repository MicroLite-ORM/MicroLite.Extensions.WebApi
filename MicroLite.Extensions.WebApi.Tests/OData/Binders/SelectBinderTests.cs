namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using System.Net.Http;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class SelectBinderTests
    {
        [Fact]
        public void BindBindSelectThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers"));

            var exception = Assert.Throws<ArgumentNullException>(
                () => SelectBinder.BindSelect(queryOptions.Select, null));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        public void BindSelectQueryOptionThrowsODataExceptionForUnspportedPropertyName()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Customers?$select=FirstName"));

            var exception = Assert.Throws<ODataException>(
                () => SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))));

            Assert.Equal(string.Format(Messages.InvalidPropertyName, "Customer", "FirstName"), exception.Message);
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

                this.sqlQuery = SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))).ToSqlQuery();
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

                this.sqlQuery = SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))).ToSqlQuery();
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

                this.sqlQuery = SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            public void AllPropertiesOnTheMappedTypeShouldBeIncluded()
            {
                var expected = SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery();

                Assert.Equal(expected, this.sqlQuery);
            }
        }
    }
}