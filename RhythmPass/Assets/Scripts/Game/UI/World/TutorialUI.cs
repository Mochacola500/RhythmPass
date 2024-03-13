using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dev.UI
{
    public class TutorialUI : ManagementUIBase
    {
        [SerializeField] SpeechBubbleUI _speechBubbleUI;
        [SerializeField] RectTransform _pointerUI;
        [SerializeField] Image _background;
        Tween _pointerTween;
        TutorialStage _tutorialStage;
        public void Init()
        {
            _tutorialStage = Game.World.CurrentStage as TutorialStage;
        }
        public void DoNextTutorial()
        {
            if (null != _tutorialStage)
                _tutorialStage.ExecuteNextTutorial();
        }
        private void Update()
        {
            if (_tutorialStage.IsTutorialEnd)
                return;
            if (_tutorialStage.CurrentTutorialStep != (int)TutorialStepEnum.IntroduceGimmick)
                return;
            Vector3 gimmickPoint = _tutorialStage.GameField.WorldCamera.Camera.WorldToScreenPoint(_tutorialStage.GameField.WorldCharacters[0].transform.position);
            _speechBubbleUI.Init(gimmickPoint, "This is obstacle", DoNextTutorial);
        }
        void ExecuteCurrentTutorial()
        {
            Camera worldCamera = _tutorialStage.GameField.WorldCamera.Camera;

            switch((TutorialStepEnum)_tutorialStage.CurrentTutorialStep)
            {
                case TutorialStepEnum.IntorducePlayer:
                    {
                        Vector3 playerPoint = worldCamera.WorldToScreenPoint(_tutorialStage.GameField.GetPlayer().transform.position);
                        _speechBubbleUI.gameObject.SetActive(true);
                        _speechBubbleUI.Init(playerPoint, "This is your character", DoNextTutorial);
                        _speechBubbleUI.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                        _speechBubbleUI.transform.DOScale(1f, 0.1f);
                        _pointerUI.gameObject.SetActive(false);
                        _background.gameObject.SetActive(true);
                        _background.raycastTarget = true;
                    }
                    break;
                case TutorialStepEnum.IntroduceGimmick:
                    {
                        Vector3 gimmickPoint = worldCamera.WorldToScreenPoint(_tutorialStage.GameField.WorldCharacters[0].transform.position);
                        _speechBubbleUI.Init(gimmickPoint, "This is obstacle", DoNextTutorial);
                        _speechBubbleUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        _speechBubbleUI.transform.DOScale(1f, 0.1f);
                        _pointerUI.gameObject.SetActive(false);
                        _background.gameObject.SetActive(true);
                        _background.raycastTarget = true;
                    }
                    break;
                case TutorialStepEnum.IntroduceGoal:
                    {
                        Vector3 goalPoint = worldCamera.WorldToScreenPoint(_tutorialStage.GameField.GetGoalObject().transform.position);
                        _speechBubbleUI.gameObject.SetActive(true);
                        _speechBubbleUI.Init(goalPoint, "Here is gaol", DoNextTutorial);
                        _speechBubbleUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        _speechBubbleUI.transform.DOScale(1f, 0.1f);
                        _pointerUI.gameObject.SetActive(false);
                        _background.gameObject.SetActive(true);
                        _background.raycastTarget = true;
                    }
                    break;
                case TutorialStepEnum.TouchPlayer:
                    {
                        Vector3 playerPoint = worldCamera.WorldToScreenPoint(_tutorialStage.GameField.GetPlayer().transform.position);
                        _speechBubbleUI.gameObject.SetActive(true);
                        _speechBubbleUI.Init(playerPoint, "Please touch your character",null);
                        _speechBubbleUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        _speechBubbleUI.transform.DOScale(1f, 0.1f);
                        _pointerUI.transform.position = playerPoint + Vector3.right * 50f;
                        if (null != _pointerTween && _pointerTween.active)
                            _pointerTween.Kill();
                        _pointerTween = _pointerUI.transform.DOScale(0.7f, 1f).SetLoops(-1);
                        _pointerUI.gameObject.SetActive(true);
                        _background.gameObject.SetActive(true);
                        _background.raycastTarget = false;
                    }
                    break;
                case TutorialStepEnum.DragToGoal:
                    {
                        Vector3 playerPoint = worldCamera.WorldToScreenPoint(_tutorialStage.GameField.GetPlayer().transform.position);
                        Vector3 goalPoint = worldCamera.WorldToScreenPoint(_tutorialStage.GameField.GetGoalObject().transform.position);
                        _speechBubbleUI.gameObject.SetActive(true);
                        _speechBubbleUI.Init(playerPoint, "Try drag to goal",null);
                        _speechBubbleUI.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        _speechBubbleUI.transform.DOScale(1f, 0.1f);
                        if (null != _pointerTween && _pointerTween.active)
                            _pointerTween.Kill();
                        _pointerUI.transform.position = playerPoint;
                        _pointerTween = _pointerUI.transform.DOMoveY(goalPoint.y, 1f).SetLoops(-1);
                        _pointerUI.localScale = Vector3.one;
                        _pointerUI.gameObject.SetActive(true);
                        _background.gameObject.SetActive(true);
                        _background.raycastTarget = false;
                    }
                    break;
                default:
                    {
                        if (null != _pointerTween && _pointerTween.active)
                        {
                            _pointerTween.Kill();
                            _pointerTween = null;
                        }
                        _pointerUI.gameObject.SetActive(false);
                        _speechBubbleUI.gameObject.SetActive(false);
                        _background.gameObject.SetActive(false);
                    }
                    break;

            }
        }
        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch(messageName)
            {
                case GameMessageEnum.ChangedTutorialStep:
                    ExecuteCurrentTutorial();
                    break;
                case GameMessageEnum.EndTutorial:
                    CloseUI();
                    break;
            }
        }
    }
}