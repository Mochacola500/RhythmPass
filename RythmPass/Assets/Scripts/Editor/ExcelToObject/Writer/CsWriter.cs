using System;
using System.Collections.Generic;
using System.IO;
using System.CodeDom;

namespace DataTableGenerator
{
    public static class CsWriter
    {
        public static bool WriteDataTableDefinitions(string directoryPath, List<DataTableDefinition> tableDefs)
        {
            if (null == directoryPath || 0 == directoryPath.Length || null == tableDefs)
                return false;

            for (int i = 0; i < tableDefs.Count; i++)
            {
                if (false == WriteDataTableDefinition(directoryPath, tableDefs[i]))
                    return false;
            }

            return true;
        }

        public static bool WriteDataTableDefinition(string directoryPath, DataTableDefinition tableDef)
        {
            if (null == directoryPath || 0 == directoryPath.Length || null == tableDef)
                return false;

            string filePath = string.Format("{0}\\{1}Data.cs", directoryPath, tableDef.tableName);

            Console.WriteLine($"generate {filePath}.");
           
            if (tableDef.HasDevField())
            {
                List<CodeCompileUnit> codes = new List<CodeCompileUnit>();
                codes.Add(new CodeSnippetCompileUnit("#if DEV"));
                codes.Add(CreateDataTableCode(tableDef, true));
                codes.Add(new CodeSnippetCompileUnit("#else"));
                codes.Add(CreateDataTableCode(tableDef, false));
                codes.Add(new CodeSnippetCompileUnit("#endif"));
                WriteFile(filePath, codes);
            }
            else
            {
                CodeCompileUnit code = CreateDataTableCode(tableDef, false);
                WriteFile(filePath, code);
            }
            return true;
        }

        public static bool WriteDataTableManager(string directoryPath, List<DataTableDefinition> tableDefs)
        {
            if (null == directoryPath || 0 == directoryPath.Length || null == tableDefs)
                return false;
            
            string filePath = string.Format("{0}\\DataTableManagerGen.cs", directoryPath);

            Console.WriteLine($"generate {filePath}.");

            CodeCompileUnit code = CreateDataTableManagerCode(tableDefs);
            WriteFile(filePath, code);
            return true;
        }

        private static CodeCompileUnit CreateDataTableCode(DataTableDefinition tableDef, bool genDevField)
        {
            string tableClassName = string.Format("{0}Table", tableDef.tableName);
            string recordClassName = string.Format("{0}Record", tableDef.tableName);
            string recordListClassName = string.Format("{0}RecordList", tableDef.tableName);
            Type tableKeyFieldType = ConvertDataTypeToCsType(tableDef.keyFieldDef.DATA_TYPE);

            string methodBodyText;

            CodeCompileUnit code = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace("Dev.Data");

            // Imports
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                codeNamespace.Imports.Add(new CodeNamespaceImport("Newtonsoft.Json"));
            }

            // Record class
            {
                CodeTypeDeclaration recordClass = CreateClass(recordClassName, true);
                foreach (var fieldDef in tableDef.recordDef.rows)
                {
                    if (fieldDef.COLUMN_NAME == string.Empty)
                        continue;
                    if (false == genDevField && true == fieldDef.IsDevField())
                        continue;
                    recordClass.Members.Add(CreateClassMemberField(ConvertDataTypeToCsType(fieldDef.DATA_TYPE).ToString(), fieldDef.COLUMN_NAME));
                }
                codeNamespace.Types.Add(recordClass);
            }

            // Record List class
            {
                CodeTypeDeclaration serializedClass = CreateClass(recordListClassName, true);
                serializedClass.Members.Add(CreateClassMemberField(string.Format("List<{0}>", recordClassName), "rows"));
                methodBodyText = string.Format(
                    //"if (DataTableManager.IsGameDataBundleLoad)"
                    "AssetManager.LoadAsync<TextAsset>(file, (textAsset) =>"
                    + "\n\t\t\t{{"
                    + "\n\t\t\t\tif (textAsset == null)"
                    + "\n\t\t\t\t{{"
                    + "\n\t\t\t\t\tcallback?.Invoke(new {0}());"
                    + "\n\t\t\t\t\treturn;"
                    + "\n\t\t\t\t}}"
                    + "\n\t\t\t\t{0} data = JsonConvert.DeserializeObject<{0}>(textAsset.text);"
                    + "\n\t\t\t\tcallback?.Invoke(data);"
                    + "\n\t\t\t}});"
                    , recordListClassName);
                serializedClass.Members.Add(CreateClassMemberMethod("LoadJson", methodBodyText, MemberAttributes.Public | MemberAttributes.Static, null, typeof(string).Name, "file", string.Format("Action<{0}>", recordListClassName), "callback = null"));
                codeNamespace.Types.Add(serializedClass);
            }

            // Table class
            {
                CodeTypeDeclaration tableClass = CreateClass(tableClassName);
                tableClass.Members.Add(CreateClassMemberField(string.Format("Dictionary<{0}, {1}>", tableKeyFieldType.ToString(), recordClassName), "records", true));

                methodBodyText =
                    "if (null == data || null == data.rows)"
                    + "\n\t\t\t\treturn;"
                    + "\n\t\t\tforeach (var row in data.rows)"
                    + "\n\t\t\t{"
                    + "\n\t\t\t\tif (!records.ContainsKey(row.ID))"
                    + "\n\t\t\t\t\trecords.Add(row.ID, row);"
                    + "\n\t\t\t}";
                tableClass.Members.Add(CreateClassMemberMethod("Init", methodBodyText, MemberAttributes.Public, null, recordListClassName, "data"));

                methodBodyText = string.Format(
                    "{0} record;"
                    + "\n\t\t\tif (false == records.TryGetValue(key, out record))"
                    + "\n\t\t\t\treturn null;"
                    + "\n\t\t\treturn record"
                    , recordClassName);
                tableClass.Members.Add(CreateClassMemberMethod("GetRecord", methodBodyText, MemberAttributes.Public, recordClassName, tableKeyFieldType.ToString(), "key"));

                codeNamespace.Types.Add(tableClass);
            }

            code.Namespaces.Add(codeNamespace);
            return code;
        }

        private static CodeCompileUnit CreateDataTableManagerCode(List<DataTableDefinition> tableDefs)
        {
            string methodBodyText;

            CodeCompileUnit code = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace("Dev.Data");

            // Imports
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            }

            CodeTypeDeclaration DataTableManagerClass = CreateClass("DataManager");
            DataTableManagerClass.IsPartial = true;

            // SerializeData class
            {
                CodeTypeDeclaration serializeDataClass = CreateClass("SerializeData", true);
                foreach (var tableDef in tableDefs)
                {
                    if (tableDef.tableName == string.Empty)
                        continue;
                    serializeDataClass.Members.Add(CreateClassMemberField(tableDef.tableName + "RecordList", ToCamelCase(tableDef.tableName + "Data")));
                }
                DataTableManagerClass.Members.Add(serializeDataClass);
            }

            // interface
            {
                methodBodyText = "SerializeData data = new SerializeData();";
                foreach (var tableDef in tableDefs)
                {
                    if (tableDef.tableName == string.Empty)
                        continue;
                    methodBodyText += string.Format("\n\t\t\t{0}RecordList.LoadJson(path + \"/{0}.json\", (sd) => {{ data.{1} = sd; if (CheckLoadComplete(data)) callback.Invoke(data); }});",
                        tableDef.tableName,
                        ToCamelCase(tableDef.tableName + "Data"));
                }
                DataTableManagerClass.Members.Add(CreateClassMemberMethod("LoadDataJson", methodBodyText, MemberAttributes.Public | MemberAttributes.Static, null, typeof(string).Name, "path", "Action<SerializeData>", "callback"));

                methodBodyText = string.Empty;
                foreach (var tableDef in tableDefs)
                {
                    if (tableDef.tableName == string.Empty)
                        continue;
                    methodBodyText += string.Format("\n\t\t\tif (null == data.{0}) return false;", ToCamelCase(tableDef.tableName + "Data"));
                }
                methodBodyText += "\n\t\t\treturn true";
                DataTableManagerClass.Members.Add(CreateClassMemberMethod("CheckLoadComplete", methodBodyText, MemberAttributes.Public | MemberAttributes.Static, typeof(bool).Name, "SerializeData", "data"));
            }
            codeNamespace.Types.Add(DataTableManagerClass);
            code.Namespaces.Add(codeNamespace);
            return code;
        }

        private static CodeTypeDeclaration CreateClass(string name, bool isSerializable = false)
        {
            CodeTypeDeclaration classCode = new CodeTypeDeclaration();
            classCode.IsClass = true;
            classCode.Name = name;
            classCode.TypeAttributes = System.Reflection.TypeAttributes.Public;
            if (isSerializable)
                classCode.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));
            return classCode;
        }

        private static CodeMemberField CreateClassMemberField(string type, string name, bool addCreateExp = false)
        {
            CodeMemberField fieldCode = new CodeMemberField(type, name);
            fieldCode.Attributes = MemberAttributes.Public;
            if (addCreateExp)
                fieldCode.InitExpression = new CodeObjectCreateExpression(type);
            return fieldCode;
        }

        private static CodeConstructor CreateClassConstructor(string bodyExp)
        {
            CodeConstructor constructorCode = new CodeConstructor();
            constructorCode.Attributes = MemberAttributes.Public;
            var body = new CodeSnippetExpression(bodyExp);
            constructorCode.Statements.Add(body);
            return constructorCode;
        }

        private static CodeMemberMethod CreateClassMemberMethod(string name, string bodyExp, MemberAttributes memberAttributes = MemberAttributes.Public,
            string returnType = null, string paramType1 = null, string paramName1 = null, string paramType2 = null, string paramName2 = null)
        {
            CodeMemberMethod methodCode = new CodeMemberMethod();
            methodCode.Attributes = memberAttributes;
            methodCode.Name = name;
            if(null != paramType1 && null != paramName1)
                methodCode.Parameters.Add(new CodeParameterDeclarationExpression(paramType1, paramName1));
            if (null != paramType2 && null != paramName2)
                methodCode.Parameters.Add(new CodeParameterDeclarationExpression(paramType2, paramName2));
            if (null != returnType)
                methodCode.ReturnType = new CodeTypeReference(returnType);
            var body = new CodeSnippetExpression(bodyExp);
            methodCode.Statements.Add(body);
            return methodCode;
        }

        private static string ToCamelCase(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        private static string ToPascalCase(string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        private static Type ConvertDataTypeToCsType(string dataTypeString)
        {
            Type type = null;
            switch (dataTypeString)
            {
                case "bool":
                case "bit":
                    type = typeof(bool);
                    break;
                case "tinyint":
                    type = typeof(byte);
                    break;
                case "smallint":
                    type = typeof(short);
                    break;
                case "int":
                    type = typeof(int);
                    break;
                case "uint":
                    type = typeof(uint);
                    break;
                case "ulong":
                    type = typeof(ulong);
                    break;
                case "long":
                case "bigint":
                    type = typeof(long);
                    break;
                case "float":   //db에서 float타입은 double이라 아마도.. 나중에 서버팀하고 같이 얘기해서 어딘가는 맞춰야..
                case "real":
                    type = typeof(float);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "datetime":
                    type = typeof(long);
                    break;
                case "time":
                    type = typeof(long);
                    break;
                case "string":
                case "nvarchar":
                    type = typeof(string);
                    break;
                default:
                    type = typeof(string);
                    break;
            }
            return type;
        }

        private static void WriteFile(string filePath, CodeCompileUnit codeData)
        {
            Stream stream = File.Open(filePath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(stream);
            System.CodeDom.Compiler.CodeGeneratorOptions option = new System.CodeDom.Compiler.CodeGeneratorOptions();
            option.BracingStyle = "C";
            option.BlankLinesBetweenMembers = false;

            System.CodeDom.Compiler.CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            provider.GenerateCodeFromCompileUnit(codeData, streamWriter, option);
            streamWriter.Close();
        }

        private static void WriteFile(string filePath, List<CodeCompileUnit> codeDatas)
        {
            Stream stream = File.Open(filePath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(stream);
            System.CodeDom.Compiler.CodeGeneratorOptions option = new System.CodeDom.Compiler.CodeGeneratorOptions();
            option.BracingStyle = "C";
            option.BlankLinesBetweenMembers = false;

            System.CodeDom.Compiler.CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            foreach(var codeData in codeDatas)
                provider.GenerateCodeFromCompileUnit(codeData, streamWriter, option);
            streamWriter.Close();
        }
    }
}
