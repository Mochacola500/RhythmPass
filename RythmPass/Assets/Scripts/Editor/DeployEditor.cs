using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Dev.EditorCode
{
    public class DeployEditor : DevEditorWindow
    {
        //private static DeployEditor _deployEditor = null;
        //public static DeployEditor Instance { get { return _deployEditor; } }        
        //public void Awake()
        //{
        //    _deployEditor = this;
        //}
        //public void OnDestroy()
        //{
        //    _deployEditor = null;
        //}

        public static readonly string OriginPath = "Assets/ArtResources/World/Prefabs";
        public static readonly string TileTargetPath = "Assets/Deploy/World/Prefabs/Field";
        public static readonly string WorldObjectTargetPath = "";
        
        private bool _init = false;
        private EditorGUIListPage<AssetInfo> _listGui;
        private List<AssetInfo> _list;
        [MenuItem("Dev/Deploy/Tile")]
        public static void ShowWindow()
        {
            var win = EditorWindow.GetWindow<DeployEditor>();            
            win.Show();
        }        

        public class AssetInfo
        {
            public string Name;
            public string Path;
            public GameObject Obj;
            public bool Check;
        }
        public void Init()
        {
            if (false != _init)
                return;
            
            var assets = AssetLoaderForEditor.LoadAllAssetsOfType<GameObject>(OriginPath, ".prefab");
            _list = (from obj in assets select new AssetInfo() { Name = obj.name, Path = AssetDatabase.GetAssetPath(obj), Obj = obj}).ToList();

            _listGui = new EditorGUIListPage<AssetInfo>(_list, "Name");
            _listGui.onGUI += ListGUI_onGUI;
            _init = true;
        }

        public Vector2 _scroll;
        private void ListGUI_onGUI(List<AssetInfo> data)
        {
            var start = _listGui.StartListIndex;
            var end = _listGui.StartListIndex + _listGui.PageSize;

            if (GUILayout.Button("선택"))
            {
                for (int i = start; i < end; i++)
                {
                    data[i].Check = true;
                }
            }
            

            using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scroll))
            {
                _scroll = scrollScope.scrollPosition;

                for (int i = start; i < end; i++)
                {
                    if (i >= data.Count) break;

                    using( HorizontalScope())
                    {
                        data[i].Check = EditorGUILayout.Toggle(data[i].Check, GUILayout.Width(20f));
                        EditorGUILayout.ObjectField("Temp", data[i].Obj, typeof(GameObject), true);
                    }
                    
                }
            }
              
                
        }

        public void OnGUI()
        {
            Init();

            _listGui?.OnGUI();
            
            using (HorizontalScope())
            {
                if (GUILayout.Button("전체선택"))
                {
                    _list.ForEach(x => x.Check = true);
                }
                if (GUILayout.Button("전체해제"))
                {
                    _list.ForEach(x => x.Check = true);
                }
            }

            if(GUILayout.Button("변환"))
            {
                _list.ForEach(OnParse);
            }
        }

        private void OnParse(AssetInfo info)
        {
            if (false == info.Check)
                return;

            try
            {
                var index = info.Path.LastIndexOf('/') + 1;
                var fileName = info.Path.Substring(index);
                var path = TileTargetPath + $"/{fileName}";
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(info.Path);
                //if(null != obj)
                //{
                //    throw new Exception($"{info.Name}은 {path}에 이미 만들어져있음");
                //}                                
                var root = GameObject.Instantiate(obj);
                var lodGroup = root.GetComponent<LODGroup>();
                var lods = lodGroup.GetLODs();
                if (null == lods || 0 == lodGroup.lodCount)
                    return;

                for (int i = 1; i < lods.Length; i++)
                {
                    var renderers = lods[i].renderers;
                    for (int j = 0; j < renderers.Length; j++)
                    {
                        CoreUtil.Destroy(renderers[j].gameObject);
                    }
                }

                
                                
                var tile = root.AddComponent<TileObject>();
                var child = new GameObject("WorldObjectResource");
                child.transform.SetParent(root.transform);

                var resource = child.AddComponent<WorldObjectResource>();
                for (int i = 0; i < root.transform.childCount; i++)
                {
                    root.transform.GetChild(i).SetParent(resource.transform);
                }
                

                var removed = root.GetComponent<LODGroup>();
                CoreUtil.Destroy(removed);

                var temp = new SerializedObject(resource);
                temp.FindProperty("_renderers").ClearArray();
                for (int i = 0; i < lods[0].renderers.Length; i++)
                {
                    temp.FindProperty("_renderers").arraySize++;
                    temp.FindProperty("_renderers").GetArrayElementAtIndex(i).objectReferenceValue = lods[0].renderers[i];                    
                }
                temp.ApplyModifiedProperties();

                temp = new SerializedObject(tile);
                temp.FindProperty("_worldObjectResource").objectReferenceValue = resource;
                temp.FindProperty("_height").floatValue = -0.5f;
                temp.ApplyModifiedProperties();


                DirectoryInfo dir = new DirectoryInfo(path);
                if (false == dir.Exists)
                    dir.Create();


                PrefabUtility.SaveAsPrefabAssetAndConnect(root, $"{path}/{fileName}.prefab", InteractionMode.AutomatedAction);
                CoreUtil.Destroy(root);

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
        }
    }
}
