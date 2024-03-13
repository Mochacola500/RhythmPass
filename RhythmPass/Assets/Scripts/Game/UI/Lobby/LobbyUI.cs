using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Dev.UI
{
    public class LobbyUI : ManagementUIBase
    {
        [SerializeField] CurrencyUIGroup[] _arrCurrencyUI;
        [SerializeField] StageGroupUI _stageGroupUI;
        [SerializeField] StateButton _leftStageButton;
        [SerializeField] StateButton _rightStageButton;
        int _stageGroupIndex;
        public void Init()
        {
            Refresh();
        }
        public void OnClickLeftStageButton()
        {
            SetStageGroupIndex(_stageGroupIndex - 1);
        }
        public void OnClickRightStageButton()
        {
            SetStageGroupIndex(_stageGroupIndex + 1);
        }
        public void OnClickOptionButton()
        {
            UIManager.LoadAsyncOptionUI();
        }
        public override void Refresh()
        {
            RefreshCurrencyUI();
            RefreshStageGroupUI();
        }
        void SetStageGroupIndex(int index)
        {
            if (0 > index || Game.StageManager.StageGroupList.Count <= index)
                return;
            _stageGroupIndex = index;
            RefreshStageGroupUI();
        }
        void RefreshStageGroupUI()
        {
            if (0 > _stageGroupIndex || Game.StageManager.StageGroupList.Count <= _stageGroupIndex)
                return;
            StageGroupInfo stageGroupInfo = Game.StageManager.StageGroupList[_stageGroupIndex];
            if (null == stageGroupInfo)
                return;
            _stageGroupUI.Init(stageGroupInfo);

            if(null != _leftStageButton)
            {
                _leftStageButton.gameObject.SetActive(_stageGroupIndex > 0);
                if (null != _leftStageButton.Text)
                    _leftStageButton.Text.text = _stageGroupIndex.ToString();
            }
            if(null != _rightStageButton)
            {
                _rightStageButton.gameObject.SetActive(_stageGroupIndex < Game.StageManager.StageGroupList.Count - 1);
                if (null != _rightStageButton.Text)
                    _rightStageButton.Text.text = (_stageGroupIndex + 2).ToString();
            }
        }
        void RefreshCurrencyUI(int id = 0)
        {
            foreach(var group in _arrCurrencyUI)
            {
                if (0 == id || (int)group.CurrencyEnum == id)
                    group.CurrencyUI.Init(Game.User.GetCurrency((int)group.CurrencyEnum));
            }
        }
        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch(messageName)
            {
                case GameMessageEnum.ChangedUserCurrency:
                    GameMessage.ChangedUserCurrency data = message.Cast<GameMessage.ChangedUserCurrency>();
                    if(null != data.Currency)
                        RefreshCurrencyUI(data.Currency.ID);
                    break;
            }
        }
    }
}