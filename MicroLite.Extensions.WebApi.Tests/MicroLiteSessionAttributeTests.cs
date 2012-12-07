namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using System.Data;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MicroLiteSessionAttribute"/> class.
    /// </summary>
    public class MicroLiteSessionAttributeTests
    {
        public class WhenCallingOnActionExecutedAndTheControllerIsAMicroLiteApiController
        {
            private readonly Mock<ISessionManager> mockSessionManager = new Mock<ISessionManager>();
            private readonly ISession session = new Mock<ISession>().Object;

            public WhenCallingOnActionExecutedAndTheControllerIsAMicroLiteApiController()
            {
                var mockController = new Mock<MicroLiteApiController>();

                var controller = mockController.Object;
                controller.Session = this.session;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new Exception()
                };

                var attribute = new MicroLiteSessionAttribute(null, this.mockSessionManager.Object);
                attribute.AutoManageTransaction = true;
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheSessionShouldBePassedToTheSessionManager()
            {
                this.mockSessionManager.Verify(x => x.OnActionExecuted(this.session, true, true));
            }
        }

        public class WhenCallingOnActionExecutedAndTheControllerIsAMicroLiteReadOnlyApiController
        {
            private readonly Mock<ISessionManager> mockSessionManager = new Mock<ISessionManager>();
            private readonly IReadOnlySession session = new Mock<IReadOnlySession>().Object;

            public WhenCallingOnActionExecutedAndTheControllerIsAMicroLiteReadOnlyApiController()
            {
                var mockController = new Mock<MicroLiteReadOnlyApiController>();

                var controller = mockController.Object;
                controller.Session = this.session;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new Exception()
                };

                var attribute = new MicroLiteSessionAttribute(null, this.mockSessionManager.Object);
                attribute.AutoManageTransaction = true;
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheSessionShouldBePassedToTheSessionManager()
            {
                this.mockSessionManager.Verify(x => x.OnActionExecuted(this.session, true, true));
            }
        }

        public class WhenCallingOnActionExecutingAndNoSessionFactoryIsFoundForTheConnectionName
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

                Assert.Equal(string.Format(ExceptionMessages.NoSessionFactoryFoundForConnectionName, "Northwind"), exception.Message);
            }
        }

        public class WhenCallingOnActionExecutingAndTheControllerIsAMicroLiteApiController
        {
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();
            private readonly Mock<ISessionManager> mockSessionManager = new Mock<ISessionManager>();

            public WhenCallingOnActionExecutingAndTheControllerIsAMicroLiteApiController()
            {
                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    this.mockSessionFactory.Object
                };

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = new Mock<MicroLiteApiController>().Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute(null, this.mockSessionManager.Object);
                attribute.AutoManageTransaction = true;
                attribute.IsolationLevel = IsolationLevel.Chaos;
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ASessionShouldBeOpened()
            {
                this.mockSessionFactory.Verify(x => x.OpenSession(), Times.Once());
            }

            [Fact]
            public void TheOnActionExecutingMethodOfTheSessionManagerShouldBeCalled()
            {
                this.mockSessionManager.Verify(x => x.OnActionExecuting(It.IsAny<IReadOnlySession>(), true, IsolationLevel.Chaos), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutingAndTheControllerIsAMicroLiteReadOnlyApiController
        {
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();
            private readonly Mock<ISessionManager> mockSessionManager = new Mock<ISessionManager>();

            public WhenCallingOnActionExecutingAndTheControllerIsAMicroLiteReadOnlyApiController()
            {
                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    this.mockSessionFactory.Object
                };

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = new Mock<MicroLiteReadOnlyApiController>().Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute(null, this.mockSessionManager.Object);
                attribute.AutoManageTransaction = true;
                attribute.IsolationLevel = IsolationLevel.Chaos;
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void AReadOnlySessionShouldBeOpened()
            {
                this.mockSessionFactory.Verify(x => x.OpenReadOnlySession(), Times.Once());
            }

            [Fact]
            public void TheOnActionExecutingMethodOfTheSessionManagerShouldBeCalled()
            {
                this.mockSessionManager.Verify(x => x.OnActionExecuting(It.IsAny<IReadOnlySession>(), true, IsolationLevel.Chaos), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutingAndTheControllerIsNotAMicroLiteApiController
        {
            [Fact]
            public void ANotSupportedExceptionShouldBeThrown()
            {
                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    new Mock<ISessionFactory>().Object
                };

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = new Mock<ApiController>().Object
                    }
                };

                var attribute = new MicroLiteSessionAttribute();

                var exception = Assert.Throws<NotSupportedException>(() => attribute.OnActionExecuting(context));

                Assert.Equal(ExceptionMessages.ControllerNotMicroLiteController, exception.Message);
            }
        }

        public class WhenCallingOnActionExecutingAndThereAreNoRegisteredSessionFactories
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

                var attribute = new MicroLiteSessionAttribute();

                var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));

                Assert.Equal(ExceptionMessages.NoSessionFactoriesSet, exception.Message);
            }
        }

        public class WhenCallingOnActionExecutingAndThereIsNoConnectionNameSetAndMultipleSessionFactoriesRegistered
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

                var attribute = new MicroLiteSessionAttribute();

                var exception = Assert.Throws<MicroLiteException>(() => attribute.OnActionExecuting(context));

                Assert.Equal(ExceptionMessages.NoConnectionNameMultipleSessionFactories, exception.Message);
            }
        }

        public class WhenConstructedUsingTheDefaultConstructor
        {
            private readonly MicroLiteSessionAttribute attribute = new MicroLiteSessionAttribute();

            [Fact]
            public void AutoManageTransactionShouldDefaultToTrue()
            {
                Assert.True(this.attribute.AutoManageTransaction);
            }

            [Fact]
            public void TheConnectionNamePropertyShouldBeNull()
            {
                Assert.Null(this.attribute.ConnectionName);
            }

            [Fact]
            public void TheIsolationLevelPropertyShouldBeNull()
            {
                Assert.Null(this.attribute.IsolationLevel);
            }
        }

        public class WhenConstructedWithAConnectionName
        {
            private readonly MicroLiteSessionAttribute attribute = new MicroLiteSessionAttribute("Northwind");

            [Fact]
            public void AutoManageTransactionShouldDefaultToTrue()
            {
                Assert.True(this.attribute.AutoManageTransaction);
            }

            [Fact]
            public void TheConnectionNamePropertyShouldBeSet()
            {
                Assert.Equal("Northwind", this.attribute.ConnectionName);
            }

            [Fact]
            public void TheIsolationLevelPropertyShouldBeNull()
            {
                Assert.Null(this.attribute.IsolationLevel);
            }
        }
    }
}