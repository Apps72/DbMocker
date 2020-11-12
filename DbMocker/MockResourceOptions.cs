using System;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public static class MockResourceOptions
    {
        /// <summary>
        /// Gets or sets the separator used to split Resource name to Tag and multiple-resources.
        /// </summary>
        public static string TagSeparator { get; set; } = "-";

        /// <summary>
        /// Gets or sets the extension of resources
        /// </summary>
        public static string Extension { get; set; } = "txt";
    }
}
