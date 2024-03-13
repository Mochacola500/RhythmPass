using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Dev
{
    [CustomPropertyDrawer(typeof(FitPositionAttribute))]
    public class FitPositionPropertyDrawer : PropertyDrawer
    {
        private static readonly float ButtonHeight = 30f;
        private static readonly float Padding = 3f;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + ButtonHeight + Padding;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);

            position.y += Padding + EditorGUIUtility.singleLineHeight;
            position.width *= 0.7f;
            position.height = ButtonHeight;
            if (GUI.Button(position, "Fit"))
            {
                var target = property.serializedObject?.targetObject as WorldObject;
                target?.FitPosition();
            }

        }
    }

}

#endif