using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dev.UI
{
    public class MainHUDUI : ManagementUIBase
    {
        [SerializeField] GameSlider _timeLimitSlider;
        [SerializeField] GameSlider _countLimitSlider;
        [SerializeField] Text _scoreCountLimitText;
        [SerializeField] Text _stageNumberText;
        public void Init()
        {
            Refresh();
        }
        public override void Refresh()
        {
            if (null != _stageNumberText)
                _stageNumberText.text = (Game.World.CurrentStage.StageInfo.Index + 1).ToString();

            RefreshTimeLimitUI();
            RefreshTryCountUI();
        }
        private void Update()
        {
            if (null != Game.World && null != Game.World.CurrentStage)
            {
                if (_timeLimitSlider.gameObject.activeSelf)
                {
                    if (Game.World.CurrentStage.IsPlaying)
                        RefreshTimeLimitUI();
                }
            }
        }
        public void OnClickOptionUI()
        {
            UIManager.LoadAsyncOptionUI();
        }
        void RefreshTimeLimitUI()
        {
            Stage stage = Game.World.CurrentStage;
            if (null != stage)
            {
                if (null != _timeLimitSlider)
                {
                    StageScore score = stage.StageInfo.GetScore(StageScoreTypeEnum.Time);
                    _timeLimitSlider.gameObject.SetActive(null != score && false == stage.IsTutorialStage);
                    if (null != score && false == stage.IsTutorialStage)
                    {
                        float stageTime = Mathf.Clamp(stage.StageTime,0f, score.Value);
                        _timeLimitSlider.InitTimeType(score.Value, stageTime);
                    }
                }
            }
        }
        void RefreshTryCountUI()
        {
            Stage stage = Game.World.CurrentStage;
            if (null != _countLimitSlider)
            {
                _countLimitSlider.gameObject.SetActive(stage.StageInfo.Record.MaxTryCount != 0 && false == stage.IsTutorialStage);
                if(stage.StageInfo.Record.MaxTryCount != 0 && false == stage.IsTutorialStage)
                {
                    _countLimitSlider.InitCountType(stage.StageInfo.Record.MaxTryCount, stage.TryCount);
                    StageScore score = stage.StageInfo.GetScore(StageScoreTypeEnum.TryCount);
                    _scoreCountLimitText.gameObject.SetActive(null != score && false == stage.IsTutorialStage);
                    if (null != score && false == stage.IsTutorialStage)
                        _scoreCountLimitText.text = score.Value.ToString();
                }
            }
        }
        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch (messageName)
            {
                case GameMessageEnum.StageEnd:
                    CloseUI();
                    break;
                case GameMessageEnum.UpdateTryCount:
                    RefreshTryCountUI();
                    break;
                  
            }
        }
    }
}