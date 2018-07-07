using System;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    public class MockDbParameter : DbParameter
    {
        internal MockDbParameter(DbParameter parameter)
        {
            if (parameter != null)
            {
                this.DbType = parameter.DbType;
                this.Direction = parameter.Direction;
                this.IsNullable = parameter.IsNullable;
                this.ParameterName = parameter.ParameterName;
                this.Size = parameter.Size;
                this.SourceColumn = parameter.SourceColumn;
                this.SourceColumnNullMapping = parameter.SourceColumnNullMapping;
                this.SourceVersion = parameter.SourceVersion;
                this.Value = parameter.Value;
            }
        }

        internal MockDbParameter(DbCommand command) { }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override int Size { get; set; }
        public override string SourceColumn { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override DataRowVersion SourceVersion { get; set; }
        public override object Value { get; set; }

        public override void ResetDbType()
        {
        }
    }
}
