using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Moq;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests
{
    public class ValidateModelNotNullAttributeTests
    {
        public class WhenCallingOnActionExecuting_AndTheActionArgumentsDoesNotContainNull
        {
            private readonly ValidateModelNotNullAttribute _attribute = new ValidateModelNotNullAttribute();

            [Fact]
            public void TheResponseShoulNotBeSet()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ActionArguments.Add("model", new object());

                _attribute.OnActionExecuting(actionContext);

                Assert.Null(actionContext.Response);
            }
        }

        public class WhenCallingOnActionExecuting_TheActionArgumentsContainNull_AndSkipValidationIsFalse
        {
            private readonly ValidateModelNotNullAttribute _attribute = new ValidateModelNotNullAttribute
            {
                SkipValidation = false
            };

            [Fact]
            public void TheResponseStatusCodeShouldBeSetToBadRequest()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ActionArguments.Add("model", null);

                _attribute.OnActionExecuting(actionContext);

                Assert.Equal(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
            }
        }

        public class WhenCallingOnActionExecuting_TheActionArgumentsContainNull_AndSkipValidationIsTrue
        {
            private readonly ValidateModelNotNullAttribute _attribute = new ValidateModelNotNullAttribute
            {
                SkipValidation = true
            };

            [Fact]
            public void TheResponseShouldNotBeSet()
            {
                var controllerContext = new HttpControllerContext(new HttpConfiguration(), new Mock<IHttpRouteData>().Object, new HttpRequestMessage());
                var actionContext = new HttpActionContext(controllerContext, new Mock<HttpActionDescriptor>().Object);
                actionContext.ActionArguments.Add("model", null);

                _attribute.OnActionExecuting(actionContext);

                Assert.Null(actionContext.Response);
            }
        }
    }
}
