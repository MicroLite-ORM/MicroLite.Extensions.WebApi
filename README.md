MicroLite.Extensions.WebApi
===========================

[![NuGet version](https://badge.fury.io/nu/MicroLite.Extensions.WebApi.svg)](http://badge.fury.io/nu/MicroLite.Extensions.WebApi)

_MicroLite.Extensions.WebApi_ is an extension to the MicroLite ORM Framework which allows integration with ASP.NET WebApi.

It is easy to use MicroLite with ASP.NET WebApi, simply supply your controller with an (`IAsyncSession` or `IAsyncReadOnlySession` .NET 4.5 + WebApi 2) / (`ISession` or `IReadOnlySession` .NET 4.0 + WebApi) and use it in your controller actions. However, using the WebApi extension for MicroLite makes it even easier and contains some useful extras.

## Supported .NET Framework Versions

The NuGet Package contains binaries compiled against:

* .NET 4.0 (Client Profile) and Microsoft.AspNet.WebApi.Core (4.0.20710)
* .NET 4.5 and Microsoft.AspNet.WebApi.Core 2 (5.0)

## Supported ASP.NET WebApi Hosts

* WebHost (IIS)
* SelfHost (OWIN)

To find out more, head over to the [Wiki](https://github.com/TrevorPilley/MicroLite.Extensions.WebApi/wiki).

Also, check out the [WebApi](http://microliteorm.wordpress.com/tag/webapi/) tag on the MicroLite Blog.
