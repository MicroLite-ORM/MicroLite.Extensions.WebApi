namespace MicroLite.Extensions.WebApi.Tests
{
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using MicroLite.Extensions.WebApi;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteSessionAttribute"/> class.
    /// </summary>
    public class MicroLiteSessionAttributeTests
    {
        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController()
            {
                var mockController = new Mock<MicroLiteApiController>();

                var controller = mockController.Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    }
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController()
            {
                var mockController = new Mock<MicroLiteReadOnlyApiController>();

                var controller = mockController.Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    }
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_AndNoRegisteredSessionFactories
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                MicroLiteSessionAttribute.SessionFactories = null;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = new Mock<MicroLiteApiController>().Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");

                var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));

                Assert.Equal(Messages.NoSessionFactoriesSet, exception.Message);
            }
        }

        public class WhenCallingOnActionExecuting_AndNoSessionFactoryIsFoundForTheConnectionName
        {
            [Fact]
            public void AMicroLiteExceptionIsThrown()
            {
                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    new Mock<ISessionFactory>().Object,
                    new Mock<ISessionFactory>().Object
                };

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = new Mock<MicroLiteApiController>().Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");

                var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));

                Assert.Equal(string.Format(Messages.NoSessionFactoryFoundForConnectionName, "Northwind"), exception.Message);
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteApiController
        {
            private readonly Mock<MicroLiteApiController> mockController = new Mock<MicroLiteApiController>();
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();

            public WhenCallingOnActionExecuting_WithAMicroLiteApiController()
            {
                this.mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
                this.mockSessionFactory.Setup(x => x.OpenAsyncSession()).Returns(this.mockSession.Object);

                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    this.mockSessionFactory.Object
                };

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = this.mockController.Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ASessionShouldBeOpened()
            {
                this.mockSessionFactory.Verify(x => x.OpenAsyncSession(), Times.Once());
            }

            [Fact]
            public void TheSessionShouldBeSetOnTheController()
            {
                Assert.Equal(this.mockSession.Object, this.mockController.Object.Session);
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController
        {
            private readonly Mock<MicroLiteReadOnlyApiController> mockController = new Mock<MicroLiteReadOnlyApiController>();
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController()
            {
                this.mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
                this.mockSessionFactory.Setup(x => x.OpenAsyncReadOnlySession()).Returns(this.mockSession.Object);

                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    this.mockSessionFactory.Object
                };

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = this.mockController.Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void AReadOnlySessionShouldBeOpened()
            {
                this.mockSessionFactory.Verify(x => x.OpenAsyncReadOnlySession(), Times.Once());
            }

            [Fact]
            public void TheSessionShouldBeSetOnTheController()
            {
                Assert.Equal(this.mockSession.Object, this.mockController.Object.Session);
            }
        }

        public class WhenConstructedWithAConnectionName
        {
            private readonly MicroLiteSessionAttribute attribute = new MicroLiteSessionAttribute("Northwind");

            [Fact]
            public void TheConnectionNamePropertyShouldBeSet()
            {
                Assert.Equal("Northwind", this.attribute.ConnectionName);
            }
        }
    }
}