MicroLite.Extensions.WebApi
===========================

An extension project for MicroLite ORM to integrate with ASP.NET WebApi

1. Install via NuGet `Install-Package MicroLite.Extensions.WebApi`
2. Load the extension in the application startup `Configure.Extensions().WithWebApi(WebApiConfigurationSettings.Default);` prior to calling `Conflgure.Fluently()...`
3. Inherit your controllers from `MicroLiteApiController` or `MicroLiteReadOnlyApiController`

If you don't want to use the defaults, you can set whichever properties `WebApiConfigurationSettings` offers explicitly.

To find out more, head over to the wiki!
