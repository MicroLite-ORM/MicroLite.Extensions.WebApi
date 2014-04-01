namespace MicroLite.Extensions.WebApi.Tests
{
    using MicroLite.Configuration;
    using Xunit;

    public class WebApiConfigurationSettingsTests
    {
        [Fact]
        public void PropertiesAreSetToTrueByDefault()
        {
            var settings = new WebApiConfigurationSettings();

            Assert.True(settings.RegisterGlobalMicroLiteSessionAttribute);
            Assert.True(settings.RegisterGlobalValidateModelNotNullAttribute);
            Assert.True(settings.RegisterGlobalValidateModelStateAttribute);
        }
    }
}