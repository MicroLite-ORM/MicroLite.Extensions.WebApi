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
                entityDataModelBuilder.RegisterEntitySet<Customer>("Customers", x => x.Id);
                entityDataModelBuilder.RegisterEntitySet<Invoice>("Invoices", x => x.Id);

                entityDataModelBuilder.BuildModel();
            }
        }
    }
}