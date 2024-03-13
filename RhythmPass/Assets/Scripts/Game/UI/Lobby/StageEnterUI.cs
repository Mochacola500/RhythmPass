using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.UI
{
    public class StageEnterUI : ManagementUIBase
    {
        [SerializeField] StageUIBase _stageUIBase;
        [SerializeField] StateButton _enterButton;
        [SerializeField] StateButton _freeEnterButton;
        [SerializeField] ScoreUIBase[] _arrScoreUI;
        [SerializeField] CurrencyUI _currencyTicketUI;
        StageInfo _stageInfo;
        public void Init(StageInfo stageInfo)
        {
            _stageInfo = stageInfo;

            Refresh();
        }
        public override void Refresh()
        {
            if (null != _stageUIBase)
                _stageUIBase.Init(_stageInfo);
            if (null != _enterButton)
                _enterButton.SetState(Game.User.GetCurrencyAmount((int)CurrencyEnum.Ticket) > 0);
            if (null != _arrScoreUI && 0 < _arrScoreUI.Length)
            {
                for (int i = 0; i < _arrScoreUI.Length; ++i)
                {
                    _arrScoreUI[i].Init(_stageInfo.Scores[i]);
                }
            }
            if (null != _currencyTicketUI)
                _currencyTicketUI.Init(Game.User.GetCurrency((int)CurrencyEnum.Ticket));
        }
        public void OnClickEnter()
        {
            Game.Instance.LoadStage(_stageInfo.ID, StageEnterTypeEnum.Currency);
        }
        public void OnClickFreeEnter()
        {
            Game.Instance.LoadStage(_stageInfo.ID, StageEnterTypeEnum.Admob);
        }
        public void OnClickGoToLobby()
        {
            Game.Instance.LoadLobby();
        }
    }
}
