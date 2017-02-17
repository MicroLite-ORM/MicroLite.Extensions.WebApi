namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using System.Data;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using MicroLite.Extensions.WebApi;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AutoManageTransactionAttribute"/> class.
    /// </summary>
    public class AutoManageTransactionAttributeTests
    {
        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAnActiveTransaction
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAnActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();

                Assert.Throws<InvalidOperationException>(() => attribute.OnActionExecuted(context));
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndNoActiveTransaction
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndNoActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndNoCurrentTransaction
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                var controller = new Mock<MicroLiteApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();

                Assert.DoesNotThrow(() => attribute.OnActionExecuted(context));
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAnActiveTransaction
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAnActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();

                Assert.Throws<InvalidOperationException>(() => attribute.OnActionExecuted(context));
            }

            [Fact]
            public void TheTransactionIsCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndNoActiveTransaction
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndNoActiveTransaction()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndNoCurrentTransaction
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
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

                var attribute = new AutoManageTransactionAttribute();

                Assert.DoesNotThrow(() => attribute.OnActionExecuted(context));
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(false);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<IAsyncReadOnlySession> mockSession = new Mock<IAsyncReadOnlySession>();
            private readonly Mock<ITransaction> mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.IsActive).Returns(true);
                this.mockSession.Setup(x => x.CurrentTransaction).Returns(this.mockTransaction.Object);

                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                    {
                        ControllerContext = new HttpControllerContext
                        {
                            Controller = controller
                        }
                    },
                    Exception = new System.Exception()
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteApiController
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteApiController()
            {
                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse()
            {
                var controller = new Mock<MicroLiteApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController()
            {
                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse()
            {
                var controller = new Mock<MicroLiteReadOnlyApiController>().Object;
                controller.Session = this.mockSession.Object;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                };

                var attribute = new AutoManageTransactionAttribute();
                attribute.AutoManageTransaction = false;
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenConstructed
        {
            private readonly AutoManageTransactionAttribute attribute = new AutoManageTransactionAttribute();

            [Fact]
            public void AutoManageTransactionIsTrue()
            {
                Assert.True(this.attribute.AutoManageTransaction);
            }

            [Fact]
            public void IsolationLevelIsReadCommitted()
            {
                Assert.Equal(IsolationLevel.ReadCommitted, this.attribute.IsolationLevel);
            }
        }
    }
}