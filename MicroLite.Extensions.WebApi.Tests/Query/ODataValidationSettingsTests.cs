namespace MicroLite.Extensions.WebApi.Tests.Query
{
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Query;
    using MicroLite.Extensions.WebApi.Query.Validation;
    using Xunit;

    public class ODataValidationSettingsTests
    {
        public class WhenValidatingAndAMaxTopIsSetButTheQueryOptionDoesNotSpecifyATopValue
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                MaxTop = 100
            };

            [Fact]
            public void NoExceptionIsThrown()
            {
                Assert.DoesNotThrow(() => this.validationSettings.Validate(this.queryOptions));
            }
        }

        public class WhenValidatingAndAMaxTopIsSetWhichTheQueryOptionDoesNotExceed
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$top=25"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                MaxTop = 100
            };

            [Fact]
            public void NoExceptionIsThrown()
            {
                Assert.DoesNotThrow(() => this.validationSettings.Validate(this.queryOptions));
            }
        }

        public class WhenValidatingAndAMaxTopIsSetWhichTheQueryOptionExceeds
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$top=150"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                MaxTop = 100
            };

            [Fact]
            public void AnExceptionIsThrown()
            {
                Assert.Throws<ODataException>(() => this.validationSettings.Validate(this.queryOptions));
            }
        }

        public class WhenValidatingAndNoMaxTopIsSet
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$top=150"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                MaxTop = null
            };

            [Fact]
            public void NoExceptionIsThrown()
            {
                Assert.DoesNotThrow(() => this.validationSettings.Validate(this.queryOptions));
            }
        }
    }
}