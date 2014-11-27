namespace MicroLite.Extensions.WebApi.Tests.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using MicroLite.Extensions.WebApi.OData;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
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

#if NET_4_0
            var exception = Assert.Throws<ODataException>(() => controller.Get(queryOptions));
            Assert.Contains("$skip", exception.Message);
#else
            var exception = Assert.Throws<AggregateException>(() => controller.Get(queryOptions).Result);
            Assert.IsType<ODataException>(exception.InnerException);
            Assert.Contains("$skip", exception.InnerException.Message);
#endif
        }

        public class TheDefaultValidatonSettings
        {
            private readonly CustomerController controller = new CustomerController();

            [Fact]
            public void CeilingFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Ceiling, AllowedFunctions.Ceiling);
            }

            [Fact]
            public void DayFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Day, AllowedFunctions.Day);
            }

            [Fact]
            public void EndsWithFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.EndsWith, AllowedFunctions.EndsWith);
            }

            [Fact]
            public void ExpandQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Expand, AllowedQueryOptions.Expand);
            }

            [Fact]
            public void FilterQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Filter, AllowedQueryOptions.Filter);
            }

            [Fact]
            public void FloorFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Floor, AllowedFunctions.Floor);
            }

            [Fact]
            public void FormatCountQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Format, AllowedQueryOptions.Format);
            }

            [Fact]
            public void HourFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Hour, AllowedFunctions.Hour);
            }

            [Fact]
            public void IndexOfFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.IndexOf, AllowedFunctions.IndexOf);
            }

            [Fact]
            public void InlineCountQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.InlineCount, AllowedQueryOptions.InlineCount);
            }

            [Fact]
            public void MaxTopIsSetTo50()
            {
                Assert.Equal(50, controller.ValidationSettings.MaxTop);
            }

            [Fact]
            public void MinuteFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Minute, AllowedFunctions.Minute);
            }

            [Fact]
            public void MonthFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Month, AllowedFunctions.Month);
            }

            [Fact]
            public void OrderByQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.OrderBy, AllowedQueryOptions.OrderBy);
            }

            [Fact]
            public void ReplaceFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Replace, AllowedFunctions.Replace);
            }

            [Fact]
            public void RoundFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Round, AllowedFunctions.Round);
            }

            [Fact]
            public void SecondFunctionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Second, AllowedFunctions.Second);
            }

            [Fact]
            public void SelectQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Select, AllowedQueryOptions.Select);
            }

            [Fact]
            public void SkipQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Skip, AllowedQueryOptions.Skip);
            }

            [Fact]
            public void SkipTokenQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.SkipToken, AllowedQueryOptions.SkipToken);
            }

            [Fact]
            public void StartsWithFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.StartsWith, AllowedFunctions.StartsWith);
            }

            [Fact]
            public void SubstringFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Substring, AllowedFunctions.Substring);
            }

            [Fact]
            public void SubstringOfFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.SubstringOf, AllowedFunctions.SubstringOf);
            }

            [Fact]
            public void ToLowerFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToLower, AllowedFunctions.ToLower);
            }

            [Fact]
            public void TopQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Top, AllowedQueryOptions.Top);
            }

            [Fact]
            public void ToUpperFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToUpper, AllowedFunctions.ToUpper);
            }

            [Fact]
            public void YearFunctionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Year, AllowedFunctions.Year);
            }
        }

        public class WhenAValidSkipValueIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$skip=15"));

            public WhenAValidSkipValueIsSpecified()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 50, 0));
#else
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 50, 0)));
#endif

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
#if NET_4_0
                this.mockSession.Verify(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(this.queryOptions.Skip.Value, 50)));
#else
                this.mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(this.queryOptions.Skip.Value, 50)));
#endif
            }
        }

        public class WhenAValidTopValueIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$top=15"));

            public WhenAValidTopValueIsSpecified()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 15, 0));
#else
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 15, 0)));
#endif

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
#if NET_4_0
                this.mockSession.Verify(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(0, this.queryOptions.Top.Value)));
#else
                this.mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(0, this.queryOptions.Top.Value)));
#endif
            }
        }

        public class WhenConstructedUsingTheDefaultConstructor
        {
            private readonly MicroLiteODataApiController<Customer, int> controller;

            public WhenConstructedUsingTheDefaultConstructor()
            {
                var mockController = new Mock<MicroLiteODataApiController<Customer, int>>();
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsNull()
            {
                Assert.Null(this.controller.Session);
            }
        }

        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteODataApiController<Customer, int> controller;
#if NET_4_0
            private readonly ISession session = new Mock<ISession>().Object;
#else
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;
#endif

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteODataApiController<Customer, int>>(this.session);
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(this.session, this.controller.Session);
            }
        }

        public class WhenFormatQueryOptionIsSpecified
        {
            private readonly CustomerController controller = new CustomerController();
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$format=application/xml"));
            private readonly HttpResponseMessage response;

            public WhenFormatQueryOptionIsSpecified()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 50, 0));
#else
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 50, 0)));
#endif

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;
#if NET_4_0
                this.response = this.controller.Get(this.queryOptions);
#else
                this.response = this.controller.Get(this.queryOptions).Result;
#endif
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
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$inlinecount=allpages"));
            private readonly HttpResponseMessage response;

            public WhenInlineCountAllPagesIsSpecified()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new object[0], 50, 0));
#else
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 50, 0)));
#endif

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Get(this.queryOptions);
#else
                this.response = this.controller.Get(this.queryOptions).Result;
#endif
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
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api"));
            private readonly HttpResponseMessage response;

            public WhenInlineCountIsNotSpecified()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Paged<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(new PagedResult<dynamic>(0, new List<object>(), 50, 0));
#else
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new List<object>(), 50, 0)));
#endif

                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Get(this.queryOptions);
#else
                this.response = this.controller.Get(this.queryOptions).Result;
#endif
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

#if NET_4_0
                var exception = Assert.Throws<ArgumentNullException>(() => controller.Get(queryOptions));
                Assert.Equal("queryOptions", exception.ParamName);
#else
                var exception = Assert.Throws<AggregateException>(() => controller.Get(queryOptions).Result);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
                Assert.Contains("queryOptions", exception.InnerException.Message);
#endif
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

            public new ODataValidationSettings ValidationSettings
            {
                get
                {
                    return base.ValidationSettings;
                }
            }

#if NET_4_0

            public HttpResponseMessage Get(ODataQueryOptions queryOptions)
            {
                return this.GetEntityResponse(queryOptions);
            }

#else

            public System.Threading.Tasks.Task<HttpResponseMessage> Get(ODataQueryOptions queryOptions)
            {
                return this.GetEntityResponseAsync(queryOptions);
            }

#endif
        }
    }
}