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
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif

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
#if NET_4_0
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();
#else
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
#endif

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
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();

            public WhenCallingOnActionExecuting_WithAMicroLiteApiController()
            {
                this.mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
#if NET_4_0
                this.mockSessionFactory.Setup(x => x.OpenSession()).Returns(this.mockSession.Object);
#else
                this.mockSessionFactory.Setup(x => x.OpenAsyncSession()).Returns(this.mockSession.Object);
#endif

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
#if NET_4_0
                this.mockSessionFactory.Verify(x => x.OpenSession(), Times.Once());
#else
                this.mockSessionFactory.Verify(x => x.OpenAsyncSession(), Times.Once());
#endif
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
#if NET_4_0
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();
#else
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
#endif
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController()
            {
                this.mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
#if NET_4_0
                this.mockSessionFactory.Setup(x => x.OpenReadOnlySession()).Returns(this.mockSession.Object);
#else
                this.mockSessionFactory.Setup(x => x.OpenAsyncReadOnlySession()).Returns(this.mockSession.Object);
#endif

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
#if NET_4_0
                this.mockSessionFactory.Verify(x => x.OpenReadOnlySession(), Times.Once());
#else
                this.mockSessionFactory.Verify(x => x.OpenAsyncReadOnlySession(), Times.Once());
#endif
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