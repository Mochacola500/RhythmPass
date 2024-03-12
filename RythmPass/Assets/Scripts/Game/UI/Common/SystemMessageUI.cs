using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dev.UI
{
    public class SystemMessageUI : ManagementUIBase
    {
        [SerializeField] Text _middleText;
        Tween _middleTextTween;
        Coroutine _middleTextCoroutine;
        //todo 큐잉 해서 순차대로 처리하게 수정
        public void PlayMiddleText(string text, float time, Action callbackEnd)
        {
            if (null != _middleTextTween && _middleTextTween.active)
                return;
            if (null != _middleTextCoroutine)
                return;
            _middleText.gameObject.SetActive(true);
            _middleText.text = text;
            _middleTextTween = _middleText.transform.DOScale(1.5f, 0.08f).SetLoops(2, LoopType.Yoyo);
            _middleTextCoroutine = StartCoroutine(CoroutineWaitMiddleTextEnd(time, callbackEnd));
        }
        IEnumerator CoroutineWaitMiddleTextEnd(float time, Action callbackEnd)
        {
            yield return new WaitForSeconds(time);

            _middleText.text = string.Empty;
            _middleText.gameObject.SetActive(false);
            _middleTextCoroutine = null;
            callbackEnd?.Invoke();
        }
    }
}