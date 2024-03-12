using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dev.UI
{
    public class SpeechBubbleUI : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] Text _text;
        [SerializeField] Button _button;
        Tween _scaleTween;
        System.Action _callbackClick;
#if UNITY_EDITOR
        private void OnValidate()
        {
            _rectTransform = transform as RectTransform;
            _text = GetComponentInChildren<Text>();
            _button = GetComponentInChildren<Button>();
        }
#endif
        public void Init(Vector3 point, string text,System.Action callbackClick = null)
        {
            if (null != _rectTransform)
                _rectTransform.position = point;
            if (null != _text)
                _text.text = text;

            _callbackClick = callbackClick;
        }
        public void OnClick()
        {
            _callbackClick?.Invoke();
        }
    }
}
