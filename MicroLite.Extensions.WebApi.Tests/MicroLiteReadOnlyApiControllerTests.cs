namespace MicroLite.Extensions.WebApi.Tests
{
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteReadOnlyApiController"/> class.
    /// </summary>
    public class MicroLiteReadOnlyApiControllerTests
    {
        public class WhenConstructedUsingTheDefaultConstructor
        {
            private readonly MicroLiteReadOnlyApiController controller;

            public WhenConstructedUsingTheDefaultConstructor()
            {
                var mockController = new Mock<MicroLiteReadOnlyApiController>();
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsNull()
            {
                Assert.Null(this.controller.Session);
            }
        }

        public class WhenConstructedWithAnIReadOnlySession
        {
            private readonly MicroLiteReadOnlyApiController controller;
#if NET_4_0
            private readonly IReadOnlySession session = new Mock<IReadOnlySession>().Object;
#else
            private readonly IAsyncReadOnlySession session = new Mock<IAsyncReadOnlySession>().Object;
#endif

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