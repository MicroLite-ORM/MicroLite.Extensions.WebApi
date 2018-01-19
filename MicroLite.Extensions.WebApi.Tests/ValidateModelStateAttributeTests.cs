namespace MicroLite.Extensions.WebApi.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Routing;
    using MicroLite.Extensions.WebApi;
    using Moq;
    using Xunit;

    public class ValidateModelStateAttributeTests
    {
        public class WhenCallingOnActionExecuting_AndTheModelStateDoesNotContainErrors
        {
            private readonly ValidateModelStateAttribute attribute = new ValidateModelStateAttribute();

            [Fact]
            public void TheResponseShouldNotBeSet()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ModelState.Clear();

                attribute.OnActionExecuting(actionContext);

                Assert.Null(actionContext.Response);
            }
        }

        public class WhenCallingOnActionExecuting_TheModelStateContainsErrors_AndSkipValidationIsFalse
        {
            private readonly ValidateModelStateAttribute attribute = new ValidateModelStateAttribute
            {
                SkipValidation = false
            };

            [Fact]
            public void TheResponseStatusCodeShouldBeSetToBadRequest()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ModelState.AddModelError("Foo", "Error");

                attribute.OnActionExecuting(actionContext);

                Assert.Equal(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
            }
        }

        public class WhenCallingOnActionExecuting_TheModelStateContainsErrors_AndSkipValidationIsTrue
        {
            private readonly ValidateModelStateAttribute attribute = new ValidateModelStateAttribute
            {
                SkipValidation = true
            };

            [Fact]
            public void TheResponseShouldNotBeSet()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ModelState.AddModelError("Foo", "Error");

                attribute.OnActionExecuting(actionContext);

                Assert.Null(actionContext.Response);
            }
        }
    }
}
