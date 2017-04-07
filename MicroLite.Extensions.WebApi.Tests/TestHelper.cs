namespace MicroLite.Extensions.WebApi.Tests.OData
{
    using Net.Http.WebApi.OData.Model;
    using TestEntities;

    internal static class TestHelper
    {
        internal static void EnsureEDM()
        {
            if (EntityDataModel.Current == null)
            {
                var entityDataModelBuilder = new EntityDataModelBuilder();
                entityDataModelBuilder.RegisterEntitySet<Customer>("Customers");
                entityDataModelBuilder.RegisterEntitySet<Invoice>("Invoices");

                entityDataModelBuilder.BuildModel();
            }
        }
    }
}