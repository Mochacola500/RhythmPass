using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace RogueWave.YulKang.GraphicsConfig
{
    internal static class URPFunctionality
    {
        internal static bool SupportsDepthTexture(UniversalRenderPipelineAsset asset, bool value) => asset.supportsCameraDepthTexture = value;
        internal static bool SupportsOpaqueTexture(UniversalRenderPipelineAsset asset, bool value) => asset.supportsCameraOpaqueTexture = value;
        internal static bool SupportsHDR(UniversalRenderPipelineAsset asset, bool value) => asset.supportsHDR = value;
        internal static float RenderScale(UniversalRenderPipelineAsset asset, float value) => asset.renderScale = value;
        internal static int MaxAdditionalLightsCount(UniversalRenderPipelineAsset asset, int value) => asset.maxAdditionalLightsCount = value;
        internal static float ShadowDistance(UniversalRenderPipelineAsset asset, float value) => asset.shadowDistance = value;
        internal static int ShadowCascadeCount(UniversalRenderPipelineAsset asset, int value) => asset.shadowCascadeCount = value;
        internal static float ShadowDepthBias(UniversalRenderPipelineAsset asset, float value) => asset.shadowDepthBias = value;
        internal static float ShadowNormalBias(UniversalRenderPipelineAsset asset, float value) => asset.shadowNormalBias = value;
        internal static ColorGradingMode ColorGradingMode(UniversalRenderPipelineAsset asset, ColorGradingMode value) => asset.colorGradingMode = value;
        internal static int ColorGradingLutSize(UniversalRenderPipelineAsset asset, int value) => asset.colorGradingLutSize = value;



        private static readonly Type _assetType = typeof(UniversalRenderPipelineAsset);
        private const BindingFlags _flag = BindingFlags.Instance | BindingFlags.NonPublic;
        private static FieldInfo GetField(string name) => _assetType.GetField(name, _flag);

        private static readonly FieldInfo m_OpaqueDownsampling = GetField("m_OpaqueDownsampling");
        private static readonly FieldInfo m_SupportsTerrainHoles = GetField("m_SupportsTerrainHoles");
        private static readonly FieldInfo m_MSAA = GetField("m_MSAA");
        private static readonly FieldInfo m_MainLightRenderingMode = GetField("m_MainLightRenderingMode");
        private static readonly FieldInfo m_MainLightShadowsSupported = GetField("m_MainLightShadowsSupported");
        private static readonly FieldInfo m_MainLightShadowmapResolution = GetField("m_MainLightShadowmapResolution");

        private static readonly FieldInfo m_AdditionalLightsRenderingMode = GetField("m_AdditionalLightsRenderingMode");
        private static readonly FieldInfo m_AdditionalLightShadowsSupported = GetField("m_AdditionalLightShadowsSupported");
        private static readonly FieldInfo m_AdditionalLightsShadowmapResolution = GetField("m_AdditionalLightsShadowmapResolution");
        private static readonly FieldInfo m_Cascade2Split = GetField("m_Cascade2Split");
        private static readonly FieldInfo m_Cascade3Split = GetField("m_Cascade3Split");
        private static readonly FieldInfo m_Cascade4Split = GetField("m_Cascade4Split");
        private static readonly FieldInfo m_SoftShadowsSupported = GetField("m_SoftShadowsSupported");

        //private static readonly FieldInfo m_UseFastSRGBLinearConversion = GetField("m_UseFastSRGBLinearConversion");
        //private static readonly FieldInfo m_MixedLightingSupported = GetField("m_MixedLightingSupported");

        internal static T SetActiveRenderFeature<T>(UniversalRenderPipelineAsset asset, bool active) where T : ScriptableRendererFeature
        {            
            var fieldInfo = GetField("m_RendererDataList");
            if (null == fieldInfo)
            {
                Debug.LogError("fail get renderfeature");
                return null;
            }
            var rendererData = (ScriptableRendererData[])fieldInfo.GetValue(asset);

            foreach (var data in rendererData)
            {
                foreach (var rendererFeature in data.rendererFeatures)
                {
                    if (rendererFeature is T)
                    {
                        rendererFeature.SetActive(active);
                        return rendererFeature as T;
                    }
                }
            }

            return null;
        }


        internal static Downsampling OpaqueDownsampling(UniversalRenderPipelineAsset asset, Downsampling value, bool activeOpaqueTexture = true)
        {
            if (activeOpaqueTexture)
                asset.supportsCameraOpaqueTexture = true;

            m_OpaqueDownsampling.SetValue(asset, value);
            return asset.opaqueDownsampling;
        }

        internal static bool SupportsTerrainHoles(UniversalRenderPipelineAsset asset, bool value)
        {
            m_SupportsTerrainHoles.SetValue(asset, value);
            return asset.supportsTerrainHoles;
        }

        internal static MsaaQuality SetMsaaQuality(UniversalRenderPipelineAsset asset, MsaaQuality value)
        {
            m_MSAA.SetValue(asset, value);
            return (MsaaQuality)asset.msaaSampleCount;
        }

        internal static LightRenderingMode MainLightRenderIngMode(UniversalRenderPipelineAsset asset, LightRenderingMode value)
        {
            if(LightRenderingMode.PerVertex == value)
            {
                Debug.LogError("Main light rendering mode can not be PerVertex");
                return asset.mainLightRenderingMode;
            }

            m_MainLightRenderingMode.SetValue(asset, value);
            return asset.mainLightRenderingMode;
        }

        internal static bool SupportsMainLightShadows(UniversalRenderPipelineAsset asset, bool value)
        {
            m_MainLightShadowsSupported.SetValue(asset, value);
            return asset.supportsMainLightShadows;
        }

        internal static ShadowResolution MainLightShadowmapResolution(UniversalRenderPipelineAsset asset, ShadowResolution value)
        {
            m_MainLightShadowmapResolution.SetValue(asset, value);
            return (ShadowResolution)asset.mainLightShadowmapResolution;
        }

        internal static LightRenderingMode AdditionalLightsRenderingMode(UniversalRenderPipelineAsset asset, LightRenderingMode value)
        {
            m_AdditionalLightsRenderingMode.SetValue(asset, value);
            return asset.additionalLightsRenderingMode;
        }

        internal static bool SupportsAdditionalLightShadows(UniversalRenderPipelineAsset asset, bool value)
        {
            m_AdditionalLightShadowsSupported.SetValue(asset, value);
            return asset.supportsAdditionalLightShadows;
        }

        internal static ShadowResolution AdditionalLightsShadowmapResolution(UniversalRenderPipelineAsset asset, ShadowResolution value)
        {
            m_AdditionalLightsShadowmapResolution.SetValue(asset, value);
            return (ShadowResolution)asset.additionalLightsShadowmapResolution;
        }

        internal static float Cascade2Split(UniversalRenderPipelineAsset asset, float value)
        {
            m_Cascade2Split.SetValue(asset, value);
            return asset.cascade2Split;
        }

        internal static Vector2 Cascade3Split(UniversalRenderPipelineAsset asset, Vector2 value)
        {
            m_Cascade3Split.SetValue(asset, value);
            return asset.cascade3Split;
        }

        internal static Vector3 Cascade4Split(UniversalRenderPipelineAsset asset, Vector3 value)
        {
            m_Cascade4Split.SetValue(asset, value);
            return asset.cascade4Split;
        }

        internal static bool SupportsSoftShadows(UniversalRenderPipelineAsset asset, bool value)
        {
            m_SoftShadowsSupported.SetValue(asset, value);
            return asset.supportsSoftShadows;
        }

    }
}
