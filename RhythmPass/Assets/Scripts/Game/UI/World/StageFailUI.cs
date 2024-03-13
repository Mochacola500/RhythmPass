using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class StageFailUI : ManagementUIBase
    {
        [SerializeField] CurrencyUI _currencyUI;
        [SerializeField] StateButton _retryButton;
        StageInfo _stageInfo;
        public void Init(StageInfo stageInfo)
        {
            _stageInfo = stageInfo;

            if (null != _currencyUI)
                _currencyUI.Init(Game.User.GetCurrency((int)CurrencyEnum.Ticket));
            if (null != _retryButton)
                _retryButton.SetState(Game.User.GetCurrencyAmount((int)CurrencyEnum.Ticket) != 0);
        }
        public void OnClickFreeRetry()
        {
            Game.Instance.LoadStage(_stageInfo.ID,StageEnterTypeEnum.Admob);
        }
        public void OnClickRetry()
        {
            Game.Instance.LoadStage(_stageInfo.ID, StageEnterTypeEnum.Currency);
        }
        public void OnClickGoToLobby()
        {
            Game.Instance.LoadLobby();
        }
    }
}