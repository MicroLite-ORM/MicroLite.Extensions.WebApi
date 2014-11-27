namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using MicroLite.Extensions.WebApi.Tests.TestEntities;
    using Moq;
    using Xunit;

    public class MicroLiteApiController_T_Tests
    {
        public class WhenCallingDeleteAndAnEntityIsDeleted
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingDeleteAndAnEntityIsDeleted()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Advanced.Delete(typeof(Customer), this.identifier)).Returns(true);
#else
                this.mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(true));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;
#if NET_4_0
                this.response = this.controller.Delete(this.identifier);
#else
                this.response = this.controller.Delete(this.identifier).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, this.response.StatusCode);
            }
        }

        public class WhenCallingDeleteAndAnEntityIsNotDeleted
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingDeleteAndAnEntityIsNotDeleted()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Advanced.Delete(typeof(Customer), this.identifier)).Returns(false);
#else
                this.mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(false));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Delete(this.identifier);
#else
                this.response = this.controller.Delete(this.identifier).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }
        }

        public class WhenCallingGetAndAnEntityIsNotReturned
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingGetAndAnEntityIsNotReturned()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns((Customer)null);
#else
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult((Customer)null));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Get(this.identifier);
#else
                this.response = this.controller.Get(this.identifier).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }
        }

        public class WhenCallingGetAndAnEntityIsReturned
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Customer customer = new Customer();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingGetAndAnEntityIsReturned()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns(this.customer);
#else
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(this.customer));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Get(this.identifier);
#else
                this.response = this.controller.Get(this.identifier).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(this.customer, ((ObjectContent)this.response.Content).Value);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }
        }

        public class WhenCallingPost
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly Customer customer = new Customer();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingPost()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Insert(It.IsAny<object>()))
#else
                this.mockSession.Setup(x => x.InsertAsync(It.IsAny<object>())).Returns(System.Threading.Tasks.Task.FromResult(0))
#endif
.Callback((object o) =>
                    {
                        ((Customer)o).Id = this.identifier;
                    });

                this.controller.Request = new HttpRequestMessage();
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Post(this.customer);
#else
                this.response = this.controller.Post(this.customer).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(this.customer, ((ObjectContent)this.response.Content).Value);
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheUriForTheEntity()
            {
                Assert.Equal(new Uri("http://localhost/api/Customers/12345"), this.response.Headers.Location);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeCreated()
            {
                Assert.Equal(HttpStatusCode.Created, this.response.StatusCode);
            }
        }

        public class WhenCallingPutAndAnEntityIsNotReturned
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingPutAndAnEntityIsNotReturned()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns((Customer)null);
#else
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult((Customer)null));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Put(this.identifier, new Customer());
#else
                this.response = this.controller.Put(this.identifier, new Customer()).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }
        }

        public class WhenCallingPutAndAnEntityIsNotUpdated
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            public WhenCallingPutAndAnEntityIsNotUpdated()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns(new Customer());
                this.mockSession.Setup(x => x.Update(It.IsAny<object>())).Returns(false);
#else
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(new Customer()));
                this.mockSession.Setup(x => x.UpdateAsync(It.IsAny<object>())).Returns(System.Threading.Tasks.Task.FromResult(false));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Put(this.identifier, new Customer());
#else
                this.response = this.controller.Put(this.identifier, new Customer()).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotModified()
            {
                Assert.Equal(HttpStatusCode.NotModified, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }
        }

        public class WhenCallingPutAndAnEntityIsUpdated
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
#if NET_4_0
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
#else
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
#endif
            private readonly HttpResponseMessage response;

            private readonly Customer updatedCustomer = new Customer
            {
                Name = "Joe Bloggs"
            };

            public WhenCallingPutAndAnEntityIsUpdated()
            {
#if NET_4_0
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns(new Customer());
                this.mockSession.Setup(x => x.Update(It.IsAny<object>())).Returns(true);
#else
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(new Customer()));
                this.mockSession.Setup(x => x.UpdateAsync(It.IsAny<object>())).Returns(System.Threading.Tasks.Task.FromResult(true));
#endif

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

#if NET_4_0
                this.response = this.controller.Put(this.identifier, this.updatedCustomer);
#else
                this.response = this.controller.Put(this.identifier, this.updatedCustomer).Result;
#endif
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }

            [Fact]
            public void TheUpdatedCustomerShouldHaveTheIdentifierSet()
            {
                Assert.Equal(this.identifier, this.updatedCustomer.Id);
            }
        }

        public class WhenConstructedUsingTheDefaultConstructor
        {
            private readonly MicroLiteApiController<Customer, int> controller;

            public WhenConstructedUsingTheDefaultConstructor()
            {
                var mockController = new Mock<MicroLiteApiController<Customer, int>>();
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
            private readonly MicroLiteApiController<Customer, int> controller;
#if NET_4_0
            private readonly ISession session = new Mock<ISession>().Object;
#else
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;
#endif

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteApiController<Customer, int>>(this.session);
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(this.session, this.controller.Session);
            }
        }

        private class CustomerController : MicroLiteApiController<Customer, int>
        {
            public CustomerController()
            {
                this.GetEntityResourceUri = (int id) =>
                {
                    return new Uri("http://localhost/api/Customers/" + id.ToString());
                };
            }

#if NET_4_0
            public HttpResponseMessage Delete(int id)
            {
                return this.DeleteEntityResponse(id);
            }

            public HttpResponseMessage Get(int id)
            {
                return this.GetEntityResponse(id);
            }

            public HttpResponseMessage Post(Customer entity)
            {
                return this.PostEntityResponse(entity);
            }

            public HttpResponseMessage Put(int id, Customer entity)
            {
                return this.PutEntityResponse(id, entity);
            }
#else

            public System.Threading.Tasks.Task<HttpResponseMessage> Delete(int id)
            {
                return this.DeleteEntityResponseAsync(id);
            }

            public System.Threading.Tasks.Task<HttpResponseMessage> Get(int id)
            {
                return this.GetEntityResponseAsync(id);
            }

            public System.Threading.Tasks.Task<HttpResponseMessage> Post(Customer entity)
            {
                return this.PostEntityResponseAsync(entity);
            }

            public System.Threading.Tasks.Task<HttpResponseMessage> Put(int id, Customer entity)
            {
                return this.PutEntityResponseAsync(id, entity);
            }

#endif
        }
    }
}