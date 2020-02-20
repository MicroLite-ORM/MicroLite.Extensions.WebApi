using Moq;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests
{
    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteReadOnlyApiController"/> class.
    /// </summary>
    public class MicroLiteReadOnlyApiControllerTests
    {
        public class WhenConstructedWithAnIReadOnlySession
        {
            private readonly MicroLiteReadOnlyApiController _controller;
            private readonly IReadOnlySession _session = new Mock<IReadOnlySession>().Object;

            public WhenConstructedWithAnIReadOnlySession()
            {
                var mockController = new Mock<MicroLiteReadOnlyApiController>(_session)
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
