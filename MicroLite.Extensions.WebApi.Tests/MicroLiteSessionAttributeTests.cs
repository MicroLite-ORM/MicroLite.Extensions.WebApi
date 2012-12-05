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
        public class WhenCallingOnActionExecuted
        {
            private readonly Mock<ISession> mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuted()
            {
                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = this.mockSession.Object;

                var attribute = new MicroLiteSessionAttribute();

                attribute.OnActionExecuted(new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    }
                });
            }

            [Fact]
            public void TheSessionIsDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithAutoManageTransactionAndTheFilterContextContainsAnException
        {
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecutedWithAutoManageTransactionAndTheFilterContextContainsAnException()
            {
                this.mockSession.Setup(x => x.Transaction).Returns(this.mockTransaction.Object);
                this.mockTransaction.Setup(x => x.WasRolledBack).Returns(false);

                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = mockSession.Object;

                var attribute = new MicroLiteSessionAttribute
                {
                    AutoManageTransaction = true
                };

                attribute.OnActionExecuted(new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new Exception()
                });
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithAutoManageTransactionAndTheTransactionHasNotBeenCommitted
        {
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecutedWithAutoManageTransactionAndTheTransactionHasNotBeenCommitted()
            {
                this.mockSession.Setup(x => x.Transaction).Returns(this.mockTransaction.Object);
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);

                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = mockSession.Object;

                var attribute = new MicroLiteSessionAttribute
                {
                    AutoManageTransaction = true
                };

                attribute.OnActionExecuted(new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    }
                });
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithoutAutoManageTransactionAndTheFilterContextContainsAnException
        {
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecutedWithoutAutoManageTransactionAndTheFilterContextContainsAnException()
            {
                this.mockSession.Setup(x => x.Transaction).Returns(this.mockTransaction.Object);
                this.mockTransaction.Setup(x => x.WasRolledBack).Returns(false);

                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = mockSession.Object;

                var attribute = new MicroLiteSessionAttribute
                {
                    AutoManageTransaction = false
                };

                attribute.OnActionExecuted(new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new Exception()
                });
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutedWithoutAutoManageTransactionAndTheTransactionHasNotBeenCommitted
        {
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecutedWithoutAutoManageTransactionAndTheTransactionHasNotBeenCommitted()
            {
                this.mockSession.Setup(x => x.Transaction).Returns(this.mockTransaction.Object);
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);

                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = mockSession.Object;

                var attribute = new MicroLiteSessionAttribute
                {
                    AutoManageTransaction = false
                };

                attribute.OnActionExecuted(new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    }
                });
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuting
        {
            private readonly MicroLiteApiController controller = new Mock<MicroLiteApiController>().Object;
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();
            private readonly ISession session;

            public WhenCallingOnActionExecuting()
            {
                this.session = mockSession.Object;
                this.mockSessionFactory.Setup(x => x.OpenSession()).Returns(this.session);

                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    mockSessionFactory.Object
                };

                var attribute = new MicroLiteSessionAttribute();
                attribute.OnActionExecuting(new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                });
            }

            [Fact]
            public void ASessionIsOpenedForTheSessionFactory()
            {
                this.mockSessionFactory.Verify(x => x.OpenSession(), Times.Once());
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(), Times.Once());
            }

            [Fact]
            public void TheSessionShouldBeSetOnTheController()
            {
                Assert.Same(this.session, this.controller.Session);
            }
        }

        public class WhenCallingOnActionExecutingAndAnIsolationIsSpecified
        {
            private readonly MicroLiteApiController controller = new Mock<MicroLiteApiController>().Object;
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();
            private readonly ISession session;

            public WhenCallingOnActionExecutingAndAnIsolationIsSpecified()
            {
                this.session = mockSession.Object;
                this.mockSessionFactory.Setup(x => x.OpenSession()).Returns(this.session);

                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    mockSessionFactory.Object
                };

                var attribute = new MicroLiteSessionAttribute
                {
                    IsolationLevel = IsolationLevel.Chaos
                };
                attribute.OnActionExecuting(new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                });
            }

            [Fact]
            public void ASessionIsOpenedForTheSessionFactory()
            {
                this.mockSessionFactory.Verify(x => x.OpenSession(), Times.Once());
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.Chaos), Times.Once());
            }

            [Fact]
            public void TheSessionShouldBeSetOnTheController()
            {
                Assert.Same(this.session, this.controller.Session);
            }
        }

        public class WhenCallingOnActionExecutingAndAutoManageTransactionIsFalse
        {
            private readonly MicroLiteApiController controller = new Mock<MicroLiteApiController>().Object;
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();
            private readonly ISession session;

            public WhenCallingOnActionExecutingAndAutoManageTransactionIsFalse()
            {
                this.session = mockSession.Object;
                this.mockSessionFactory.Setup(x => x.OpenSession()).Returns(this.session);

                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    mockSessionFactory.Object
                };

                var attribute = new MicroLiteSessionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuting(new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                });
            }

            [Fact]
            public void ASessionIsOpenedForTheSessionFactory()
            {
                this.mockSessionFactory.Verify(x => x.OpenSession(), Times.Once());
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(), Times.Never());
            }

            [Fact]
            public void TheSessionShouldBeSetOnTheController()
            {
                Assert.Same(this.session, this.controller.Session);
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

        public class WhenCallingOnActionExecutingAndTheControllerIsNotAMicroLiteApiController
        {
            [Fact]
            public void ANotSupportedExceptionShouldBeThrown()
            {
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

        public class WhenCallingOnActionExecutingWithANamedConnection
        {
            private readonly MicroLiteApiController controller = new Mock<MicroLiteApiController>().Object;
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly Mock<ISessionFactory> mockSessionFactory = new Mock<ISessionFactory>();
            private readonly ISession session;

            public WhenCallingOnActionExecutingWithANamedConnection()
            {
                this.session = mockSession.Object;
                this.mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
                this.mockSessionFactory.Setup(x => x.OpenSession()).Returns(this.session);

                MicroLiteSessionAttribute.SessionFactories = new[]
                {
                    mockSessionFactory.Object
                };

                var attribute = new MicroLiteSessionAttribute("Northwind");
                attribute.OnActionExecuting(new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                });
            }

            [Fact]
            public void ASessionIsOpenedForTheSessionFactory()
            {
                this.mockSessionFactory.Verify(x => x.OpenSession(), Times.Once());
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(), Times.Once());
            }

            [Fact]
            public void TheSessionShouldBeSetOnTheController()
            {
                Assert.Same(this.session, this.controller.Session);
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