namespace MicroLite.Extensions.WebApi.Tests.Filters
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;
    using MicroLite.Extensions.WebApi.Filters;
    using Moq;
    using Xunit;

    public class ValidateModelStateAttributeTests
    {
        public class WhenCallingOnActionExecutingAndTheActionArgumentsContainNull
        {
            private readonly ValidateModelStateAttribute attribute = new ValidateModelStateAttribute();

            [Fact]
            public void TheResponseShouldBeSetToBadRequest()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ModelState.AddModelError("Foo", "Error");

                attribute.OnActionExecuting(actionContext);

                Assert.Equal(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
            }
        }

        public class WhenCallingOnActionExecutingAndTheActionArgumentsDoesNotContainNull
        {
            private readonly ValidateModelStateAttribute attribute = new ValidateModelStateAttribute();

            [Fact]
            public void TheResponseShoulNotBeSet()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ModelState.Clear();

                attribute.OnActionExecuting(actionContext);

                Assert.Null(actionContext.Response);
            }
        }
    }
}