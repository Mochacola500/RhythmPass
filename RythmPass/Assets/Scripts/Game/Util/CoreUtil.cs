using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace Dev
{
    public class CoreUtil
    {
#if UNITY_EDITOR
        public static T CreateOrUpdate<T>(T target, string path, bool callSaveAssets = true) where T : UnityObject
        {
            if(typeof(AnimatorController) == typeof(T))
            {
                throw new System.Exception("AnimatorController는 StateMachine을 내부에 기록하지만 이 Method로는 duplicate가 불가능하기 때문에 AssetDatabase.CopyAsset을 통해서 수동으로 수정해야함");                
            }

            T result = null;
            var oldAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            if(null == oldAsset)
            {
                AssetDatabase.CreateAsset(target, path);
                result = target;
                //AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry()
                //AddressableAssetEntry
            }
            else
            {
                EditorUtility.CopySerializedIfDifferent(target, oldAsset);
                
                if(callSaveAssets)
                    AssetDatabase.SaveAssets();

                result = oldAsset;
            }

            return result;
        }
#endif

        public static void Destroy(UnityObject obj)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    UnityObject.Destroy(obj);
                else
                    UnityObject.DestroyImmediate(obj,true);
#else
                UnityObject.Destroy(obj);
#endif
            }
        }
    }
}