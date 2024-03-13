#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dev.EditorCode
{
    public class EditorGUIListPage<T>
    {
        private class ID
        {
            //public int Id;
            public string Name;
        }

        private IList<T> _dataList;
        private Dictionary<ID, T> _dataDic = new Dictionary<ID, T>();        
        private List<T> _visiableList;
        private bool _needFlush = true;
        
        public event Action<List<T>> onGUI;
        public int DataCount { get { return _dataDic.Count; } }

        private string _searchString;
        private int _pageSize = 20;
        public int PageSize { get { return _pageSize; } }
        private int _currentPage = 0;
        public int MaxPageSize { get; set; } = 100;
        public int CurrentPage { get { return _currentPage + 1; } }
        private int PageCount { get; set; }
        public int CurrentPageIndex { get { return _currentPage; } }
        private bool _searchable = true;

        public int StartListIndex { get { return CurrentPageIndex * PageSize; } }
        private readonly string _searchTargetFieldName;

        //현재 인덱스를 알아오는것
        public int StartIndex { get { return CurrentPageIndex * PageSize; } }

        public EditorGUIListPage(IList<T> list, string searchTargetFieldName = "", bool indexable = true, bool searchable = true)
        {
            _searchable = searchable;

            _dataList = list;
            _searchString = "";
            PageCount = int.MaxValue;

            _searchTargetFieldName = string.IsNullOrWhiteSpace(searchTargetFieldName) ? "Name" : searchTargetFieldName;
            Flush(list);

        }

        public void Flush(IList<T> list)
        {
            _dataList = list;
            _searchString = "";
            _needFlush = true;
            _dataDic.Clear();

            if (_searchable)
            {
                var type = typeof(T);
                var fields = type.GetFields();
                System.Reflection.FieldInfo targetField = null;

                foreach (var field in fields)
                {
                    if (field.Name.Contains(_searchTargetFieldName) && null == targetField)
                    {
                        targetField = field;
                        break;
                    }
                }
                if (targetField == null)
                {
                    foreach (var field in fields)
                    {
                        if (field.Name.Contains("Comment") && null == targetField)
                        {
                            Debug.LogError("Class Field에 Name이 없어서 Comment로 대체함");
                            targetField = field;
                            break;
                        }
                    }
                }

                if (targetField != null)
                {
                    foreach (var item in _dataList)
                    {
                        object value = targetField.GetValue(item);
                        //if (null == value)
                        //    continue;

                        _dataDic.Add(new ID() { Name = value.ToString() }, item);
                    }
                    GC.Collect();
                }
                else
                {
                    Debug.LogError("데이터 클래스에 적당한 Field를 못 찾음, Name이나 Comment Field 필요");
                }

            }
            else
            {
                _visiableList = _dataList.ToList();
            }

        }

        private void PageControlGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
            {
                _currentPage--;
                _currentPage = Mathf.Clamp(_currentPage, 0, int.MaxValue);
                _needFlush = true;
            }
            if (GUILayout.Button(">"))
            {
                _currentPage++;
                _currentPage = Mathf.Clamp(_currentPage, 0, int.MaxValue);
                _needFlush = true;
            }
            EditorGUI.BeginChangeCheck();
            _currentPage = EditorGUILayout.DelayedIntField(CurrentPage) - 1;
            if (EditorGUI.EndChangeCheck())
            {
                _currentPage = Mathf.Clamp(_currentPage, 0, int.MaxValue);
                _needFlush = true;
            }
            EditorGUI.BeginChangeCheck();
            _pageSize = EditorGUILayout.IntSlider(_pageSize, 20, MaxPageSize);
            if (EditorGUI.EndChangeCheck())
            {
                _needFlush = true;
            }
            EditorGUILayout.EndHorizontal();

            if (_searchable)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("검색", GUILayout.Width(50f));
                    EditorGUI.BeginChangeCheck();
                    _searchString = EditorGUILayout.DelayedTextField(_searchString);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _needFlush = true;
                    }
                    if (GUILayout.Button("초기화", GUILayout.Width(50f)))
                    {
                        _searchString = string.Empty;
                        _needFlush = true;
                    }
                }
            }


            if (false != _needFlush)
            {

                if (_searchable)
                {
                    _visiableList?.Clear();
                    _visiableList = null;
                    _visiableList = (from pair in _dataDic
                                     where pair.Key.Name.ToLower().Contains(_searchString.ToLower())
                                     select pair.Value).ToList();
                    GC.Collect();
                }

                PageCount = (int)(_visiableList.Count / PageSize);
                _currentPage = Mathf.Clamp(_currentPage, 0, PageCount);
                _needFlush = false;
            }
        }

        public void OnGUI()
        {
            PageControlGUI();
            GuiUtil.DrawLine();
            onGUI?.Invoke(_visiableList);
        }


        public void Foreach(Action<T> action)
        {
            if (null == action) return;

            foreach(var item in _dataDic.Values)
            {
                action.Invoke(item);
            }
        }


    }


} 
#endif