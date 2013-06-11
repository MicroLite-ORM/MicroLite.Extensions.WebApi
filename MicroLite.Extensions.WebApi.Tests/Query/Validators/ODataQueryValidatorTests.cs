namespace MicroLite.Extensions.WebApi.Tests.Query.Validators
{
    using System.Net.Http;
    using MicroLite.Extensions.WebApi.Query;
    using MicroLite.Extensions.WebApi.Query.Validators;
    using Xunit;

    public class ODataQueryValidatorTests
    {
        public class WhenTheFormatQueryOptionIsSetAndItIsNotSpecifiedInAllowedQueryOptions
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$format=xml"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                AllowedQueryOptions = AllowedQueryOptions.None
            };

            [Fact]
            public void AnODataExceptionIsThrown()
            {
                Assert.Throws<ODataException>(() => new ODataQueryValidator().Validate(this.queryOptions, this.validationSettings));
            }
        }

        public class WhenTheFormatQueryOptionIsSetAndItIsSpecifiedInAllowedQueryOptions
        {
            private readonly ODataQueryOptions queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://localhost/api?$format=xml"));

            private readonly ODataValidationSettings validationSettings = new ODataValidationSettings
            {
                AllowedQueryOptions = AllowedQueryOptions.Format
            };

            [Fact]
            public void AnODataExceptionIsNotThrown()
            {
                Assert.DoesNotThrow(() => new ODataQueryValidator().Validate(this.queryOptions, this.validationSettings));
            }
        }
    }
}