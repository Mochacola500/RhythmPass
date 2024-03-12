using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dev.Data
{
    using Dev.Data.Utility;
    public static class TableStream
    {
        struct LoadInfo
        {
            public string Path;
            public string JsonPath;
            public string KeyTypeName;
            public string ValueTypeName;
            public string ValueName;

            public LoadInfo(string path, string jsonPath , string keyTypeName, string valueTypeName,string valueName)
            {
                Path = path;
                JsonPath = jsonPath;
                KeyTypeName = keyTypeName;
                ValueTypeName = valueTypeName;
                ValueName = valueName;
            }
        }

        public static Table[] LoadTablesByXLSX(string path)
        {
            Table[] result = null;

            byte[] bin = File.ReadAllBytes(path);
            using (MemoryStream stream = new MemoryStream(bin))
            using (ExcelPackage excelPakage = new ExcelPackage(stream))
            {
                ExcelWorkbook workBook = excelPakage.Workbook;
                result = new Table[workBook.Worksheets.Count];
                int index = 0;
                foreach (ExcelWorksheet sheet in workBook.Worksheets)
                {
                    result[index] = Table.Create(sheet);
                    ++index;
                }
            }

            return result;
        }

        public static Table LoadTableByTSV(string path)
        {
            string splitFileName = @"/";
            string[] arrSplit = Regex.Split(path, splitFileName);
            string fileName = arrSplit[arrSplit.Length - 1];
            
            using (FileStream stream = File.Open(path,FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                string body = reader.ReadToEnd();
                return Table.Create(fileName, body);
            }
        }

        public static void WriteTSVByTable(string writePath,Table table)
        {
            using (StreamWriter stream = new StreamWriter(writePath) )
            {
                const string tabToken = "\t";
                const string openBracket = "(";
                const string closeBracket = ")";

                StringBuilder builder = new StringBuilder();
                for(int i = 0; i < table.dataNames.Length; ++i)
                {
                    builder.Append(table.dataNames[i]);
                    builder.Append(openBracket);
                    builder.Append(table.typeNames[i]);
                    builder.Append(closeBracket);
                    if (i != table.dataNames.Length - 1)
                        builder.Append(tabToken);
                }

                for(int y=0; y < table.rowCount; ++y)
                {
                    builder.AppendLine();
                    for (int x = 0;x < table.colCount; ++x)
                    {
                        builder.Append(table.data[y, x]);
                        if(x != table.colCount - 1)
                            builder.Append(tabToken);
                    }
                }

                stream.Write(builder.ToString());
            }
        }

        public static void WriteJsonByTable(string writePath, Table table)
        {
            using (StreamWriter stream = new StreamWriter(writePath))
            {
                JObject json = new JObject();
                JArray listJson = new JArray();

                for(int y =0; y < table.rowCount; ++y)
                {
                    JObject colObject = new JObject();
                    for(int x= 0;x < table.colCount; ++x)
                    {
                        colObject.Add(table.dataNames[x], table.data[y, x]);
                    }
                    listJson.Add(colObject);
                }

                json.Add("rows", listJson);

                stream.Write(json.ToString());
            }
        }

        public static void SaveJson<T>(string path,Dictionary<int,T> dic)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                var arr = dic.Values;
                stream.Write(JsonConvert.SerializeObject(arr));
            }
        }
    }
    
}