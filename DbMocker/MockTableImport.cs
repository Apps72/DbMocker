using Apps72.Dev.Data.DbMocker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    public partial class MockTable
    {
        /// <summary />
        public static MockTable FromCsv(string content, string delimiter, CsvImportOptions options)
        {
            var table = MockTable.Empty();
            Type[] types = null;
            bool isFirstRow = true;          // First row = Column names
            bool isFirstDataRow = false;     // Second row = First data row
            bool mustRemoveEmptyLines = (options & CsvImportOptions.RemoveEmptyLines) == CsvImportOptions.RemoveEmptyLines;
            bool mustTrimLines = (options & CsvImportOptions.TrimLines) == CsvImportOptions.TrimLines;

            foreach (string row in content.Split(Environment.NewLine))
            {
                if (mustRemoveEmptyLines && string.IsNullOrEmpty(row))
                {

                }
                else
                {
                    string[] data;

                    if (mustTrimLines)
                        data = row.Trim().Split(delimiter);
                    else
                        data = row.Split(delimiter);

                    if (isFirstRow)
                        table.AddColumns(data);
                    else
                    {
                        if (isFirstDataRow)
                            types = GetTypesOfFirstDataRow(data);

                        table.AddRow(ConvertStringToTypes(data, types));
                    }

                    isFirstRow = false;
                    isFirstDataRow = !isFirstDataRow ? true : isFirstDataRow;
                }
            }

            return table;
        }

        /// <summary />
        public static MockTable FromCsv(string content, string delimiter)
        {
            return FromCsv(content, delimiter, CsvImportOptions.RemoveEmptyLines | CsvImportOptions.TrimLines);
        }

        /// <summary />
        public static MockTable FromCsv(string content)
        {
            return FromCsv(content, "\t");
        }

        /// <summary />
        private static Type[] GetTypesOfFirstDataRow(string[] values)
        {
            return values.Select(i => i.BestType()).ToArray();
        }

        /// <summary />
        private static object[] ConvertStringToTypes(string[] values, Type[] types)
        {
            var result = new List<object>();

            for (int i = 0; i < values.Length; i++)
            {
                if (i < types.Length)
                    result.Add(Convert.ChangeType(values[i], types[i]));
                else
                    result.Add(values[i]);
            }

            return result.ToArray();
        }
    }

    public enum CsvImportOptions
    {
        None = 0,
        RemoveEmptyLines = 1,
        TrimLines = 2,
    }
}
