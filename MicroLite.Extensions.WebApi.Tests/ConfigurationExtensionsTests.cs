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
        public class WhenCallingWithWebApiAndRegisterGlobalFilterFalse
        {
            public WhenCallingWithWebApiAndRegisterGlobalFilterFalse()
            {
                GlobalConfiguration.Configuration.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(registerGlobalFilter: false);
            }

            [Fact]
            public void NoMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Null(filter);
            }
        }

        public class WhenCallingWithWebApiAndThereIsAMicroLiteSessionAttributeRegistered
        {
            private readonly MicroLiteSessionAttribute attribute = new MicroLiteSessionAttribute();

            public WhenCallingWithWebApiAndThereIsAMicroLiteSessionAttributeRegistered()
            {
                GlobalConfiguration.Configuration.Filters.Clear();
                GlobalConfiguration.Configuration.Filters.Add(this.attribute);

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(registerGlobalFilter: true);
            }

            [Fact]
            public void TheOriginalFilterShouldNotBeReplaced()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.Same(attribute, filter.Instance);
            }
        }

        public class WhenCallingWithWebApiAndThereIsNoMicroLiteSessionAttributeRegistered
        {
            public WhenCallingWithWebApiAndThereIsNoMicroLiteSessionAttributeRegistered()
            {
                GlobalConfiguration.Configuration.Filters.Clear();

                var configureExtensions = new Mock<IConfigureExtensions>().Object;

                configureExtensions.WithWebApi(registerGlobalFilter: true);
            }

            [Fact]
            public void AMicroLiteSessionAttributeShouldBeRegistered()
            {
                var filter = GlobalConfiguration.Configuration.Filters.Where(f => f.Instance.GetType().IsAssignableFrom(typeof(MicroLiteSessionAttribute))).SingleOrDefault();

                Assert.NotNull(filter);
            }
        }
    }
}