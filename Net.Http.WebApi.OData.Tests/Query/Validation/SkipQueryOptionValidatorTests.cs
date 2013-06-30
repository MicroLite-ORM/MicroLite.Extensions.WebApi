namespace Net.Http.WebApi.OData.Tests.Query.Validation
{
    using System.Net.Http;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Validation;
    using Xunit;

    public class SkipQueryOptionValidatorTests
    {
        public class WhenValidatingAndTheValueIsAboveZero
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$skip=10"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                MaxTop = null
            };

            [Fact]
            public void NoExceptionIsThrown()
            {
                Assert.DoesNotThrow(() => SkipQueryOptionValidator.Validate(this.queryOptions, this.validationSettings));
            }
        }

        public class WhenValidatingAndTheValueIsBelowZero
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$skip=-10"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                MaxTop = null
            };

            [Fact]
            public void AnExceptionIsThrown()
            {
                Assert.Throws<ODataException>(() => SkipQueryOptionValidator.Validate(this.queryOptions, this.validationSettings));
            }
        }
    }
}