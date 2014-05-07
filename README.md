MicroLite.Extensions.WebApi
===========================

_MicroLite.Extensions.WebApi_ is an extension to the MicroLite ORM Framework which allows integration with ASP.NET WebApi.

It is easy to use MicroLite with ASP.NET WebApi, simply supply your controller with an `ISession` or `IReadOnlySession` and use it in your controller actions. However, using the WebApi extension for MicroLite makes it even easier and contains some useful extras.

In order to use the WebApi extension for the MicroLite ORM framework, you need to reference it in your solution. The easiest way to do this is install it via NuGet (if you are unfamiliar with NuGet, it's a package manager for Visual Studio - visit [nuget.org](http://www.nuget.org/) for more information).

    Install-Package MicroLite.Extensions.WebApi

## Configuring the Extension

Register the action filters for the behaviour you want in your `WebApiConfig` in the App_Start folder of your project:

    public static void Register(HttpConfiguration config)
    {
        ...

        // Add the MicroLite filters in the following order (only add the ones you want based upon the descriptions of their behaviour below)
        config.Filters.Add(new ValidateModelNotNullAttribute());
        config.Filters.Add(new ValidateModelStateAttribute());
        config.Filters.Add(new MicroLiteSessionAttribute("ConnectionName"));
        config.Filters.Add(new AutoManageTransactionAttribute());
    }

Create a `MicroLiteConfig` class in your App_Start folder using the following template:

    public static class MicroLiteConfig
    {
        public static void ConfigureConnection()
        {
            // Load any MicroLite extensions
            Configure
                .Extensions() // if you are also using a logging extension, register it first
                .WithWebApi();
        }

        public static void ConfigureExtensions()
        {
            // Create session factories for any connections
            Configure
                .Fluently()
                .For...Connection("ConnectionName")
                .CreateSessionFactory();
        }
    }

Call the `MicroLiteConfig` from the application start method in your Global.asax:

    protected void Application_Start()
    {
        // Register routes, bundles etc
        WebApiConfig.Register(GlobalConfiguration.Configuration);
        ...

        // Always configure extensions before connections.
        MicroLiteConfig.ConfigureExtensions();
        MicroLiteConfig.ConfigureConnection();
    }

Lastly, implement `IHaveSession` or `IHaveReadOnlySession` - either directly or by inheriting from the `MicroLiteApiController` or `MicroLiteReadOnlyApiController`.

## What do the MicroLite Filters do?

### ValidateModelNotNullAttribute

The `ValidateModelNotNullAttribute` will throw an `ArgumentNullException` if the model in a request is null, it saves having to write `if (model == null) { throw new ArgumentNullException("model"); }` at the start of every action.

You may not want the attribute to apply to a certain action, therefore the `ValidateModelNotNullAttribute` has a `SkipValidation` property (false by default) which allows you to opt out individual actions.

    [ValidateModelNotNull(SkipValidation = true)] // will only affect the action it is applied to
    public HttpResponseMessage Edit(int id, Model model) { ... }

### ValidateModelStateAttribute

The `ValidateModelStateAttribute` validates the model state in a request and returns a HTTP 400 Bad Request if it is invalid, it saves having to wrap all your methods with `if (ModelState.IsValid())`.

There are times when you may want to opt out individual actions:

    [ValidateModelState(SkipValidation = true)] // will only affect individual methods
    public HttpResponseMessage Edit(int id, Model model) { ... }

### MicroLiteSessionAttribute

The `MicroLiteSessionAttribute` will ensure that an `ISession` or `IReadOnlySession` is opened and assigned to your controller if it inherits from `MicroLiteApiController`/`MicroLiteReadOnlyApiController` or implements `IHaveSession`/`IHaveReadOnlySession` prior to an action being called. It will then ensure that the session is disposed of after the action has been called. When constructed, it requires the name of the connection the session should be opened for.

NOTE: If you use an IOC container to manage sessions, then you do not need to use the MicroLiteSessionAttribute.

### AutoManageTransactionAttribute

The `AutoManageTransactionAttribute` will ensure that a transaction is begun for the session provided to a controller if it inherits from `MicroLiteApiController`/`MicroLiteReadOnlyApiController` or implements `IHaveSession`/`IHaveReadOnlySession` prior to an action being called. It will then ensure that the transaction is either rolled back or committed depending on whether the action results in an exception being thrown or not.

There are times when you may want to manually manage the transaction so it is easy to opt out at either action or controller level:

    [AutoManageTransaction(AutoManageTransaction = false)] // will affect all methods in the controller
    public class CustomerController : MicroLiteApiController { ... }

    [AutoManageTransaction(AutoManageTransaction = false)] // will only affect individual methods
    public HttpResponseMessage Edit(int id, Model model) { ... }

## Controllers

The extension includes 2 basic controllers, `MicroLiteApiController` and `MicroLiteReadOnlyApiController` which define an `ISession` or `IReadOnlySession` property called `Session`.

    public class CustomerApiController : MicroLiteApiController // This will provide an ISession
    {
        ...
    }

    public class OrderApiController : MicroLiteReadOnlyApiController // This will provide an IReadOnlySession
    {
        ...
    }

The extension also contains a more capable "entity" controller which adds CRUD capabilities with minimal boiler plate code. Simply inherit from the new controller and add public methods to opt-in to whichever actions you wish the controller to have.

    public class CustomerApiController : MicroLiteApiController<Customer, int>
    {
        public HttpResponseMessage Delete(int id)
        {
            return this.DeleteEntityResponse(id);
        }

        public HttpResponseMessage Get(int id)
        {
            return this.GetEntityResponse(id);
        }

        public HttpResponseMessage Post(Customer entity)
        {
            return this.PostEntityResponse(entity);
        }

        public HttpResponseMessage Put(int id, Customer entity)
        {
            return this.PutEntityResponse(id, entity);
        }
    }

For further info, visit the [Blog Post](http://microliteorm.wordpress.com/2013/08/26/webapi-3-0-microliteapicontroller-update/).

## OData

The extension also contains a controller which enables OData query support in addition to the CRUD support

    public class CustomerApiController : MicroLiteODataApiController<Customer, int>
    {
        public HttpResponseMessage Get(ODataQueryOptions queryOptions)
        {
            return this.GetEntityResponse(queryOptions);
        }
    }

Again, this is an opt-in. Check the [OData Support](http://microliteorm.wordpress.com/2013/09/04/webapi-3-0-odata-support/) and [OData Filtering](http://microliteorm.wordpress.com/2013/09/06/webapi-odata-filtering/) blog posts for further information.

Check out the [WebApi](http://microliteorm.wordpress.com/tag/webapi/) posts on the MicroLite Blog for further information.

## Supported .NET Framework Versions

The NuGet Package contains binaries compiled against:

* .NET 4.0 (Full)
* .NET 4.5

## Supported ASP.NET WebApi Versions

* ASP.NET WebApi 4 onwards
* WebHost or SelfHost