using System;
using System.Data;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Moq;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests
{
    /// <summary>
    /// Unit Tests for the <see cref="AutoManageTransactionAttribute"/> class.
    /// </summary>
    public class AutoManageTransactionAttributeTests
    {
        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAnActiveTransaction
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAnActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndNoActiveTransaction
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndNoActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndNoCurrentTransaction
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAnActiveTransaction
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAnActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndCommittingAnActiveTransactionThrowsAnException
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndCommittingAnActiveTransactionThrowsAnException()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndNoActiveTransaction
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndNoActiveTransaction()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndNoCurrentTransaction
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();

            [Fact]
            public void OnActionExecutedDoesNotThrowAnException()
            {
                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(false);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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

                var attribute = new AutoManageTransactionAttribute();
                attribute.OnActionExecuted(context);
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsNotRolledBackAgain()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack
        {
            private readonly Mock<IReadOnlySession> _mockSession = new Mock<IReadOnlySession>();
            private readonly Mock<ITransaction> _mockTransaction = new Mock<ITransaction>();

            public WhenCallingOnActionExecuted_WithAMicroLiteReadOnlyApiController_AndTheContextContainsAnException_AndTheTransactionHasNotBeenRolledBack()
            {
                _mockTransaction.Setup(x => x.IsActive).Returns(true);
                _mockSession.Setup(x => x.CurrentTransaction).Returns(_mockTransaction.Object);

                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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
                _mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotCommitted()
            {
                _mockTransaction.Verify(x => x.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                _mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteApiController
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteApiController()
            {
                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

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
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteApiController_AndAutoManageTransactionIsFalse()
            {
                MicroLiteApiController controller = new Mock<MicroLiteApiController>(_mockSession.Object).Object;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                };

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController()
            {
                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

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
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }
        }

        public class WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse
        {
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();

            public WhenCallingOnActionExecuting_WithAMicroLiteReadOnlyApiController_AndAutoManageTransactionIsFalse()
            {
                MicroLiteReadOnlyApiController controller = new Mock<MicroLiteReadOnlyApiController>(_mockSession.Object).Object;

                var context = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext
                    {
                        Controller = controller
                    }
                };

                var attribute = new AutoManageTransactionAttribute
                {
                    AutoManageTransaction = false
                };
                attribute.OnActionExecuting(context);
            }

            [Fact]
            public void ATransactionIsNotStarted()
            {
                _mockSession.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Never());
            }
        }

        public class WhenConstructed
        {
            private readonly AutoManageTransactionAttribute _attribute = new AutoManageTransactionAttribute();

            [Fact]
            public void AutoManageTransactionIsTrue()
            {
                Assert.True(_attribute.AutoManageTransaction);
            }

            [Fact]
            public void IsolationLevelIsReadCommitted()
            {
                Assert.Equal(IsolationLevel.ReadCommitted, _attribute.IsolationLevel);
            }
        }
    }
}
