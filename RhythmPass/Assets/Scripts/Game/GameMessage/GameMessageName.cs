using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public enum GameMessageEnum : int
    {
        None = 0,

        BeatTime,                       //박자 타이밍 메세지
        ChangedStage,                   //스테이지 변경
        ChangedStageState,              //스테이지 상태 변경
        PlayerMoveStart,                 //플레이어 동작 시작
        PlayerMoveEnd,                  //플레이어 동작 끝
        StageEnd,                       //스테이지 종료
        ChangedUserCurrency,            //재화 변경
        GetReward,                      //보상 획득
        ChangedTutorialStep,            //튜토리얼 스텝 변경
        EndTutorial,                    //튜토리얼 끝
        PlayerDead,                     //플레이어 사망
        UpdateTryCount,                 //스테이지 시도 횟수 변경
    }
}