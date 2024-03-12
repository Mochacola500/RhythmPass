using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.Rendering;
using DG.Tweening.Core;
using DG.Tweening;

namespace Dev.Rendering
{

    [Serializable]
    public class ColorIntensitySetting
    {
        public float Intensity = 1f;
        public LightLayerEnum LightLayerEnum;   //TODO PreFix - 1 and else (not default)
    }
    public class DarknessRenderFeature : ScriptableRendererFeature
    {        
        [SerializeField] private ColorIntensitySetting _setting = new ColorIntensitySetting();
        private DarknessPass _darknessPass = null;
        private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> _tween;
        public void DoBrightness(float time = 0.3f)
        {
            KillDarkness();
            _tween = DOTween.To(() => _setting.Intensity, (x) => _setting.Intensity = x, 1f, time) ;
            _tween.SetOptions(false).SetTarget(this);

        }
        public void DoDarkness(float intensity = 0.3f, float time = 0.2f)
        {
            KillDarkness();
            _tween = DOTween.To(() => _setting.Intensity, (x) => _setting.Intensity = x, intensity, time);
            _tween.SetOptions(false).SetTarget(this);
        }

        public void KillDarkness()
        {
            if (_tween != null)
            {
                _tween.Kill();
            }

        }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _darknessPass.Setup(_setting);
            renderer.EnqueuePass(_darknessPass);
        }

        public override void Create()
        {
            if(null == _darknessPass)
                _darknessPass = new DarknessPass();
        }

        [System.Serializable]
        public class DarknessPass : ScriptableRenderPass
        {
            private static readonly int _CurrentIntensicy = Shader.PropertyToID("_CurrentIntensicy");
            private static readonly int _EffectLayerMask = Shader.PropertyToID("_EffectLayerMask");

            private ProfilingSampler _profilingSampler = new ProfilingSampler("Darkness");
            private ColorIntensitySetting _currnetSetting;
            public void Setup(ColorIntensitySetting settings)
            {
                _currnetSetting = settings;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get();
                using(new ProfilingScope(cmd, _profilingSampler))
                {
                    //단순히 LitInput.hlsl을 사용하는 Shader에 전역변수만 제어함
                    
                    Shader.SetGlobalFloat(_CurrentIntensicy, _currnetSetting.Intensity);
                    uint lightLayerMask = (uint)_currnetSetting.LightLayerEnum;
                    Shader.SetGlobalInt(_EffectLayerMask, (int)lightLayerMask);
                }                               
            }
        }

    }
}