namespace MicroLite.Extensions.WebApi.Tests.Query
{
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Query;
    using Xunit;

    public class ODataQueryOptionsTests
    {
        public class WhenCreated
        {
            private readonly HttpRequestMessage httpRequestMessage;
            private readonly ODataQueryOptions option;

            public WhenCreated()
            {
                this.httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get,
                    "http://localhost/api");

                this.option = new ODataQueryOptions(this.httpRequestMessage);
            }

            [Fact]
            public void TheRawValuesPropertyShouldBeSet()
            {
                Assert.NotNull(this.option.RawValues);
            }

            [Fact]
            public void TheRequestPropertyShouldReturnTheRequestMessage()
            {
                Assert.Equal(this.httpRequestMessage, this.option.Request);
            }
        }
    }
}