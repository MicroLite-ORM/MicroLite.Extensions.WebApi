namespace MicroLite.Extensions.WebApi.Tests.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using MicroLite.Extensions.WebApi.OData;
    using Moq;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Xunit;

    public class MicroLiteODataApiControllerTests
    {
        [Fact]
        public void WhenCallingGetEntityResponseTheODataQueryOptionsAreValidated()
        {
            var queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$skip=-1"));

            var controller = new CustomerController();

            var exception = Assert.Throws<ODataException>(() => controller.Get(queryOptions));
            Assert.Contains("$skip", exception.Message);
        }

        public class WhenAValidSkipValueIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$skip=15"));

            public WhenAValidSkipValueIsSpecified()
            {
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 50, 0));

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
                this.mockSession.Verify(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(this.queryOptions.Skip.Value, 50)));
            }
        }

        public class WhenAValidTopValueIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$top=15"));

            public WhenAValidTopValueIsSpecified()
            {
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 15, 0));

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
                this.mockSession.Verify(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(0, this.queryOptions.Top.Value)));
            }
        }

        public class WhenFormatQueryOptionIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$format=application/xml"));
            private readonly HttpResponseMessage response;

            public WhenFormatQueryOptionIsSpecified()
            {
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 50, 0));

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheResponseContentTypeHeaderIsSet()
            {
                Assert.Equal(this.queryOptions.Format.MediaTypeHeaderValue, response.Content.Headers.ContentType);
            }
        }

        public class WhenInlineCountAllPagesIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$inlinecount=allpages"));
            private readonly HttpResponseMessage response;

            public WhenInlineCountAllPagesIsSpecified()
            {
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 50, 0));

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheResponseIsAnInlineCountCollection()
            {
                Assert.IsType<InlineCountCollection<dynamic>>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenInlineCountIsNotSpecified
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api"));
            private readonly HttpResponseMessage response;

            public WhenInlineCountIsNotSpecified()
            {
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new List<object>(), 50, 0));

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheResponseIsAnList()
            {
                Assert.IsType<List<dynamic>>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenNullQueryOptionsAreSupplied
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var controller = new CustomerController();

                var queryOptions = default(ODataQueryOptions);

                var exception = Assert.Throws<ArgumentNullException>(() => controller.Get(queryOptions));

                Assert.Equal("queryOptions", exception.ParamName);
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
        }

        private class CustomerController : MicroLiteODataApiController<Customer, int>
        {
            public CustomerController()
            {
                this.GetEntityResourceUri = (int id) =>
                {
                    return new Uri("http://localhost/api/Customers/" + id.ToString());
                };
            }

            public HttpResponseMessage Get(ODataQueryOptions queryOptions)
            {
                return this.GetEntityResponse(queryOptions);
            }
        }
    }
}