namespace MicroLite.Extensions.WebApi.Query.Binders
{
    using System.Linq;
    using MicroLite.Mapping;
    using MicroLite.Query;

    internal static class SelectBinder
    {
        public static IWhereOrOrderBy BindSelectQueryOption<T>(this ODataQueryOptions queryOptions)
        {
            string[] columnNames;

            if (queryOptions.Select == null || (queryOptions.Select.Properties.Count == 1 && queryOptions.Select.Properties[0] == "*"))
            {
                columnNames = new string[] { "*" };
            }
            else
            {
                var objectInfo = ObjectInfo.For(typeof(T));

                columnNames = queryOptions
                    .Select
                    .Properties
                    .Select(s => objectInfo.TableInfo.Columns.Single(c => c.PropertyInfo.Name == s).ColumnName)
                    .ToArray();
            }

            return SqlBuilder.Select(string.Join(", ", columnNames)).From(typeof(T));
        }
    }
}