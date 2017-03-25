namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System.Net.Http;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
    using Net.Http.WebApi.OData.Model;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class ODataQueryOptionExtensionsTests
    {
        [Fact]
        public void CreateSqlQueryBindsSelectThenAddsFilterAndOrderBy()
        {
            TestHelper.EnsureEDM();

            var option = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/api/Customers?$select=Forename,Surname&$filter=Forename eq 'John'&$orderby=Surname"),
                EntityDataModel.Current.Collections["Customers"]);

            var sqlQuery = option.CreateSqlQuery();

            var expected = SqlBuilder.Select("Forename", "Surname").From(typeof(Customer)).Where("(Forename = ?)", "John").OrderByAscending("Surname").ToSqlQuery();

            Assert.Equal(expected, sqlQuery);
        }

        [Fact]
        public void CreateSqlQueryBindsSelectWildcardThenAddsFilterAndOrderBy()
        {
            TestHelper.EnsureEDM();

            var option = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/api/Customers?$filter=Forename eq 'John'&$orderby=Surname"),
                EntityDataModel.Current.Collections["Customers"]);

            var sqlQuery = option.CreateSqlQuery();

            var expected = SqlBuilder.Select("*").From(typeof(Customer)).Where("(Forename = ?)", "John").OrderByAscending("Surname").ToSqlQuery();

            Assert.Equal(expected, sqlQuery);
        }
    }
}