using Apps72.Dev.Data.DbMocker.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    internal class MockTableImportFixed
    {
        /// <summary>
        /// Convert the fixed fields string to a MockResource, with a MockTable.
        /// </summary>
        /// <param name="content"></param>
        public MockTableImportFixed(string content)
        {
            // All lines (without commented rows)
            this.Lines = content.Split(new char[] { '\n', '\r' })
                                .Where(line => String.IsNullOrWhiteSpace(line) == false)
                                .Where(line => line.StartsWith("#") == false);

            // Fields positions
            string columnsRow = this.Lines?.ElementAtOrDefault(0);
            var positions = GetPositions(columnsRow).ToArray();

            // Fields name
            var fieldsNames = SplitAtPositions(columnsRow, positions);

            // Data types
            string dataTypesRow = this.Lines?.ElementAtOrDefault(1);
            var dataTypes = SplitAtPositions(dataTypesRow, positions);

            // Combine to Fields
            this.Fields = positions.Select(i => new Field()
            {
                Position = i,
                Name = fieldsNames.ContainsKey(i) ? fieldsNames[i] : null,
                DataType = dataTypes.ContainsKey(i) ? dataTypes[i].KeepOnlyLetters().NameToType() : null
            });

            // Rows
            var rows = new List<string[]>();
            for (int i = 2; i < this.Lines.Count(); i++)
            {
                var newRow = SplitAtPositions(this.Lines.ElementAt(i), positions).Select(item => item.Value);
                rows.Add(newRow.ToArray());
            }
            this.Rows = GetRowsConverted(rows);
        }

        /// <summary>
        /// Convert the fixed fields text embedded file to a MockResource, with a MockTable.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        public MockTableImportFixed(Assembly assembly, string resourceName)
            : this(ReadResourceFile(assembly, resourceName))
        {

        }

        /// <summary>
        /// Gets all lines included in the resource file.
        /// </summary>
        public IEnumerable<string> Lines { get; }

        /// <summary>
        /// Gets all lines splitted, included in the resource file.
        /// </summary>
        public IEnumerable<object[]> Rows { get; }

        /// <summary>
        /// Gets fields description
        /// </summary>
        public IEnumerable<Field> Fields { get; }

        /// <summary />
        public MockTable GetMockTable()
        {
            var columns = this.Fields.Select(i => (i.Name, i.DataType)).ToArray();

            var table = new MockTable().AddColumns(columns);

            foreach (var row in this.Rows)
            {
                table.AddRow(row);
            }

            return table;
        }


        /// <summary />
        private IEnumerable<object[]> GetRowsConverted(IEnumerable<string[]> rows)
        {
            int fieldsCount = Fields.Count();
            Type[] fieldsType = Fields.Select(i => i.DataType).ToArray();

            foreach (var row in rows)
            {
                var rowConverted = new object[fieldsCount];
                for (int i = 0; i < fieldsCount; i++)
                {
                    // If not a string => String.Empty = NULL
                    if (fieldsType[i] != typeof(string) && String.IsNullOrEmpty(row[i]))
                    {
                        row[i] = "NULL";
                    }

                    if (fieldsType[i] != null && String.Compare(row[i], "NULL", ignoreCase: true) != 0)
                    {
                        // Convert
                        var converter = System.ComponentModel.TypeDescriptor.GetConverter(fieldsType[i]);
                        rowConverted[i] = Convert.ChangeType(converter.ConvertFrom(row[i]), fieldsType[i]);

                        // Guillemets
                        if (fieldsType[i] == typeof(string) && row[i].Length >= 2 && row[i][0] == '"' && row[i][row[i].Length -1] == '"')
                        {
                            rowConverted[i] = row[i].Substring(1, row[i].Length - 2);
                        }
                    }
                }
                yield return rowConverted;
            }
        }

        /// <summary>
        /// Split specified text line to data using fixed positions.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        private Dictionary<int, string> SplitAtPositions(string line, int[] positions)
        {
            var result = new Dictionary<int, string>();
            int numberOfPosition = positions.Length;

            if (!String.IsNullOrEmpty(line))
            {
                for (int i = 0; i < numberOfPosition; i++)
                {
                    int indexFrom = positions[i];
                    int? indexTo = (i + 1) < numberOfPosition
                                 ? positions[i + 1]
                                 : (int?)null;

                    if (indexFrom < line.Length)
                    {
                        string part = indexTo != null && indexTo < line.Length
                                    ? line.Substring(indexFrom, indexTo.Value - indexFrom)
                                    : line.Substring(indexFrom);

                        part = part.Trim();

                        result.Add(indexFrom, part);
                    }
                    else
                    {

                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all index positions of words
        /// "ABC 123" => [0, 4]
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private IEnumerable<int> GetPositions(string line)
        {
            var positions = new List<int>();
            bool findNext = true;

            for (int i = 0; i < line.Length; i++)
            {
                char item = line[i];

                if (item == ' ' || item == '\t')
                    findNext = true;

                if (findNext && item != ' ' && item != '\t')
                {
                    positions.Add(i);
                    findNext = false;
                }
            }

            return positions;
        }

        /// <summary>
        /// Read the resource text file.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private static string ReadResourceFile(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();                
            }
        }

        /// <summary />
        [DebuggerDisplay("[{Position}] {Name} ({DataType})")]
        public class Field
        {
            /// <summary />
            public string Name { get; internal set; }
            /// <summary />
            public int Position { get; internal set; }
            /// <summary />
            public Type DataType { get; internal set; }
        }
    }
}
