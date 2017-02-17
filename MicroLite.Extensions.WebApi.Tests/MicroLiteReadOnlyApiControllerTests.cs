namespace MicroLite.Extensions.WebApi.Tests
{
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteReadOnlyApiController"/> class.
    /// </summary>
    public class MicroLiteReadOnlyApiControllerTests
    {
        public class WhenConstructedWithAnIReadOnlySession
        {
            private readonly MicroLiteReadOnlyApiController controller;
            private readonly IAsyncReadOnlySession session = new Mock<IAsyncReadOnlySession>().Object;

            public WhenConstructedWithAnIReadOnlySession()
            {
                var mockController = new Mock<MicroLiteReadOnlyApiController>(this.session);
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