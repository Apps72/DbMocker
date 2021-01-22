using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    public partial class MockTable
    {
        public const BindingFlags DefaultFromTypeBindingFlags = BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public;

        /// <summary />
        public static MockTable FromType<T>(
            IEnumerable<T> rows = null,
            BindingFlags propertyBindingFlags = DefaultFromTypeBindingFlags
        )
        {
            var propertyInfos = typeof(T).GetProperties(propertyBindingFlags);

            var columns = propertyInfos
                .Select(propertyInfo => (propertyInfo.Name, propertyInfo.PropertyType))
                .ToArray();

            var table = WithColumns(columns);

            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var values = propertyInfos
                        .Select(pi => pi.GetValue(row))
                        .ToArray();

                    table.AddRow(values);
                }
            }

            return table;
        }
    }
}
