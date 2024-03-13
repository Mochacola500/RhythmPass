using UnityEngine;
using UnityEngine.Rendering.Universal;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace RogueWave.YulKang.GraphicsConfig
{        
    public class URPAssetConfiguring
    {

        public URPAssetConfiguring(UniversalRenderPipelineAsset asset)
        {
            Target = asset;
        }

        public UniversalRenderPipelineAsset Target { get; internal set; }
        
        public void SetShadowSettings(bool show)
        {
            if(show)
            {
                MainLightShadowsCasting(true);
                AdditionalLightsShadowsCasting(true);
                MainLightShadowResolution(ShadowResolution._2048);
                AdditionalLightShadowResolution(ShadowResolution._512);
                MaxAdditionalLightsCount(4);
            }
            else
            {
                MainLightShadowsCasting(false);
                AdditionalLightsShadowsCasting(false);
                MaxAdditionalLightsCount(0);
            }

            //switch (@enum)
            //{
            //    case GraphicOption.ShadowQualityEnum.Disable:
            //    {
            //        MainLightShadowsCasting(false);
            //        AdditionalLightsShadowsCasting(false);
            //        MaxAdditionalLightsCount(0);
            //    }
            //    break;
            //    case GraphicOption.ShadowQualityEnum.Low:
            //    {
            //        MainLightShadowsCasting(true);
            //        AdditionalLightsShadowsCasting(false);
            //        MainLightShadowResolution(ShadowResolution._512);
            //        MaxAdditionalLightsCount(2);
            //    }
            //    break;
            //    case GraphicOption.ShadowQualityEnum.Middle:
            //    {
            //        MainLightShadowsCasting(true);
            //        AdditionalLightsShadowsCasting(false);
            //        MainLightShadowResolution(ShadowResolution._1024);
            //        MaxAdditionalLightsCount(4);
            //    }
            //    break;
            //    case GraphicOption.ShadowQualityEnum.High:
            //    {
            //        MainLightShadowsCasting(true);
            //        AdditionalLightsShadowsCasting(true);
            //        MainLightShadowResolution(ShadowResolution._2048);
            //        AdditionalLightShadowResolution(ShadowResolution._512);
            //        MaxAdditionalLightsCount(4);
            //    }
            //    break;
            //}
        }


        public bool DepthTexture(bool value) => URPFunctionality.SupportsDepthTexture(Target, value);
        public bool OpaqueTexture(bool value) => URPFunctionality.SupportsOpaqueTexture(Target, value);
        public Downsampling OpaqueDownsampling(Downsampling value) => URPFunctionality.OpaqueDownsampling(Target, value);
        public bool TerrainHoles(bool value) => URPFunctionality.SupportsTerrainHoles(Target, value);

        public bool HDR(bool value) => URPFunctionality.SupportsHDR(Target, value);
        public MsaaQuality SetMsaaQuality(MsaaQuality value) => URPFunctionality.SetMsaaQuality(Target, value);
        public float RenderScale(float value) => URPFunctionality.RenderScale(Target, value);

        public LightRenderingMode MainLightRenderingMode(LightRenderingMode value) => URPFunctionality.MainLightRenderIngMode(Target, value);
        public bool MainLightShadowsCasting(bool value) => URPFunctionality.SupportsMainLightShadows(Target, value);
        public ShadowResolution MainLightShadowResolution(ShadowResolution value) => URPFunctionality.MainLightShadowmapResolution(Target, value);
        public LightRenderingMode AdditionalLightsMode(LightRenderingMode value) => URPFunctionality.AdditionalLightsRenderingMode(Target, value);

        public int MaxAdditionalLightsCount(int value) => URPFunctionality.MaxAdditionalLightsCount(Target, value);
        public bool AdditionalLightsShadowsCasting(bool value) => URPFunctionality.SupportsAdditionalLightShadows(Target, value);
        public ShadowResolution AdditionalLightShadowResolution(ShadowResolution value) => URPFunctionality.AdditionalLightsShadowmapResolution(Target, value);

        public float ShadowDistance(float value) => URPFunctionality.ShadowDistance(Target, value);
        public int CascadeCount(int value) => URPFunctionality.ShadowCascadeCount(Target, value);
        public float Cascade2Split(float value) => URPFunctionality.Cascade2Split(Target, value);
        public Vector2 Cascade3Split(Vector2 value) => URPFunctionality.Cascade3Split(Target, value);
        public Vector3 Cascade4Split(Vector3 value) => URPFunctionality.Cascade4Split(Target, value);
        public float DepthBias(float value) => URPFunctionality.ShadowDepthBias(Target, value);
        public float NormalBias(float value) => URPFunctionality.ShadowNormalBias(Target, value);
        public bool SoftShadows(bool value) => URPFunctionality.SupportsSoftShadows(Target, value);

        public ColorGradingMode ColorGradingMode(ColorGradingMode value) => URPFunctionality.ColorGradingMode(Target, value);
        public int ColorGradingLutSize(int value) => URPFunctionality.ColorGradingLutSize(Target, value);

    }
}
