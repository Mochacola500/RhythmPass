using System.Collections;
using UnityEngine;

namespace Dev
{
    public static class TextureExtensions
    {
        public static Texture2D ToTexture2D(this Texture texture, TextureFormat format)
        {
            var tex = Texture2D.CreateExternalTexture(texture.width, texture.height, format, false, false, texture.GetNativeTexturePtr());
            tex.Apply();
            return tex;
        }
        public static Texture2D CopyTexture(this Texture2D src, bool bLinear = false)
        {
            Texture2D output = new Texture2D(src.width, src.height, src.format, src.mipmapCount, bLinear);

            if (SystemInfo.copyTextureSupport == UnityEngine.Rendering.CopyTextureSupport.None)
            {
                Color[] buffer = src.GetPixels(0, 0, src.width, src.height);
                output.SetPixels(buffer);
                output.Apply();
            }
            else
            {
                Graphics.CopyTexture(src, output);
            }
            return output;
        }
    }
}