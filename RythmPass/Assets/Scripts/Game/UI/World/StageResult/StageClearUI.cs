using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class StageClearUI : ManagementUIBase
    {
        [SerializeField] ScoreUIBase[] _topScores;
        [SerializeField] ScoreUIBase[] _middleScores;

        GameMessage.StageEnd _message;
        public void Init(GameMessage.StageEnd result)
        {
            _message = result;

            for (int i =0; i < _topScores.Length; ++i)
            {
                _topScores[i].Init(result.StageInfo.Scores[i]);
                _middleScores[i].Init(result.StageInfo.Scores[i]);
            }
            StartCoroutine(CoroutineDirection());
        }
        public override void OnCloseUI()
        {
            base.OnCloseUI();

            StageInfo nextStage = _message.StageInfo.GetNextStage();
            if(null != _message.RewardInfo)
            {
                UIManager.LoadAsyncRewardUI(_message.RewardInfo, () => 
                {
                    if (null == nextStage)
                        Game.Instance.LoadLobby();
                    else
                        UIManager.LoadAsyncNextStageEnterUI(nextStage);

                });
            }
            else
            {
                if (null == nextStage)
                    Game.Instance.LoadLobby();
                else
                    UIManager.LoadAsyncNextStageEnterUI(nextStage);
            }
        }
        
        //todo 이후 수정
        IEnumerator CoroutineDirection()
        {
            WaitForSeconds waitSeconds = new WaitForSeconds(0.3f);
            _topScores[0].FadeIn();
            yield return waitSeconds;
            _topScores[1].FadeIn();
            yield return waitSeconds;
            _topScores[2].FadeIn();
            yield return waitSeconds;
            _middleScores[0].FadeIn();
            yield return waitSeconds;
            _middleScores[1].FadeIn();
            yield return waitSeconds;
            _middleScores[2].FadeIn();
        }
    }
}