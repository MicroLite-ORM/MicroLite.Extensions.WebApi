namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Query;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class ODataQueryOptionExtensionsTests
    {
        [Fact]
        public void CreateSqlQueryBindsSelectThenAddsFilterAndOrderBy()
        {
            var httpRequestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                "http://localhost/api?$filter=Forename eq 'John'&$orderby=Surname");

            var option = new ODataQueryOptions(httpRequestMessage);

            var sqlQuery = option.CreateSqlQuery<Customer>();

            var expected = SqlBuilder.Select("*").From(typeof(Customer)).Where("(Forename = @p0)", "John").OrderByAscending("Surname").ToSqlQuery();

            Assert.Equal(expected, sqlQuery);
        }

        public class Customer
        {
            public string Forename
            {
                get;
                set;
            }

            public int Id
            {
                get;
                set;
            }

            public string Surname
            {
                get;
                set;
            }
        }
    }
}