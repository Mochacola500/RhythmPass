using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RogueWave.YulKang
{
    using GraphicsConfig;
    public static class GraphicsConfigurator
    {
        private static readonly URPAssetConfiguring _URPAssetConfiguring = new URPAssetConfiguring(UniversalRenderPipeline.asset);
        public static URPAssetConfiguring Handle
        {
            get
            {
                _URPAssetConfiguring.Target = UniversalRenderPipeline.asset;
                return _URPAssetConfiguring;
            }
        }
        //private static readonly float TargetResolutionRatio = 2f;
        //private static readonly float StandardFov = 3.940851f;
        public static void AdaptiveFov(bool bClean = false)
        {
            //if (null == Game.MainCamera)
            //    return;

            //if(bClean)
            //{
            //    Game.MainCamera.fieldOfView = StandardFov;
            //}

            //var deviceH = (float)Screen.height;
            //var deviceW = (float)Screen.width;
            //var deviceRatio = deviceW / deviceH;
            //if (deviceRatio > TargetResolutionRatio) 
            //{
            //    var factor = TargetResolutionRatio / deviceRatio;
            //    var targetFov = StandardFov * factor;
            //    Game.MainCamera.fieldOfView = targetFov;

            //    Debug.Log($"screen height: {deviceH}, screen width: {deviceW}, target fov: {targetFov}");
            //}                        
        }


        //public static void SetYKAnitAliasing(UniversalAdditionalCameraData cameraData, GraphicOption option)
        //{
        //    if (null == cameraData || null == option) return;

        //    switch (option.AnitAliasing)
        //    {
        //        case GraphicOption.AnitAliasingType.NoAA:
        //        {
        //            GraphicsConfigurator.Handle.SetMsaaQuality(MsaaQuality.Disabled);
        //            cameraData.antialiasing = AntialiasingMode.None;
        //        }
        //        break;
        //        case GraphicOption.AnitAliasingType.FXAA:
        //        {
        //            GraphicsConfigurator.Handle.SetMsaaQuality(MsaaQuality.Disabled);
        //            cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        //        }
        //        break;
        //        case GraphicOption.AnitAliasingType.MSAAx4:
        //        {
        //            GraphicsConfigurator.Handle.SetMsaaQuality(MsaaQuality._4x);
        //            cameraData.antialiasing = AntialiasingMode.None;
        //        }
        //        break;
        //    }
        //}

        public static void SetPostPorocessing(string volumeProfilePath = "", UniversalAdditionalCameraData cameraData = null, UnityEngine.Rendering.Volume volume = null)
        {

            //if (null == cameraData || null == volume || string.IsNullOrWhiteSpace(volumeProfilePath))
            //{
            //    cameraData.renderPostProcessing = false;
            //    return;
            //}

            //cameraData.renderPostProcessing = true;
            //AssetManager.LoadAssetAsync<UnityEngine.Rendering.VolumeProfile>(volumeProfilePath, (profile) =>
            //{
            //    volume.profile = profile;
            //    cameraData.renderPostProcessing = null != profile;                
            //});
        }

        //defualt 설정값을 자동으로 조정하기 위한것 50미만는 중~하옵으로 조정할 예정
        public static int GetShaderLevel()
        {            
            return SystemInfo.graphicsShaderLevel;
        }

        public enum SettingScore : int
        {
            Low = 0,
            Middle = 1,
            Hight = 2,
        }

        private static readonly float Benchmark_ProcessorFrequency = 2730f;
        private static readonly float Benchmark_ProcessorCount = 8f;
        private static readonly float Benchmark_GraphicsMemorySize = 7052f;
        private static readonly float Benchmark_MaxTextureSize = 8192f;
        private static readonly float Benchmark_SystemMemorySize = 7402f;

        public static SettingScore GetSettingScore()
        {
            var rate = GetSettingScoreRate();
            var score = Mathf.FloorToInt(rate + 0.5f);

            return (SettingScore)score;
        }

        public static float GetSettingScoreRate()
        {
            var scoreRate = SystemInfo.processorFrequency / Benchmark_ProcessorFrequency;
            scoreRate *= SystemInfo.processorCount / Benchmark_ProcessorCount;
            scoreRate *= SystemInfo.graphicsMemorySize / Benchmark_GraphicsMemorySize;
            scoreRate *= SystemInfo.maxTextureSize / Benchmark_MaxTextureSize;
            scoreRate *= SystemInfo.systemMemorySize / Benchmark_SystemMemorySize;
            return scoreRate;
        }
    }
}