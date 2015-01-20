﻿namespace MicroLite.Extensions.WebApi.Tests
{
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteApiController"/> class.
    /// </summary>
    public class MicroLiteApiControllerTests
    {
        public class WhenConstructedUsingTheDefaultConstructor
        {
            private readonly MicroLiteApiController controller;

            public WhenConstructedUsingTheDefaultConstructor()
            {
                var mockController = new Mock<MicroLiteApiController>();
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsNull()
            {
                Assert.Null(this.controller.Session);
            }
        }

        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteApiController controller;
#if NET_4_0
            private readonly ISession session = new Mock<ISession>().Object;
#else
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;
#endif

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