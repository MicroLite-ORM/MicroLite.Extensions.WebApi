namespace MicroLite.Extensions.WebApi.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using Moq;
    using Xunit;

    public class MicroLiteApiController_T_Tests
    {
        public class WhenCallingDeleteAndAnEntityIsDeleted
        {
            private readonly CustomerController controller = new CustomerController();
            private readonly int identifier = 12345;
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingDeleteAndAnEntityIsDeleted()
            {
                this.mockSession.Setup(x => x.Advanced.Delete(typeof(Customer), this.identifier)).Returns(true);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Delete(this.identifier);
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
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingDeleteAndAnEntityIsNotDeleted()
            {
                this.mockSession.Setup(x => x.Advanced.Delete(typeof(Customer), this.identifier)).Returns(false);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Delete(this.identifier);
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
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingGetAndAnEntityIsNotReturned()
            {
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns((Customer)null);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.identifier);
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
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingGetAndAnEntityIsReturned()
            {
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns(this.customer);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.identifier);
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
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingPost()
            {
                this.mockSession.Setup(x => x.Insert(It.IsAny<object>()))
                    .Callback((object o) =>
                    {
                        ((Customer)o).Id = this.identifier;
                    });

                this.controller.Request = new HttpRequestMessage();
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Post(this.customer);
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
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingPutAndAnEntityIsNotReturned()
            {
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns((Customer)null);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Put(this.identifier, new Customer());
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
            private readonly Mock<ISession> mockSession = new Mock<ISession>();
            private readonly HttpResponseMessage response;

            public WhenCallingPutAndAnEntityIsNotUpdated()
            {
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns(new Customer());
                this.mockSession.Setup(x => x.Update(It.IsAny<object>())).Returns(false);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Put(this.identifier, new Customer());
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

            private readonly Customer existingCustomer = new Customer
            {
                Id = 12345
            };

            private readonly int identifier = 12345;

            private readonly Mock<ISession> mockSession = new Mock<ISession>();

            private readonly HttpResponseMessage response;

            private readonly Customer updatedCustomer = new Customer
            {
                Name = "Joe Bloggs"
            };

            public WhenCallingPutAndAnEntityIsUpdated()
            {
                this.mockSession.Setup(x => x.Single<Customer>(this.identifier)).Returns(this.existingCustomer);
                this.mockSession.Setup(x => x.Update(It.IsAny<object>())).Returns(true);

                this.controller.Request = new HttpRequestMessage();
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Put(this.identifier, this.updatedCustomer);
            }

            [Fact]
            public void TheExistingCustomerShouldBeUpdatedWithTheNewName()
            {
                Assert.Equal(this.updatedCustomer.Name, this.existingCustomer.Name);
            }

            [Fact]
            public void TheExistingCustomerShouldMaintainTheSameIdentifier()
            {
                Assert.Equal(this.identifier, this.existingCustomer.Id);
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
        }

        private class Customer
        {
            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
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

            public HttpResponseMessage Delete(int id)
            {
                return this.DeleteEntityResponse(id);
            }

            public HttpResponseMessage Get(int id)
            {
                return this.GetEntityResponse(id);
            }

            public virtual HttpResponseMessage Post(Customer entity)
            {
                return this.PostEntityResponse(entity);
            }

            public HttpResponseMessage Put(int id, Customer entity)
            {
                return this.PutEntityResponse(id, entity);
            }
        }
    }
}