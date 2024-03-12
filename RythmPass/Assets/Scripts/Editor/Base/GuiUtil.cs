#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Dev.EditorCode
{
    public class GuiUtil
    {
        public static float DrawLine(float thickness = 1f, bool expandWidth = true)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(expandWidth), GUILayout.Height(thickness));
            return thickness;
        }

        public static bool Foldout(bool bDisplay, GUIContent title)
        {
            var style = new GUIStyle("ShurikenModuleTitle");

            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

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

        public static bool FoldoutSubMenu(bool bDisplay, GUIContent title)
        {
            var style = new GUIStyle("MapEditorModuleTitle");
            style.font = new GUIStyle(EditorStyles.boldLabel).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.padding = new RectOffset(5, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(32f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 16f, rect.y + 2f, 13f, 13f);
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


        public static bool Foldout(bool bDisplay, string title)
        {
            return Foldout(bDisplay, new GUIContent(title));
        }
        public static bool Foldout(bool bDisplay, Texture2D title)
        {
            return Foldout(bDisplay, new GUIContent(title));
        }

        public static bool FoldoutSubMenu(bool bDisplay, string title)
        {
            return FoldoutSubMenu(bDisplay, new GUIContent(title));
        }
        public static bool FoldoutSubMenu(bool bDisplay, Texture2D title)
        {
            return FoldoutSubMenu(bDisplay, new GUIContent(title));
        }

    }
}

#endif