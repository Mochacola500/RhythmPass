using UnityEngine;
using Light = UnityEngine.Light;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Dev
{
    [RequireComponent(typeof(Light))]
    public class WorldLight : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private Light _darknessLight;
        public Light Light { get { return _light; } }
        public Light DarknessLight { get { return _darknessLight; } }
        public void OnValidate()
        {
            _light = GetComponent<Light>();
            var count = transform.childCount;
            if(0 >= count)
            {
                _darknessLight = null;
            }
            else
            {
                _darknessLight = transform.GetChild(0)?.GetComponent<Light>();
            }
        }
    }

#if UNITY_EDITOR
    
    [CustomEditor(typeof(WorldLight))]
    public class WorldLightEditor : Editor
    {
        private SerializedProperty _light;
        private SerializedProperty _darknessLight;
        private void OnEnable()
        {
            _light = serializedObject.FindProperty("_light");
            _darknessLight = serializedObject.FindProperty("_darknessLight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_light);
            EditorGUILayout.PropertyField(_darknessLight);

            if(null != _light && null != _darknessLight && _light != _darknessLight)
            {
                var editor = Editor.CreateEditor(_light.objectReferenceValue);
                if(null != editor)
                {
                    EditorGUI.BeginChangeCheck();
                    editor.OnInspectorGUI();
                    if(EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.CopySerialized(_light.objectReferenceValue, _darknessLight.objectReferenceValue);
                        Debug.Log("ldfafjasfjkafkla");
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

}