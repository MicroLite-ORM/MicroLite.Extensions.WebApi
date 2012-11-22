MicroLite.Extensions.Mvc
========================

An extension project for MicroLite ORM to integrate with ASP.NET WebApi

1. Install via NuGet `Install-Package MicroLite.Extensions.WebApi`
2. Load the extension in the application startup `Configure.Extensions().WithWebApi();`
3. Make your controllers inherit from `MicroLiteApiController`
4. Add the `MicroLiteSessionAttribute` to methods which require an `ISession`

To find out more, head over to the wiki!