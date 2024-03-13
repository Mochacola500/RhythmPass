using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
namespace Dev.UI
{
    public class StageSelectScrollItem : EnhancedScrollerCellView
    {
        [SerializeField] RectTransform _rectTransform;
        [SerializeField] StageUIBase _stageUI;
        [SerializeField] RewardUIBase _rewardUI;
        [SerializeField] Image _upLineImage;
        [SerializeField] Image _rightLineImage;
        [SerializeField] Image _downLineImage;

        StageInfo _stageInfo;
#if UNITY_EDITOR
        private void OnValidate()
        {
            _rectTransform = transform as RectTransform;
        }
#endif
        public float GetHeight()
        {
            return _rectTransform.sizeDelta.y;
        }
        public void Init(StageInfo stageInfo)
        {
            _stageInfo = stageInfo;
            if (null != _stageUI)
                _stageUI.Init(stageInfo);
            if(null != _rewardUI)
            {
                _rewardUI.gameObject.SetActive(stageInfo.Record.RewardID != 0);
                IRewardInfo rewardInfo = stageInfo.GetRewardInfo();
                if (null != rewardInfo)
                {
                    _rewardUI.Init(rewardInfo);
                }
            }
            if (null != _upLineImage)
                _upLineImage.gameObject.SetActive(stageInfo.Index != 0);
            if (null != _rightLineImage)
                _rightLineImage.gameObject.SetActive(stageInfo.Record.RewardID != 0);
            if(null != _downLineImage)
                _downLineImage.gameObject.SetActive(stageInfo.Index < stageInfo.StageGroupInfo.StageInfoList.Count - 1);
        }
        public void OnClickStage()
        {
            if(Game.Instance.CheckCanStageEnter(_stageInfo.ID))
                UIManager.LoadAsyncStageEnterPopup(_stageInfo);
        }
    }
}