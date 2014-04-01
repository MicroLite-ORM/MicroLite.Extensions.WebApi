namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using MicroLite.Configuration;
    using MicroLite.Extensions.WebApi.Filters;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithWebApi_AndConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = default(IConfigureExtensions);

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithWebApi(new HttpConfiguration(), new WebApiConfigurationSettings()));

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }

        public class WhenCallingWithWebApi_AndHttpConfigurationIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithWebApi(null, new WebApiConfigurationSettings()));

                Assert.Equal("httpConfiguration", exception.ParamName);
            }
        }

        public class WhenCallingWithWebApi_AndWebApiConfigurationSettingsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => configureExtensions.WithWebApi(new HttpConfiguration(), null));

                Assert.Equal("settings", exception.ParamName);
            }
        }

        public class WhenCallingWithWebApiAndThereAreNoFiltersRegistered
        {
            private readonly HttpConfiguration configuration = new HttpConfiguration();

            public WhenCallingWithWebApiAndThereAreNoFiltersRegistered()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(configuration, WebApiConfigurationSettings.Default);
            }

            [Fact]
            public void AMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute)))
                    .SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void AValidateModelNotNullAttributeShouldBeRegistered()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute)))
                    .SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void AValidateModelStateAttributeShouldBeRegistered()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute)))
                    .SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void TheMicroLiteSessionAttributeShouldBeThird()
            {
                var filters = this.configuration.Filters.Select(f => f.Instance).ToArray();

                Assert.IsType<MicroLiteSessionAttribute>(filters[2]);
            }

            [Fact]
            public void TheValidateModelNotNullAttributeShouldBeFirst()
            {
                var filters = this.configuration.Filters.Select(f => f.Instance).ToArray();

                Assert.IsType<ValidateModelNotNullAttribute>(filters[0]);
            }

            [Fact]
            public void TheValidateModelStateAttributeShouldBeSecond()
            {
                var filters = this.configuration.Filters.Select(f => f.Instance).ToArray();

                Assert.IsType<ValidateModelStateAttribute>(filters[1]);
            }
        }

        public class WhenCallingWithWebApiWithConfigurationSettingsDisabled
        {
            private readonly HttpConfiguration configuration = new HttpConfiguration();

            public WhenCallingWithWebApiWithConfigurationSettingsDisabled()
            {
                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(
                    configuration,
                    new WebApiConfigurationSettings
                    {
                        RegisterGlobalMicroLiteSessionAttribute = false,
                        RegisterGlobalValidateModelNotNullAttribute = false,
                        RegisterGlobalValidateModelStateAttribute = false
                    });
            }

            [Fact]
            public void NoMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute)))
                    .SingleOrDefault();

                Assert.Null(filter);
            }

            [Fact]
            public void NoValidateModelNotNullAttributeShouldBeRegistered()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute)))
                    .SingleOrDefault();

                Assert.Null(filter);
            }

            [Fact]
            public void NoValidateModelStateAttributeShouldBeRegistered()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute)))
                    .SingleOrDefault();

                Assert.Null(filter);
            }
        }

        public class WhenCallingWithWebApiWithDefaultSettingsButFiltersAreAlreadyRegistered
        {
            private readonly HttpConfiguration configuration = new HttpConfiguration();
            private readonly MicroLiteSessionAttribute microLiteSessionAttribute = new MicroLiteSessionAttribute();
            private readonly ValidateModelNotNullAttribute validateModelNotNullAttribute = new ValidateModelNotNullAttribute();
            private readonly ValidateModelStateAttribute validateModelStateAttribute = new ValidateModelStateAttribute();

            public WhenCallingWithWebApiWithDefaultSettingsButFiltersAreAlreadyRegistered()
            {
                this.configuration.Filters.Add(this.microLiteSessionAttribute);
                this.configuration.Filters.Add(this.validateModelNotNullAttribute);
                this.configuration.Filters.Add(this.validateModelStateAttribute);

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(this.configuration, WebApiConfigurationSettings.Default);
            }

            [Fact]
            public void TheOriginalMicroLiteSessionAttributeShouldNotBeReplaced()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute)))
                    .SingleOrDefault();

                Assert.Same(microLiteSessionAttribute, filter.Instance);
            }

            [Fact]
            public void TheOriginalValidateModelNotNullAttributeShouldNotBeReplaced()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute)))
                    .SingleOrDefault();

                Assert.Same(validateModelNotNullAttribute, filter.Instance);
            }

            [Fact]
            public void TheOriginalValidateModelStateAttributeShouldNotBeReplaced()
            {
                var filter = this.configuration.Filters
                    .Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute)))
                    .SingleOrDefault();

                Assert.Same(validateModelStateAttribute, filter.Instance);
            }
        }
    }
}