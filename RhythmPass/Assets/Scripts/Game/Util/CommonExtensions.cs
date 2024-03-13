using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public static class CommonExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string GetColorText(this string text, Color color)
        {
            string rgb = ColorUtility.ToHtmlStringRGB(color);
            string result = string.Format("<color=#{0}>{1}</color>", rgb, text);
            return result;
        }

        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }
            comp = go.AddComponent<T>();
            return comp;
        }
    }
}