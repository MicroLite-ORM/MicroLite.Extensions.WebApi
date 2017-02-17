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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingDeleteAndAnEntityIsDeleted()
            {
                this.mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(true));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Delete(this.identifier).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingDeleteAndAnEntityIsNotDeleted()
            {
                this.mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(false));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Delete(this.identifier).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingGetAndAnEntityIsNotReturned()
            {
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult((Customer)null));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.identifier).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingGetAndAnEntityIsReturned()
            {
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(this.customer));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.identifier).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingPost()
            {
                this.mockSession.Setup(x => x.InsertAsync(It.IsNotNull<Customer>())).Returns(System.Threading.Tasks.Task.FromResult(0))
.Callback((object o) =>
                    {
                        ((Customer)o).Id = this.identifier;
                    });

                this.controller.Request = new HttpRequestMessage();
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Post(this.customer).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingPutAndAnEntityIsNotReturned()
            {
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult((Customer)null));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Put(this.identifier, new Customer()).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCallingPutAndAnEntityIsNotUpdated()
            {
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(new Customer()));
                this.mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(System.Threading.Tasks.Task.FromResult(false));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Put(this.identifier, new Customer()).Result;
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
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            private readonly Customer updatedCustomer = new Customer
            {
                Name = "Joe Bloggs"
            };

            public WhenCallingPutAndAnEntityIsUpdated()
            {
                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(System.Threading.Tasks.Task.FromResult(new Customer()));
                this.mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(System.Threading.Tasks.Task.FromResult(true));

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Put(this.identifier, this.updatedCustomer).Result;
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
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;

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
        }
    }
}