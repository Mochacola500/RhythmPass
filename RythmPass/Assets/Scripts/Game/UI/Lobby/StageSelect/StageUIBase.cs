using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class StageUIBase : MonoBehaviour
    {
        [SerializeField] Text _stageNumberText;
        [SerializeField] Image _bgImage;
        [SerializeField] Image _navigationImage;
        [SerializeField] GameObject _scoreImageGroupObject;
        [SerializeField] Image[] _arrScoreImage;
        [Header("Resource")]
        [SerializeField] Sprite _clearBGSprite;
        [SerializeField] Sprite _lockBGSprite;
        [SerializeField] Sprite _firstEnterBGSprite;

        StageInfo _stageInfo;
        public void Init(StageInfo stageInfo)
        {
            _stageInfo = stageInfo;
            bool isNeedToClearStage = stageInfo.IsLock() == false && stageInfo.IsClear == false;
            if (null != _stageNumberText)
                _stageNumberText.text = (_stageInfo.Index + 1).ToString();
            if (null != _scoreImageGroupObject)
                _scoreImageGroupObject.SetActive(_stageInfo.IsClear);
            if(null != _arrScoreImage && 0 < _arrScoreImage.Length)
            {
                for(int i =0; i < _arrScoreImage.Length; ++i)
                {
                    if (i < _stageInfo.GetScorePoint())
                        _arrScoreImage[i].gameObject.SetActive(true);
                    else
                        _arrScoreImage[i].gameObject.SetActive(false);
                }
            }
            if (null != _navigationImage)
                _navigationImage.gameObject.SetActive(isNeedToClearStage && _stageInfo.IsClear == false);
            if(null != _bgImage)
            {
                if (_stageInfo.IsClear)
                    _bgImage.sprite = _clearBGSprite;
                else if (isNeedToClearStage)
                    _bgImage.sprite = _firstEnterBGSprite;
                else
                    _bgImage.sprite = _lockBGSprite;
            }
        }
    }
}