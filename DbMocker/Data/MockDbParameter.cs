using System;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker.Data
{
    /// <summary />
    public class MockDbParameter : DbParameter
    {
        private bool _hasDbTypeIsDefined = false;       // True when the DbType property is already setted and defined.
        private object _value = null;
        private DbType _dbType = DbType.AnsiString;
        private DbParameter _parameter;
        /// <summary />
        public MockDbParameter()
        {
        }

        /// <summary />
        internal MockDbParameter(DbParameter parameter)
        {
            if (parameter != null)
            {
                _parameter = parameter;
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

        #region LEGACY METHODS

        /// <summary />
        internal MockDbParameter(DbCommand command) { }
        /// <summary />
        public override DbType DbType
        {
            get
            {
                return _dbType;
            }
            set
            {
                _dbType = value;
                _hasDbTypeIsDefined = true;
            }
        }

        /// <summary />
        public override ParameterDirection Direction { get; set; }
        /// <summary />
        public override bool IsNullable { get; set; }
        /// <summary />
        public override string ParameterName { get; set; }
        /// <summary />
        public override int Size { get; set; }
        /// <summary />
        public override string SourceColumn { get; set; }
        /// <summary />
        public override bool SourceColumnNullMapping { get; set; }
        /// <summary />
        public override DataRowVersion SourceVersion { get; set; }
        /// <summary />
        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                // set value to original dbParameter
                if (_parameter != null)
                    _parameter.Value = value; 

                _value = value;
                if (_hasDbTypeIsDefined == false)
                    this.DbType = Helpers.DbTypeMap.FirstDbType(value?.GetType());
            }
        }
        /// <summary />
        public override void ResetDbType()
        {
        }

        #endregion
    }
}
