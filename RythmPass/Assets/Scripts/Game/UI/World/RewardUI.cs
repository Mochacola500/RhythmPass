using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class RewardUI : ManagementUIBase
    {
        [SerializeField] SlotBase _slotPrefab;
        [SerializeField] RectTransform _slotParent;

        GameObjectPool<SlotBase> _slotObjectPool;
        Action _callbackClose;
        public void Init(IRewardInfo rewardInfo,Action callbackClose)
        {
            if (null == rewardInfo)
                return;
            _callbackClose = callbackClose;
            GetSlotObjectPool().SleepAll();
            GetSlotObjectPool().Get().Init(rewardInfo); 
        }
        public override void OnCloseUI()
        {
            base.OnCloseUI();

            _callbackClose?.Invoke();
        }
        GameObjectPool<SlotBase> GetSlotObjectPool()
        {
            if (null == _slotObjectPool)
                _slotObjectPool = new GameObjectPool<SlotBase>(_slotPrefab.gameObject, _slotParent);
            return _slotObjectPool;
        }
    }
}