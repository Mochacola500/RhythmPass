using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using System.Linq;
namespace Dev.UI
{
    using Data;
    public class StageSelectUI : ManagementUIBase, IEnhancedScrollerDelegate
    {
        [SerializeField] CurrencyUIGroup[] _arrCurrencyUI;
        [SerializeField] EnhancedScroller _scroller;
        [SerializeField] StageSelectScrollItem _prefab;
        StageGroupInfo _stageGroupInfo;
        public void Init(int stageGroupID)
        {
            _scroller.Delegate = this;
            _stageGroupInfo = Game.StageManager.GetStageGroupInfo(stageGroupID);
            foreach(var ui in _arrCurrencyUI)
            {
                ui.CurrencyUI.Init(Game.User.GetCurrency((int)ui.CurrencyEnum));
            }

            StartCoroutine(CoroutineWaitInit());
        }

        IEnumerator CoroutineWaitInit()
        {
            yield return null;

            int targetDataIndex = _stageGroupInfo.StageInfoList.Count - 1;
            foreach (var stageInfo in _stageGroupInfo.StageInfoList)
            {
                if (stageInfo.IsLock() == false && stageInfo.IsClear == false)
                {
                    targetDataIndex = stageInfo.Index;
                    break;
                }
            }

            if (targetDataIndex > -1)
            {
                _scroller.JumpToDataIndex(targetDataIndex,  10f ,0f,true,EnhancedScroller.TweenType.easeInOutQuart,0.5f);
            }
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            StageSelectScrollItem item = scroller.GetCellView(_prefab) as StageSelectScrollItem;
            item.Init(_stageGroupInfo.StageInfoList[dataIndex]);
            return item;
        }
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return _prefab.GetHeight();
        }
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return _stageGroupInfo.StageInfoList.Count;
        }
    }
}
