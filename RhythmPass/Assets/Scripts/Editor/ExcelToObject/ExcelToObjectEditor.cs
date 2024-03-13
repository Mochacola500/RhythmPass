using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
namespace Dev.EditorCode
{
    using Dev.Data;
    using Dev.Data.Utility;
    using DataTableGenerator;
    public class ExcelToObjectEditor : EditorWindow
    {
        [MenuItem("Dev/Data/ExcelToObject")]
        static void Open()
        {
            GetWindow<ExcelToObjectEditor>().Show();
        }

        DataLoadAsset _asset;

        private void OnEnable()
        {
            _asset = AssetDatabase.LoadAssetAtPath("Assets/EditorResource/DataLoadAsset.asset", typeof(DataLoadAsset)) as DataLoadAsset;
        }

        private void OnGUI()
        {
            _asset = EditorGUILayout.ObjectField("LoadAsset", _asset, typeof(DataLoadAsset),false) as DataLoadAsset;

            if(GUILayout.Button("Generate All"))
            {
                CreateCSFile();
                CreateJsonFiles();
            }

            if(GUILayout.Button("Generate Json"))
            {
                CreateJsonFiles();
            }
        }
        void CreateCSFile()
        {
            List<DataTableDefinition> tableDefs = new List<DataTableDefinition>();

            if (ExcelReader.ReadDataTableDefinitions(AssetDatabase.GetAssetPath(_asset.ExcelFolder), tableDefs))
            {
                if (CsWriter.WriteDataTableDefinitions(AssetDatabase.GetAssetPath(_asset.CodeFolder) + "\\DataTable", tableDefs))
                {
                    if (CsWriter.WriteDataTableManager(AssetDatabase.GetAssetPath(_asset.CodeFolder), tableDefs))
                    {
                        CompilationPipeline.RequestScriptCompilation();
                        AssetDatabase.Refresh();
                        Debug.Log("Generate Code succeeded");
                    }
                }
            }
        }
        void CreateJsonFiles()
        {
            string excelFolderPath = AssetDatabase.GetAssetPath(_asset.ExcelFolder);
            string jsonFolderPath = AssetDatabase.GetAssetPath(_asset.JsonFolder);

            DirectoryInfo directoryInfo = new DirectoryInfo(excelFolderPath);
            string xlsx = ".xlsx";
            List<Dev.Data.Table> tableList = new List<Table>();
            List<string> jsonPaths = new List<string>();
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                //같으면 0을 반환함
                if (fileInfo.Extension.ToLower().CompareTo(xlsx) == 0)
                {
                    Table[] tables = TableStream.LoadTablesByXLSX(fileInfo.FullName);
                    string[] jsons = new string[tables.Length];
                    for (int i = 0; i < tables.Length; ++i)
                    {
                        jsons[i] = jsonFolderPath + "/" + tables[i].name + ".json";
                        TableStream.WriteJsonByTable(jsons[i], tables[i]);
                    }
                    tableList.AddRange(tables);
                    jsonPaths.AddRange(jsons);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}