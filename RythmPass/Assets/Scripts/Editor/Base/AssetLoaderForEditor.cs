#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dev.EditorCode
{
    public static class AssetLoaderForEditor
    {
        public static List<T> LoadAllAssetsOfType<T>(string folderPath, string extension, SearchOption option = SearchOption.AllDirectories) where T : UnityEngine.Object
        {

            if (folderPath != string.Empty)
            {
                if (folderPath.EndsWith("/"))
                {
                    folderPath = folderPath.TrimEnd('/');
                }
            }

            List<T> result = new List<T>();

            DirectoryInfo info = new DirectoryInfo(folderPath);
            var index = extension.LastIndexOf('.') + 1;
            if (index >= extension.Length)
                return result;

            extension = extension.Substring(index);
            FileInfo[] files = info.GetFiles(string.Format("*.{0}", extension), option);
            foreach (var file in files)
            {
                string fullPath = file.FullName.Replace(@"\", "/");
                string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset != null)
                {
                    result.Add(asset);
                }
            }

            info = null;
            files = null;
            GC.Collect();

            return result;
        }
    }
}

#endif