namespace MicroLite.Extensions.WebApi.Tests
{
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteApiController"/> class.
    /// </summary>
    public class MicroLiteApiControllerTests
    {
        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteApiController controller;
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteApiController>(this.session);
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(this.session, this.controller.Session);
            }
        }
    }
}