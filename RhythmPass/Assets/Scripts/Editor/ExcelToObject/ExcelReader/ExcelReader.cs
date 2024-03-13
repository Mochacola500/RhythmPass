using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DataTableGenerator
{
    using Dev.Data;
    public static class ExcelReader
    {
        public static bool ReadDataTableDefinitions(string directoryPath, List<DataTableDefinition> definitions)
        {
            if (null == directoryPath || 0 == directoryPath.Length || null == definitions)
                return false;

            definitions.Clear();
            
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            foreach (var file in fileInfos)
            {
                if (file.Extension == ".xlsx")
                {
                    Console.WriteLine($"read {file.Name}.");

                    Table[] tables = TableStream.LoadTablesByXLSX(file.FullName);
                    foreach(var table in tables)
                    {
                        DataRecordDefinition recordDefinition = new DataRecordDefinition();
                        for(int i =0; i < table.colCount; ++i)
                        {
                            DataFieldDefinition fieldDefinition = new DataFieldDefinition();
                            fieldDefinition.COLUMN_NAME = table.dataNames[i];
                            fieldDefinition.DATA_TYPE = table.typeNames[i];
                            recordDefinition.rows.Add(fieldDefinition);
                        }
                        definitions.Add(new DataTableDefinition(table.name, recordDefinition));
                    }
                }
            }

            return true;

        }
    }
}
