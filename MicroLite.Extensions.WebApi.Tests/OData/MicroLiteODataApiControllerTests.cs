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

            var controller = new CustomerController(Mock.Of<IAsyncSession>());

            var exception = Assert.Throws<AggregateException>(() => controller.Get(queryOptions).Result);
            Assert.IsType<ODataException>(exception.InnerException);
            Assert.Contains("$skip", exception.InnerException.Message);
        }

        public class TheDefaultValidatonSettings
        {
            private readonly CustomerController controller = new CustomerController(Mock.Of<IAsyncSession>());

            [Fact]
            public void AllArithmeticOperatorsAreAllowed()
            {
                Assert.Equal(AllowedArithmeticOperators.All, controller.ValidationSettings.AllowedArithmeticOperators & AllowedArithmeticOperators.All);
            }

            [Fact]
            public void AllLogicalOperatorsAreAllowed()
            {
                Assert.Equal(AllowedLogicalOperators.All, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.All);
            }

            [Fact]
            public void TrimFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Trim, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Trim);
            }

            [Fact]
            public void ConcatFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Concat, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Concat);
            }

            [Fact]
            public void LengthFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Length, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Length);
            }

            [Fact]
            public void CeilingFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Ceiling, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Ceiling);
            }

            [Fact]
            public void DayFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Day, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Day);
            }

            [Fact]
            public void EndsWithFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.EndsWith, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.EndsWith);
            }

            [Fact]
            public void ExpandQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(AllowedQueryOptions.Expand, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Expand);
            }

            [Fact]
            public void FilterQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Filter, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Filter);
            }

            [Fact]
            public void FloorFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Floor, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Floor);
            }

            [Fact]
            public void FormatCountQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Format, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Format);
            }

            [Fact]
            public void HourFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Hour, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Hour);
            }

            [Fact]
            public void IndexOfFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.IndexOf, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.IndexOf);
            }

            [Fact]
            public void InlineCountQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.InlineCount, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.InlineCount);
            }

            [Fact]
            public void MaxTopIsSetTo50()
            {
                Assert.Equal(50, controller.ValidationSettings.MaxTop);
            }

            [Fact]
            public void MinuteFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Minute, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Minute);
            }

            [Fact]
            public void MonthFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Month, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Month);
            }

            [Fact]
            public void OrderByQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.OrderBy, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.OrderBy);
            }

            [Fact]
            public void ReplaceFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Replace, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Replace);
            }

            [Fact]
            public void RoundFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Round, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Round);
            }

            [Fact]
            public void SecondFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Second, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Second);
            }

            [Fact]
            public void SelectQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Select, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Select);
            }

            [Fact]
            public void SkipQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Skip, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Skip);
            }

            [Fact]
            public void SkipTokenQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(AllowedQueryOptions.SkipToken, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.SkipToken);
            }

            [Fact]
            public void StartsWithFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.StartsWith, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.StartsWith);
            }

            [Fact]
            public void SubstringFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Substring, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Substring);
            }

            [Fact]
            public void SubstringOfFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.SubstringOf, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.SubstringOf);
            }

            [Fact]
            public void ToLowerFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.ToLower, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToLower);
            }

            [Fact]
            public void TopQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Top, AllowedQueryOptions.Top);
            }

            [Fact]
            public void ToUpperFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.ToUpper, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToUpper);
            }

            [Fact]
            public void YearFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Year, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Year);
            }
        }

        public class WhenAValidSkipValueIsSpecified
        {
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$skip=15"));

            public WhenAValidSkipValueIsSpecified()
            {
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 50, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
                this.mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(this.queryOptions.Skip.Value, 50)));
            }
        }

        public class WhenAValidTopValueIsSpecified
        {
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$top=15"));

            public WhenAValidTopValueIsSpecified()
            {
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 15, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
                this.mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(0, this.queryOptions.Top.Value)));
            }
        }
        
        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteODataApiController<Customer, int> controller;
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;

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
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$format=application/xml"));
            private readonly HttpResponseMessage response;

            public WhenFormatQueryOptionIsSpecified()
            {
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 50, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;
                this.response = this.controller.Get(this.queryOptions).Result;
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
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$inlinecount=allpages"));
            private readonly HttpResponseMessage response;

            public WhenInlineCountAllPagesIsSpecified()
            {
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new object[0], 50, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheResponseIsAnInlineCount()
            {
                Assert.IsType<InlineCount<dynamic>>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenInlineCountIsNotSpecified
        {
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(new HttpRequestMessage(HttpMethod.Get, "http://localhost/api"));
            private readonly HttpResponseMessage response;

            public WhenInlineCountIsNotSpecified()
            {
                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(System.Threading.Tasks.Task.FromResult(new PagedResult<dynamic>(0, new List<object>(), 50, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
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
                var controller = new CustomerController(Mock.Of<IAsyncSession>());

                var queryOptions = default(ODataQueryOptions);

                var exception = Assert.Throws<AggregateException>(() => controller.Get(queryOptions).Result);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
                Assert.Contains("queryOptions", exception.InnerException.Message);
            }
        }

        private class CustomerController : MicroLiteODataApiController<Customer, int>
        {
            public CustomerController(IAsyncSession session)
                : base(session)
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

            public System.Threading.Tasks.Task<HttpResponseMessage> Get(ODataQueryOptions queryOptions)
            {
                return this.GetEntityResponseAsync(queryOptions);
            }
        }
    }
}