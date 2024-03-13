using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Dev
{
    public static class ExtensionMethods
    {
        public static Rect ToRectSize(this Texture2D texture2D)
        {
            return new Rect(0, 0, texture2D.width, texture2D.height);
        }

        public static bool IsInIndex(this IList list, int index)
        {
            return 0 <= index && index < list.Count;
        }

        public static void SetParent(this GameObject go, GameObject parent, bool worldPositionStays = true)
        {
            go.transform.SetParent(parent.transform, worldPositionStays);
        }
    }
}