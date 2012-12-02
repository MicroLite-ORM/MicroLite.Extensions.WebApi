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
        [Fact]
        public void ConnectionNameConstructorSetsAutoManageTransactionToTrue()
        {
            var attribute = new MicroLiteSessionAttribute("Northwind");

            Assert.True(attribute.AutoManageTransaction);
        }

        [Fact]
        public void ConstructorSetsConnectionName()
        {
            var connectionName = "Northwind";

            var attribute = new MicroLiteSessionAttribute(connectionName);

            Assert.Equal(connectionName, attribute.ConnectionName);
        }

        [Fact]
        public void DefaultConstructorSetsAutoManageTransactionToTrue()
        {
            var attribute = new MicroLiteSessionAttribute();

            Assert.True(attribute.AutoManageTransaction);
        }

        [Fact]
        public void OnActionExecutedCommitsTransactionIfActionExecutedContextHasNoExceptionAndTransactionIsActive()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());
            mockSession.Setup(x => x.Transaction.IsActive).Returns(true);
            mockSession.Setup(x => x.Transaction.Commit());

            var controller = new Mock<MicroLiteApiController>().Object;
            controller.Session = mockSession.Object;

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

            mockSession.VerifyAll();
        }

        [Fact]
        public void OnActionExecutedDoesNotCommitTransactionIfActionExecutedHasNoExceptionAndTransactionIsNotActive()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());
            mockSession.Setup(x => x.Transaction.IsActive).Returns(false);
            mockSession.Setup(x => x.Transaction.Commit());

            var controller = new Mock<MicroLiteApiController>().Object;
            controller.Session = mockSession.Object;

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

            mockSession.Verify(x => x.Transaction.Commit(), Times.Never(), "If the transaction has already been committed, it should not be again");
            mockSession.Verify(x => x.Dispose(), "The session should still be disposed");
        }

        [Fact]
        public void OnActionExecutedDoesNotCommitTransactionIfAutoManageTransactionIsFalse()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());
            mockSession.Setup(x => x.Transaction.IsActive).Returns(true);
            mockSession.Setup(x => x.Transaction.Commit());

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

            mockSession.Verify(x => x.Transaction.Commit(), Times.Never(), "If the attribute is marked AutoManageTransaction = false, the transaction should not be committed automatically");
            mockSession.Verify(x => x.Dispose(), "The session should still be disposed");
        }

        [Fact]
        public void OnActionExecutedDoesNotRollbackTransactionIfActionExecutedContextHasExceptionAndTransactionWasRolledBack()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());
            mockSession.Setup(x => x.Transaction.WasRolledBack).Returns(true);
            mockSession.Setup(x => x.Transaction.Rollback());

            var controller = new Mock<MicroLiteApiController>().Object;
            controller.Session = mockSession.Object;

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

            mockSession.Verify(x => x.Transaction.Rollback(), Times.Never(), "If the transaction has already been rolled back, it should not be again");
            mockSession.Verify(x => x.Dispose(), "The session should still be disposed");
        }

        [Fact]
        public void OnActionExecutedDoesNotRollbackTransactionIfAutoManageTransactionIsFalse()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());
            mockSession.Setup(x => x.Transaction.WasCommitted).Returns(false);
            mockSession.Setup(x => x.Transaction.Rollback());

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

            mockSession.Verify(x => x.Transaction.Rollback(), Times.Never(), "If the attribute is marked AutoManageTransaction = false, the transaction should not be rolledback automatically");
            mockSession.Verify(x => x.Dispose(), "The session should still be disposed");
        }

        [Fact]
        public void OnActionExecutedRollsBackTransactionIfActionExecutedContextHasExceptionAndTransactionNotRolledBack()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Dispose());
            mockSession.Setup(x => x.Transaction.WasRolledBack).Returns(false);
            mockSession.Setup(x => x.Transaction.Rollback());

            var controller = new Mock<MicroLiteApiController>().Object;
            controller.Session = mockSession.Object;

            var attribute = new MicroLiteSessionAttribute();

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

            mockSession.VerifyAll();
        }

        [Fact]
        public void OnActionExecutingOpensSessionAndBeginsTransaction()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.BeginTransaction());

            var session = mockSession.Object;

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.OpenSession()).Returns(session);

            MicroLiteSessionAttribute.SessionFactories = new[]
            {
                mockSessionFactory.Object
            };

            var controller = new Mock<MicroLiteApiController>().Object;

            var attribute = new MicroLiteSessionAttribute();

            attribute.OnActionExecuting(new HttpActionContext
            {
                ControllerContext = new HttpControllerContext
                {
                    Controller = controller
                }
            });

            mockSessionFactory.VerifyAll();
            mockSession.VerifyAll();

            Assert.Same(session, controller.Session);
        }

        [Fact]
        public void OnActionExecutingOpensSessionAndBeginsTransactionWithSpecifiedIsolationLevel()
        {
            var isolationLevel = IsolationLevel.Chaos;

            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.BeginTransaction(isolationLevel));

            var session = mockSession.Object;

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.OpenSession()).Returns(session);

            MicroLiteSessionAttribute.SessionFactories = new[]
            {
                mockSessionFactory.Object
            };

            var controller = new Mock<MicroLiteApiController>().Object;

            var attribute = new MicroLiteSessionAttribute
            {
                IsolationLevel = isolationLevel
            };

            attribute.OnActionExecuting(new HttpActionContext
            {
                ControllerContext = new HttpControllerContext
                {
                    Controller = controller
                }
            });

            mockSessionFactory.VerifyAll();
            mockSession.VerifyAll();

            Assert.Same(session, controller.Session);
        }

        [Fact]
        public void OnActionExecutingOpensSessionButDoesNotBeginTransactionIfAutoManageTransactionIsSetToFalse()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.BeginTransaction());

            var session = mockSession.Object;

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.OpenSession()).Returns(session);

            MicroLiteSessionAttribute.SessionFactories = new[]
            {
                mockSessionFactory.Object
            };

            var controller = new Mock<MicroLiteApiController>().Object;

            var attribute = new MicroLiteSessionAttribute()
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

            mockSessionFactory.VerifyAll();
            mockSession.Verify(x => x.BeginTransaction(), Times.Never());

            Assert.Same(session, controller.Session);
        }

        [Fact]
        public void OnActionExecutingThrowsMicroLiteExceptionIfNoConnectionNameAndMultipleSessionFactories()
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

        [Fact]
        public void OnActionExecutingThrowsMicroLiteExceptionIfNoSessionFactories()
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

        [Fact]
        public void OnActionExecutingThrowsMicroLiteExceptionIfNoSessionFactoryFoundForConnectionName()
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

        [Fact]
        public void OnActionExecutingThrowsNotSupportedExceptionIfControllerIsNotMicroLiteController()
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

        [Fact]
        public void OnActionOpensSessionAndBeginsTransactionForNamedConnection()
        {
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.BeginTransaction());

            var session = mockSession.Object;

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.ConnectionName).Returns("Northwind");
            mockSessionFactory.Setup(x => x.OpenSession()).Returns(session);

            MicroLiteSessionAttribute.SessionFactories = new[]
            {
                mockSessionFactory.Object
            };

            var controller = new Mock<MicroLiteApiController>().Object;

            var attribute = new MicroLiteSessionAttribute("Northwind");

            attribute.OnActionExecuting(new HttpActionContext
            {
                ControllerContext = new HttpControllerContext
                {
                    Controller = controller
                }
            });

            mockSessionFactory.VerifyAll();
            mockSession.VerifyAll();

            Assert.Same(session, controller.Session);
        }
    }
}