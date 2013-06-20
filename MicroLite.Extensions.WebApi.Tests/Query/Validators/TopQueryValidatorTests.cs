namespace MicroLite.Extensions.WebApi.Tests.Query.Validation
{
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Query;
    using MicroLite.Extensions.WebApi.Query.Validators;
    using Xunit;

    public class TopQueryValidatorTests
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
                Assert.DoesNotThrow(() => new TopQueryValidator().Validate(this.queryOptions, this.validationSettings));
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
                Assert.DoesNotThrow(() => new TopQueryValidator().Validate(this.queryOptions, this.validationSettings));
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
                var exception = Assert.Throws<ODataException>(() => new TopQueryValidator().Validate(this.queryOptions, this.validationSettings));
                Assert.Equal(string.Format(Messages.TopValueExceedsMaxAllowed, 100), exception.Message);
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
                Assert.DoesNotThrow(() => new TopQueryValidator().Validate(this.queryOptions, this.validationSettings));
            }
        }
    }
}