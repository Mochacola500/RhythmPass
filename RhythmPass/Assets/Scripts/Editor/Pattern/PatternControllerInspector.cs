using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dev.EditorCode
{
    class Styles
    {
        public static readonly GUIContent Remove;
        public static readonly Texture2D MenuIcon;
        public static readonly Texture2D ArrowRight;
        public static readonly Texture2D ArrowDown;
        public static Rect GenericMenuRect;
        public static GenericMenu GenericMenu;
        static Styles()
        {
            Remove = new GUIContent("Remove");
            MenuIcon = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
    
            GenericMenu = new GenericMenu();
            GenericMenuRect = new Rect();
        }
    }
    
    [CustomEditor(typeof(PatternController))]
    public class PatternControllerInspector : Editor
    {
        readonly List<Type> _patternTypeList = new List<Type>();
        string[] _patternTypeDisplay;
    
        PatternController _target;
        private void OnEnable()
        {
            _target = target as PatternController;
            PatternBase[] patterns = _target.GetComponents<PatternBase>();
            foreach(var pattern in patterns)
            {
                pattern.hideFlags = HideFlags.HideInInspector;
            }
            GetPatternTypeList();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
    
            GUILayout.Label("Patterns",EditorStyles.boldLabel);
            if (null != _target.Patterns)
            {
                PatternBase currentPattern = null;
                Styles.GenericMenu = new GenericMenu();
                for (int i = 0; i < _target.Patterns.Count; ++i)
                {
                    currentPattern = _target.Patterns[i];
                    bool isExtend = true;
                    int index = i;
                    DrawSplitter();
                    DrawPatternHeader(currentPattern.PatternName, (genericMenu) =>
                    {
                        genericMenu.AddItem(Styles.Remove, false, () =>
                        {
                            RemovePattern(index);
                            --i;
                        });
                    },Color.red ,ref isExtend);

                    EditorGUI.BeginChangeCheck();
                    currentPattern.OnInspectorGUI();
                    if(EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(currentPattern);
                    }
                    EditorGUILayout.Space();
                }
            }
    
            DrawSplitter();
    
            int selectIndex = EditorGUILayout.Popup(0, _patternTypeDisplay) - 1;
            if (selectIndex >= 0)
            {
                AddPattern(_patternTypeList[selectIndex]);
            }
            serializedObject.ApplyModifiedProperties();
        }

        void AddPattern(Type type)
        {
            _target.AddPattern(type);
            EditorUtility.SetDirty(_target);
        }
        void RemovePattern(int index)
        {
            _target.RemovePattern(index);
            EditorUtility.SetDirty(_target);
        }
        List<Type> GetPatternTypeList()
        {
            if(null == _patternTypeList || 0 == _patternTypeList.Count)
            {
                List<System.Type> types = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
                                               from assemblyType in domainAssembly.GetTypes()
                                               where assemblyType.IsSubclassOf(typeof(PatternBase))
                                               select assemblyType).ToList();
                _patternTypeList.Clear();
                List<string> typeNameList = new List<string>();
                typeNameList.Add("Add new Pattern...");
                int index = 0;
                foreach (var type in types)
                {
                    _patternTypeList.Add(type);
                    typeNameList.Add(type.Name);
                    ++index;
                }
                _patternTypeDisplay = typeNameList.ToArray();
            }
            return _patternTypeList;
        }
    
        void DrawSplitter()
        {
            Rect rect = GUILayoutUtility.GetRect(1f, 1f);
    
            rect.xMin = 0f;
            rect.width += 4f;
    
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
    
            EditorGUI.DrawRect(rect, Color.black);
        }
    
        void DrawPatternHeader(string name,Action<GenericMenu> initGenericMenu , Color color,ref bool isExpend)
        {
            Rect backGroundRect = GUILayoutUtility.GetRect(0, 17f);
            backGroundRect.xMin = 0f;
            Rect labelRect = backGroundRect;
            float offset = 4f;
            labelRect.xMin += 16f + offset;
            labelRect.xMax -= 20f;
    
            Rect colorRect = new Rect();
            colorRect.x = labelRect.xMin;
            colorRect.y = labelRect.yMin;
            colorRect.width = 5f;
            colorRect.height = 17f;
            colorRect.xMin = 0f;
            colorRect.xMax = 5f;
    
            Rect foldOutRect = new Rect();
            foldOutRect = backGroundRect;
            foldOutRect.y += 1f;
            foldOutRect.xMin += offset + 2;
            foldOutRect.width = 13f;
            foldOutRect.height = 13f;
    
            EditorGUI.DrawRect(backGroundRect, new Color(0.18f, 0.18f, 0.18f, 1f));
    
            EditorGUI.DrawRect(colorRect, color);
    
            isExpend = GUI.Toggle(foldOutRect, isExpend, GUIContent.none, EditorStyles.foldout);
    
            EditorGUI.LabelField(labelRect, name, EditorStyles.boldLabel);
    
            Rect menuRect = new Rect();
            menuRect.x = labelRect.xMax + 4f;
            menuRect.y = labelRect.y;
            menuRect.width = Styles.MenuIcon.width;
            menuRect.height = Styles.MenuIcon.height;
            
            GUI.DrawTexture(menuRect, Styles.MenuIcon);
    
            Event e = Event.current;
            if (e.type == EventType.MouseDown)
            {
                if (menuRect.Contains(e.mousePosition))
                {
                    initGenericMenu(Styles.GenericMenu);
    
                    Styles.GenericMenuRect.x = menuRect.x;
                    Styles.GenericMenuRect.y = menuRect.yMax;
                    Styles.GenericMenuRect.width = 0f;
                    Styles.GenericMenuRect.height = 0f;
                    Styles.GenericMenu.DropDown(Styles.GenericMenuRect);
                    e.Use();
                }
            }
        }
    }
}