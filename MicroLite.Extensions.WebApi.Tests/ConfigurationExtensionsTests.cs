namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using MicroLite.Configuration;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithWebApi : IDisposable
        {
            public WhenCallingWithWebApi()
            {
                MicroLiteSessionAttribute.SessionFactories = null;

                var configureExtensions = Mock.Of<IConfigureExtensions>();

                configureExtensions.WithWebApi();
            }

            public void Dispose()
            {
                MicroLiteSessionAttribute.SessionFactories = null;
            }

            [Fact]
            public void TheSessionFactoriesShouldBeSetOnTheMicroLiteSessionAttribute()
            {
                Assert.NotNull(MicroLiteSessionAttribute.SessionFactories);
            }
        }

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