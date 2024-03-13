using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace Dev.EditorCode
{
    using UI;
    using Data;    

    [CustomEditor(typeof(LocalizeText)), CanEditMultipleObjects]
    public class LocalizeTextInspector : Editor
    {
        SerializedProperty tableKey;
        SerializedProperty activate;
        SerializedProperty textUi;

        LocalizeText localizeText;
        public static DescTable descTable { get; private set; }

        public void Load()
        {
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Deploy/DataTable/Desc.json");
            if(textAsset != null)
            {
                DescRecordList recordList = JsonConvert.DeserializeObject<DescRecordList>(textAsset.text);
                foreach (var row in recordList.rows)
                {
                    if (!descTable.records.ContainsKey(row.ID))
                        descTable.records.Add(row.ID, row);
                };
            }
        }

        private void OnEnable()
        {
            tableKey = serializedObject.FindProperty("tableKey");
            activate = serializedObject.FindProperty("activate");
            textUi = serializedObject.FindProperty("textUi");
        }

        DescRecord GetText(int key)
        {
            return descTable.GetRecord(key);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(textUi);
            }

            if (null != descTable)
            {
                EditorGUILayout.PropertyField(activate);

                EditorGUI.BeginChangeCheck();
                tableKey.intValue = EditorGUILayout.IntField("LocalizeKey", tableKey.intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    var desc = GetText(tableKey.intValue);
                    activate.boolValue = null != desc;
                }

                if (-1 != tableKey.intValue)
                {
                    var desc = GetText(tableKey.intValue);
                    string text = string.Empty;
                    if (desc == null)
                    {
                        EditorGUILayout.LabelField(string.Format("NoData Key = {0}", tableKey.intValue));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(string.Format("Kr = {0}", desc.Kor));
                        EditorGUILayout.LabelField(string.Format("Eng = {0}", desc.Eng));
                        if (GUILayout.Button("Text Substitute"))
                        {
                            var targetText = new SerializedObject(textUi.objectReferenceValue);
                            targetText.Update();
                            var prop = targetText.FindProperty("m_Text");
                            if(null != prop)
                            {
                                prop.stringValue = desc.Kor;
                            }
                            targetText.ApplyModifiedProperties();
                        }
                    }
                }

            }

            if (GUILayout.Button("Load"))
            {
                descTable = new Data.DescTable();
                Load();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
