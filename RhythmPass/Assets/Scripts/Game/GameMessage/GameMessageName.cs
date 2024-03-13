using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public enum GameMessageEnum : int
    {
        None = 0,

        BeatTime,                       //���� Ÿ�̹� �޼���
        ChangedStage,                   //�������� ����
        ChangedStageState,              //�������� ���� ����
        PlayerMoveStart,                 //�÷��̾� ���� ����
        PlayerMoveEnd,                  //�÷��̾� ���� ��
        StageEnd,                       //�������� ����
        ChangedUserCurrency,            //��ȭ ����
        GetReward,                      //���� ȹ��
        ChangedTutorialStep,            //Ʃ�丮�� ���� ����
        EndTutorial,                    //Ʃ�丮�� ��
        PlayerDead,                     //�÷��̾� ���
        UpdateTryCount,                 //�������� �õ� Ƚ�� ����
    }
}