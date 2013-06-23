namespace MicroLite.Extensions.WebApi.Tests
{
    using System.Linq;
    using System.Web.Http;
    using MicroLite.Configuration;
    using MicroLite.Extensions.WebApi.Filters;
    using Moq;
    using Xunit;

    public class ConfigurationExtensionsTests
    {
        public class WhenCallingWithWebApiAndThereAreNoFiltersRegistered
        {
            public WhenCallingWithWebApiAndThereAreNoFiltersRegistered()
            {
                GlobalConfiguration.Configuration.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(WebApiConfigurationSettings.Default);
            }

            [Fact]
            public void AMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void AValidateModelNotNullAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute))).SingleOrDefault();

                Assert.NotNull(filter);
            }

            [Fact]
            public void AValidateModelStateAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))).SingleOrDefault();

                Assert.NotNull(filter);
            }
        }

        public class WhenCallingWithWebApiWithConfigurationSettingsDisabled
        {
            public WhenCallingWithWebApiWithConfigurationSettingsDisabled()
            {
                GlobalConfiguration.Configuration.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(new WebApiConfigurationSettings
                {
                    RegisterGlobalMicroLiteSessionAttribute = false,
                    RegisterGlobalValidateModelNotNullAttribute = false,
                    RegisterGlobalValidateModelStateAttribute = false
                });
            }

            [Fact]
            public void NoMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Null(filter);
            }

            [Fact]
            public void NoValidateModelNotNullAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute))).SingleOrDefault();

                Assert.Null(filter);
            }

            [Fact]
            public void NoValidateModelStateAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))).SingleOrDefault();

                Assert.Null(filter);
            }
        }

        public class WhenCallingWithWebApiWithDefaultSettingsButFiltersAreAlreadyRegistered
        {
            private readonly MicroLiteSessionAttribute microLiteSessionAttribute = new MicroLiteSessionAttribute();
            private readonly ValidateModelNotNullAttribute validateModelNotNullAttribute = new ValidateModelNotNullAttribute();
            private readonly ValidateModelStateAttribute validateModelStateAttribute = new ValidateModelStateAttribute();

            public WhenCallingWithWebApiWithDefaultSettingsButFiltersAreAlreadyRegistered()
            {
                GlobalConfiguration.Configuration.Filters.Clear();
                GlobalConfiguration.Configuration.Filters.Add(this.microLiteSessionAttribute);
                GlobalConfiguration.Configuration.Filters.Add(this.validateModelNotNullAttribute);
                GlobalConfiguration.Configuration.Filters.Add(this.validateModelStateAttribute);

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(WebApiConfigurationSettings.Default);
            }

            [Fact]
            public void TheOriginalMicroLiteSessionAttributeShouldNotBeReplaced()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Same(microLiteSessionAttribute, filter.Instance);
            }

            [Fact]
            public void TheOriginalValidateModelNotNullAttributeShouldNotBeReplaced()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelNotNullAttribute))).SingleOrDefault();

                Assert.Same(validateModelNotNullAttribute, filter.Instance);
            }

            [Fact]
            public void TheOriginalValidateModelStateAttributeShouldNotBeReplaced()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(ValidateModelStateAttribute))).SingleOrDefault();

                Assert.Same(validateModelStateAttribute, filter.Instance);
            }
        }
    }
}