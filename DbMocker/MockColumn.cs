using System;
using System.Diagnostics;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    [DebuggerDisplay("Name")]
    public class MockColumn
    {
        /// <summary />
        public MockColumn() : this(string.Empty, typeof(object))
        {
        }

        /// <summary />
        public MockColumn(string name) : this(name, typeof(object))
        {
        }

        /// <summary />
        public MockColumn(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary />
        public string Name { get; set; }

        /// <summary />
        public Type Type { get; set; }

        /// <summary />
        public static implicit operator string(MockColumn column)
        {
            return column.Name;
        }

        /// <summary />
        public static implicit operator MockColumn(string name)
        {
            return new MockColumn()
            {
                Name = name,
                Type = typeof(object)
            };
        }
    }

    public static class Columns
    {
        public static MockColumn[] WithNames(params string[] names)
        {
            return names.Select(name => new MockColumn(name)).ToArray();
        }
    }
}
