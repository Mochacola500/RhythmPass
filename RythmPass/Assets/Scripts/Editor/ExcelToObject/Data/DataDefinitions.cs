using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTableGenerator
{
    [System.Serializable]
    public class DataFieldDefinition
    {
        public string COLUMN_NAME = string.Empty;
        public string DATA_TYPE = string.Empty;

        public bool IsDevField()
        {
            if (0 == string.Compare(COLUMN_NAME, 0, DataTableDefinition.DEV_FIELD_PREFIX, 0, 4, true))
                return true;
            return false;
        }
    }

    [System.Serializable]
    public class DataRecordDefinition
    {
        public List<DataFieldDefinition> rows = new List<DataFieldDefinition>();
    }

    public class DataTableDefinition
    {
        public const string KEY_FIELD_NAME = "ID";
        public const string DEV_FIELD_PREFIX = "dev_";

        public string tableName;
        public DataFieldDefinition keyFieldDef = null;
        public DataRecordDefinition recordDef = new DataRecordDefinition();

        public DataTableDefinition(string tableName, DataRecordDefinition recordDef)
        {
            this.tableName = tableName;
            this.recordDef = recordDef;
            SetKeyField();
        }

        public bool HasDevField()
        {
            foreach(var field in recordDef.rows)
            {
                if (true == field.IsDevField())
                    return true;
            }
            return false;
        }

        private void SetKeyField()
        {
            foreach (var field in recordDef.rows)
            {
                if (0 == string.Compare(field.COLUMN_NAME, KEY_FIELD_NAME, true))
                {
                    keyFieldDef = field;
                    break;
                }
            }
        }
    }
}
