using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using UI;
    public enum TutorialStepEnum : int
    {
        IntorducePlayer = 0,
        IntroduceGimmick,
        IntroduceGoal,
        TouchPlayer,
        DragToGoal,
        End
    }
    public class TutorialStage : Stage
    {
        public int CurrentTutorialStep { get; private set; }
        public bool IsTutorialEnd => CurrentTutorialStep >= (int)TutorialStepEnum.End;
        public override bool IsTutorialStage => true;
        public override void Init(int stageID)
        {
            base.Init(stageID);
            CurrentTutorialStep = -1;

            Game.Instance.StartCoroutine(LateInit());
        }
        IEnumerator LateInit()
        {
            yield return null;
            UIManager.LoadAsyncTutorialUI();
        }
        public override bool IsLoadComplete()
        {
            return base.IsLoadComplete() && Game.UIManager.GetUI<TutorialUI>(UINameEnum.TutorialUI) != null;
        }
        protected override void OnstartStage()
        {
            Play();
            _startTime = Game.GameTime.GetClientLocalTime();

            ExecuteNextTutorial();
        }
        protected override void OnStartDrag()
        {
            base.OnStartDrag();

            if(CurrentTutorialStep == (int)TutorialStepEnum.TouchPlayer)
            {
                ExecuteNextTutorial();
            }
        }
        protected override void OnEndDrag()
        {
            switch((TutorialStepEnum)CurrentTutorialStep)
            {
                case TutorialStepEnum.DragToGoal:
                    if (PathController.IsValidPath())
                    {
                        ExecuteNextTutorial();
                    }
                    else
                    {
                        SetTutorialStep((int)TutorialStepEnum.TouchPlayer);
                    }
                    break;
            }

            base.OnEndDrag();
        }
        protected override bool IsInputActive()
        {
            return base.IsInputActive() || false == IsTutorialEnd;
        }
        public void ExecuteNextTutorial()
        {
            SetTutorialStep(CurrentTutorialStep + 1);
        }
        void SetTutorialStep(int step)
        {
            CurrentTutorialStep = step;
            Game.Instance.SendGameMessage(GameMessageEnum.ChangedTutorialStep, null);
        }
        public override void OnPlayerCharacterArriveGoal()
        {
            base.OnPlayerCharacterArriveGoal();

            Game.LocalData.SetTutorialClear();
            Game.Instance.SendGameMessage(GameMessageEnum.EndTutorial, null);
        }
        public override void OnPlayerCharacterDead(PlayerCharacter playerCharacter)
        {
            _tryCount++;
            GameField.InitPlayer(playerCharacter);

            SetTutorialStep((int)TutorialStepEnum.TouchPlayer);
        }
    }
}