using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace DataTableGenerator
{
    public static class JsonReader
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
                if (file.Extension == ".json")
                {
                    Console.WriteLine($"read {file.Name}.");

                    var reader = file.OpenText();
                    string text = reader.ReadToEnd();
                    DataRecordDefinition recordDef = JsonConvert.DeserializeObject<DataRecordDefinition>(text);

                    string tableName = file.Name.Split('.')[0];
                    DataTableDefinition dataTableDef = new DataTableDefinition(tableName, recordDef);
                    definitions.Add(dataTableDef);
                }
            }

            return true;
        }

    }
}
