using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dev.UI
{
    using Data;
    public class ScoreUIBase : MonoBehaviour
    {
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] Image _scoreImage;
        [SerializeField] Text _scoreDescText;

        Tween _tween;
        public void Init(StageScore score)
        {
            if(null != _scoreImage)
                _scoreImage.color = score.IsSucceeded ? Color.white : new Color(0.25f, 0.25f, 0.25f, 1f);
            if (null != _scoreDescText)
                _scoreDescText.text = score.GetScoreText();
        }
        public void FadeIn()
        {
            if (null != _canvasGroup)
                _tween = _canvasGroup.DOFade(1f, 1f);
        }

        private void OnDestroy()
        {
            if (null != _tween)
                _tween.Kill();
        }
    }
}