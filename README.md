# MicroLite.Extensions.WebApi

## Status

|Service|Status|
|-------|------|
|nuget|[![NuGet version](https://badge.fury.io/nu/MicroLite.Extensions.WebApi.svg)](http://badge.fury.io/nu/MicroLite.Extensions.WebApi)|
|/develop|[![Build Status](https://dev.azure.com/trevorpilley/MicroLite-ORM/_apis/build/status/MicroLite-ORM.MicroLite.Extensions.WebApi?branchName=develop)](https://dev.azure.com/trevorpilley/MicroLite-ORM/_build/latest?definitionId=28&branchName=develop)|
|/master|[![Build Status](https://dev.azure.com/trevorpilley/MicroLite-ORM/_apis/build/status/MicroLite-ORM.MicroLite.Extensions.WebApi?branchName=master)](https://dev.azure.com/trevorpilley/MicroLite-ORM/_build/latest?definitionId=28&branchName=master)|

## Installation

Install the nuget package `Install-Package MicroLite.Extensions.WebApi`

## Summary

MicroLite.Extensions.WebAp is a .NET 4.5 library which adds an extension for the MicroLite ORM Framework to integrate with ASP.NET WebApi.

It is easy to use MicroLite with ASP.NET WebApi, simply supply your controller with a Session `IAsyncSession` or `IAsyncReadOnlySession` and use it in your controller actions. However, using the WebApi extension for MicroLite makes it even easier and contains some useful extras.

To find out more, head over to the [Wiki](https://github.com/MicroLite-ORM/MicroLite.Extensions.WebApi/wiki).

Also, check out the [WebApi](http://microliteorm.wordpress.com/tag/WebApi/) tag on the MicroLite Blog.

For OData support, check out the [MicroLite.Extensions.WebApi.OData](https://github.com/MicroLite-ORM/MicroLite.Extensions.WebApi.OData) add-on.

## Supported .NET Versions

The NuGet Package contains binaries compiled against (dependencies indented):

* .NET Framework 4.5
  * MicroLite 7.0.0
  * Microsoft.AspNet.WebApi.Core 5.2.7
