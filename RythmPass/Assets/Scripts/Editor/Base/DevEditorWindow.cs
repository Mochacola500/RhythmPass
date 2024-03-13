using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Dev.EditorCode
{
    //#if UNITY_EDITOR    
    public class DevEditorWindow : EditorWindow
    {        
        static public void Label(string comment, bool bold = false)
        {
            if (bold)
            {
                EditorGUILayout.LabelField(comment, EditorStyles.miniBoldLabel);
                return;
            }
            EditorGUILayout.LabelField(comment);
        }

        public static void HorizonLine(int strok = 1)
        {
            new HLineGUI(strok, Color.grey);
        }

        public static void VerticalLine(int strok = 1)
        {
            new VLineGUI(strok, Color.grey);
        }

        static public HorizontalStyleScope HorizontalScope(GUIStyle style = null, params GUILayoutOption[] options)
        {
            return new HorizontalStyleScope(style, options);
        }

        static public VerticalStyleScope VerticalScope(GUIStyle style = null, params GUILayoutOption[] options)
        {
            return new VerticalStyleScope(style, options);
        }

        static public ColorGUI ColorScope(Color color)
        {
            return new ColorGUI(color);
        }

        static public GroupGUI GroupScope(string title)
        {
            return new GroupGUI(title);
        }

        static public void Separator()
        {
            EditorGUILayout.Separator();
        }
        static public void Space()
        {
            EditorGUILayout.Space();
        }        
        static public void Space(float width)
        {
            EditorGUILayout.Space(width);
        }

        static public bool ImageButton(Texture tex, string emptyMessage, int width)
        {
            bool result = GUILayout.Button(GUIContent.none, GUILayout.Width(width), GUILayout.Height(width));
            Rect rect = GUILayoutUtility.GetLastRect();
            if (tex != null)
                GUI.DrawTexture(rect, tex);
            if (tex == null)
            {
                var labelGUI = new GUIStyle();
                labelGUI.alignment = TextAnchor.MiddleCenter;
                labelGUI.wordWrap = true;
                GUI.Label(rect, emptyMessage, labelGUI);
            }
            return result;
        }

        static public void Image(Texture tex, int width)
        {
            var current = GUI.enabled;
            GUI.enabled = false;
            GUILayout.Button(GUIContent.none, GUILayout.Width(width), GUILayout.Height(width));
            Rect rect = GUILayoutUtility.GetLastRect();
            if (tex != null)
                GUI.DrawTexture(rect, tex);
            GUI.enabled = current;
        }

        static public void ShowObjectList<T>(T @object) where T : UnityEngine.Object
        {
            EditorGUIUtility.ShowObjectPicker<T>(@object, false, "", 0);
        }

        static public void ShowObjectList<T>(T @object, int index) where T : UnityEngine.Object
        {
            EditorGUIUtility.ShowObjectPicker<T>(@object, false, "", index);
        }

        static public T ObjectListUpdate<T>(T @object) where T : UnityEngine.Object
        {
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                return EditorGUIUtility.GetObjectPickerObject() as T;
            }
            return @object;
        }

        static public bool ObjectListUpdate<T>(List<T> items) where T : UnityEngine.Object
        {
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                int index = EditorGUIUtility.GetObjectPickerControlID();
                items[index] = EditorGUIUtility.GetObjectPickerObject() as T;
                return true;
            }
            return false;
        }

        static public string ListField(string[] list, string current)
        {
            int selected = -1;
            for (int i = 0; i < list.Length; ++i)
            {
                if (list[i] == current)
                    break;
            }

            if (selected == -1)
                return current;

            int next = EditorGUILayout.Popup(selected, list);
            if (next != selected)
            {
                return list[next];
            }
            else
            {
                return current;
            }
        }
        public static bool Foldout(bool bDisplay, string title)
        {
            return Foldout(bDisplay, new GUIContent(title));
        }
        public static bool Foldout(bool bDisplay, Texture2D title)
        {
            return Foldout(bDisplay, new GUIContent(title));
        }
        public static bool Foldout(bool bDisplay, GUIContent title)
        {
            //var style = new GUIStyle("ShurikenModuleTitle");
            var style = AllocCommonGUIStyle();

            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);

            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, bDisplay, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                bDisplay = !bDisplay;
                e.Use();
            }
            return bDisplay;
        }
        static public MenuGUI Menu()
        {
            return new MenuGUI(null);
        }

        public struct ColorGUI : IDisposable
        {
            Color mColor;

            public ColorGUI(Color color)
            {
                mColor = UnityEngine.GUI.color;
                UnityEngine.GUI.color = color;
            }

            public void Dispose()
            {
                UnityEngine.GUI.color = mColor;
            }
        }

        public struct VerticalStyleScope : IDisposable
        {
            public VerticalStyleScope(GUIStyle style, params GUILayoutOption[] options)
            {                
                EditorGUILayout.BeginVertical(style ?? GUIStyle.none, options);
            }

            public void Dispose()
            {
                EditorGUILayout.EndVertical();
            }
        }

        public struct HorizontalStyleScope : IDisposable
        {
            public HorizontalStyleScope(GUIStyle style, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(style ?? GUIStyle.none, options);
            }

            public void Dispose()
            {
                GUILayout.EndHorizontal();
            }
        }

        public struct VLineGUI
        {
            static GUIStyle mStyle = AllocSliceGUIStyle();

            public VLineGUI(int strok, Color color)
            {
                AllockBackground(mStyle, _sliceBackgroundColor);
                using (ColorScope(color))
                {
                    float prev_strok = mStyle.fixedWidth;
                    mStyle.fixedWidth = strok;
                    GUILayout.Box(GUIContent.none, mStyle);
                    mStyle.fixedWidth = prev_strok;
                }
            }
        }

        public struct HLineGUI
        {
            static GUIStyle mStyle = AllocSliceGUIStyle();

            public HLineGUI(int strok, Color color)
            {
                AllockBackground(mStyle, _sliceBackgroundColor);
                using (ColorScope(color))
                {
                    float prev_strok = mStyle.fixedHeight;
                    mStyle.fixedHeight = strok;
                    GUILayout.Box(GUIContent.none, mStyle);
                    mStyle.fixedHeight = prev_strok;
                }
            }
        }

        public struct GroupGUI : IDisposable
        {
            static GUIStyle mGroup = AllocCommonGUIStyle();
            static GUIStyle mHeader = AllocFitGUIStyle();

            VerticalStyleScope vertical;
            public GroupGUI(string title)
            {

                AllockBackground(mGroup, _commonBackgroundColor);
                AllockBackground(mHeader, _fitBackgroundColor);                

                vertical = VerticalScope(mGroup);                
                {                    
                    using (HorizontalScope(mHeader))
                    {
                        Label(title, bold: true);
                    }
                }
            }

            public void Dispose()
            {
                HorizonLine(1);
                vertical.Dispose();
            }
        }

        public struct MenuGUI : IDisposable
        {
            GenericMenu mMenu;

            public MenuGUI(object _)
            {
                mMenu = new GenericMenu();
            }

            public void Add(string title, GenericMenu.MenuFunction action)
            {
                mMenu.AddItem(new GUIContent(title), false, action);
            }

            public void Dispose()
            {
                mMenu.ShowAsContext();
                mMenu = null;
            }
        }

        private static Color _commonBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        private static Color _fitBackgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        private static Color _sliceBackgroundColor = Color.grey;
        static GUIStyle AllocCommonGUIStyle()
        {
            var style = new GUIStyle();
            style.border = new RectOffset(4, 4, 4, 4);
            style.margin = new RectOffset(8, 8, 8, 8);
            AllockBackground(style, _commonBackgroundColor);
            return style;
        }
         
        static GUIStyle AllocFitGUIStyle()
        { 
            var style = new GUIStyle();
            style.border = new RectOffset(0, 0, 0, 0);
            style.margin = new RectOffset(0, 0, 0, 0);
            //SetBackground(style, new Color(0.3f, 0.3f, 0.3f));
            AllockBackground(style, new Color(0.4f, 0.4f, 0.4f, 1f));
            return style;

        }

        static GUIStyle AllocSliceGUIStyle()
        {
            var style = new GUIStyle();
            style.margin = new RectOffset(2, 2, 2, 2);
            AllockBackground(style, Color.grey);
            return style;
        }

        protected static void AllockBackground(GUIStyle style, Color color)
        {
            var texture = style?.normal?.background;
            if (texture == null)
            {
                texture = new Texture2D(1, 1);
                style.normal.background = texture;
                texture.SetPixel(0, 0, color);
                texture.Apply();
            }

        }

        protected static void SetBackgroundColor(GUIStyle style, Color color)
        {
            var tex = style?.normal?.background;
            if(null != tex)
            {
                tex.SetPixel(0, 0, color);
                tex.Apply();
            }
        }
    }


    //#endif
}
