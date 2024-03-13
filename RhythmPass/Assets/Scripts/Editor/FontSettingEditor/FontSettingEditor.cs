using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Dev.EditorCode
{
    public class FontSettingEditor : EditorWindow
    {
        [MenuItem("Dev/Font/UI Setting")]
        static void Open()
        {
            GetWindow<FontSettingEditor>().Show();
        }

        GameObject _uiPrefab;
        Font _fontAsset;  

        private void OnGUI()
        {
            _uiPrefab = EditorGUILayout.ObjectField("UI Prefab", _uiPrefab, typeof(GameObject), false, null) as GameObject;
            _fontAsset = EditorGUILayout.ObjectField("Font Asset", _fontAsset, typeof(Font), false, null) as Font;

            if (null != _uiPrefab && null != _fontAsset)
            {
                if (GUILayout.Button("Confirm"))
                {
                    var texts = _uiPrefab.GetComponentsInChildren<Text>();
                    foreach (var text in texts)
                    {
                        text.font = _fontAsset;
                    }
                    AssetDatabase.SaveAssetIfDirty(_uiPrefab);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}