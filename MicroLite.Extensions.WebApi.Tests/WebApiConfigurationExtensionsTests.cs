namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using MicroLite.Configuration;
    using Moq;
    using Xunit;

    public class WebApiConfigurationExtensionsTests
    {
        public class WhenCallingWithWebApi_AndConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = default(IConfigureExtensions);

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithWebApi());

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }
    }
}