using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using MicroLite.Extensions.WebApi.Tests.TestEntities;
using Moq;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests
{
    public class MicroLiteApiController_T_Tests
    {
        public class WhenCallingDeleteAndAnEntityIsDeleted
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingDeleteAndAnEntityIsDeleted()
            {
                _mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), _identifier)).Returns(Task.FromResult(true));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };

                _response = _controller.Delete(_identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, _response.StatusCode);
            }
        }

        public class WhenCallingDeleteAndAnEntityIsNotDeleted
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingDeleteAndAnEntityIsNotDeleted()
            {
                _mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), _identifier)).Returns(Task.FromResult(false));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };

                _response = _controller.Delete(_identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }

        public class WhenCallingGetAndAnEntityIsNotReturned
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingGetAndAnEntityIsNotReturned()
            {
                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult((Customer)null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };

                _response = _controller.Get(_identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }
        }

        public class WhenCallingGetAndAnEntityIsReturned
        {
            private readonly CustomerController _controller;
            private readonly Customer _customer = new Customer();
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingGetAndAnEntityIsReturned()
            {
                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult(_customer));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(_customer, ((ObjectContent)_response.Content).Value);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }
        }

        public class WhenCallingPost
        {
            private readonly CustomerController _controller;
            private readonly Customer _customer = new Customer();
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingPost()
            {
                _mockSession.Setup(x => x.InsertAsync(It.IsNotNull<Customer>()))
                    .Returns(Task.FromResult(0))
                    .Callback((object o) =>
                    {
                        ((Customer)o).Id = _identifier;
                    });

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Post(_customer).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(_customer, ((ObjectContent)_response.Content).Value);
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheUriForTheEntity()
            {
                Assert.Equal(new Uri("http://services.microlite.org/api/Customers/12345"), _response.Headers.Location);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeCreated()
            {
                Assert.Equal(HttpStatusCode.Created, _response.StatusCode);
            }
        }

        public class WhenCallingPutAndAnEntityIsNotReturned
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingPutAndAnEntityIsNotReturned()
            {
                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult((Customer)null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };

                _response = _controller.Put(_identifier, new Customer()).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }
        }

        public class WhenCallingPutAndAnEntityIsNotUpdated
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCallingPutAndAnEntityIsNotUpdated()
            {
                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult(new Customer()));
                _mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(Task.FromResult(false));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };

                _response = _controller.Put(_identifier, new Customer()).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotModified()
            {
                Assert.Equal(HttpStatusCode.NotModified, _response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }
        }

        public class WhenCallingPutAndAnEntityIsUpdated
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            private readonly Customer _updatedCustomer = new Customer
            {
                Name = "Joe Bloggs"
            };

            public WhenCallingPutAndAnEntityIsUpdated()
            {
                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult(new Customer()));
                _mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(Task.FromResult(true));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage()
                };

                _response = _controller.Put(_identifier, _updatedCustomer).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, _response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }

            [Fact]
            public void TheUpdatedCustomerShouldHaveTheIdentifierSet()
            {
                Assert.Equal(_identifier, _updatedCustomer.Id);
            }
        }

        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteApiController<Customer, int> _controller;
            private readonly IAsyncSession _session = new Mock<IAsyncSession>().Object;

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteApiController<Customer, int>>(_session)
                {
                    CallBase = true
                };

                _controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(_session, _controller.Session);
            }
        }

        private class CustomerController : MicroLiteApiController<Customer, int>
        {
            public CustomerController(IAsyncSession session)
                : base(session)
            {
                GetEntityResourceUri = (int id) =>
                {
                    return new Uri("http://services.microlite.org/api/Customers/" + id.ToString());
                };
            }

            public Task<HttpResponseMessage> Delete(int id) => DeleteEntityResponseAsync(id);

            public Task<HttpResponseMessage> Get(int id) => GetEntityResponseAsync(id);

            public Task<HttpResponseMessage> Post(Customer entity) => PostEntityResponseAsync(entity);

            public Task<HttpResponseMessage> Put(int id, Customer entity) => PutEntityResponseAsync(id, entity);
        }
    }
}
