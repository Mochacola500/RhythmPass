using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public enum FadeTypeEnum : int
    {
        FadeOut,
        FadeIn,
    }
    public class FadeUI : ManagementUIBase
    {
        public const float DefaultFadeTime = 1f;
        [SerializeField] Image _fadeImage;
        FadeTypeEnum _fadeState;
        Coroutine _fadeCoroutine;
        public void FadeOut(float time, Action callbackEnd)
        {
            if(null != _fadeCoroutine)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            _fadeState = FadeTypeEnum.FadeOut;
            _fadeCoroutine = StartCoroutine(CoroutineFade(time,0f, 1f, callbackEnd));
        }
        public void FadeIn(float time, Action callbackEnd)
        {
            if (null != _fadeCoroutine)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            _fadeState = FadeTypeEnum.FadeIn;
            _fadeCoroutine = StartCoroutine(CoroutineFade(time,1f, 0f, callbackEnd));
        }
        public FadeTypeEnum GetFadeState()
        {
            return _fadeState;
        }
        IEnumerator CoroutineFade(float time, float fromAlpha,float toAlpha, Action callbackEnd)
        {
            float currentTime = 0.00001f;

            Color color = _fadeImage.color;
            while(currentTime < time)
            {
                float currentAlpha;
                if (fromAlpha < toAlpha)
                    currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, currentTime / time);
                else
                    currentAlpha = 1f - Mathf.Lerp(toAlpha, fromAlpha, currentTime / time);
                color.a = currentAlpha;
                _fadeImage.color = color;
                currentTime += Game.GameTime.GetDeltaTime();
                yield return null;
            }
            color.a = toAlpha;
            _fadeImage.color = color;
            _fadeCoroutine = null;
            callbackEnd?.Invoke();
        }
    }
}