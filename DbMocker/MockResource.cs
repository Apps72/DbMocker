using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockResource
    {
        /// <summary>
        /// Convert the fixed fields string to a MockResource, with a MockTable.
        /// </summary>
        /// <param name="content"></param>
        public MockResource(string content)
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
                DataType = dataTypes.ContainsKey(i) ? dataTypes[i] : null
            });

            // Rows
            var rows = new List<string[]>();
            for (int i = 2; i < this.Lines.Count(); i++)
            {
                var newRow = SplitAtPositions(this.Lines.ElementAt(i), positions).Select(item => item.Value);
                rows.Add(newRow.ToArray());
            }
            this.Rows = rows;
        }

        /// <summary>
        /// Convert the fixed fields text embedded file to a MockResource, with a MockTable.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName"></param>
        public MockResource(Assembly assembly, string resourceName)
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
        public IEnumerable<string[]> Rows { get; }

        /// <summary>
        /// Gets fields description
        /// </summary>
        public IEnumerable<Field> Fields { get; }

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
            public string DataType { get; internal set; }
        }
    }
}
