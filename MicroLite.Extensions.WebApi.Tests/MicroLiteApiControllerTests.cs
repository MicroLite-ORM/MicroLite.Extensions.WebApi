using Moq;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests
{
    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteApiController"/> class.
    /// </summary>
    public class MicroLiteApiControllerTests
    {
        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteApiController _controller;
            private readonly ISession _session = new Mock<ISession>().Object;

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteApiController>(_session)
                {
                    CallBase = true
                };

                _controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(_session, _controller.Session);
            }
        }
    }
}
