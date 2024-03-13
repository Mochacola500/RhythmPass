
namespace Dev
{
    public class GameMessage
    {
        //���� Ÿ�̹� �޼���
        public struct BeatTime : IGameMessage
        {
            public long NodeTime;
            public int Index;
        }
        //�������� ���� ����
        public struct ChangedStageState : IGameMessage
        {
            public StageStateEnum PrevState;
            public StageStateEnum CurrentState;
        }
        public struct PlayerMoveStart : IGameMessage
        {
            public PlayerCharacter Player;
        }
        public struct PlayerMoveEnd : IGameMessage
        {
            public Dev.PlayerCharacter Player;
            public UnityEngine.Vector2Int PlayerIndex;
        }
        public struct StageEnd : IGameMessage
        {
            public bool IsClear;
            public StageInfo StageInfo;
            public IRewardInfo RewardInfo;
        }
        public struct ChangedStage : IGameMessage
        {
            public int StageID;
        }
        public struct ChangedUserCurrency : IGameMessage
        {
            public ICurrency Currency;
        }
        public struct GetReward : IGameMessage
        {
            public IRewardInfo RewardInfo;
            public System.Action CallbackCloseUI;
        }
        public struct PlayerDead : IGameMessage
        {
            public PlayerCharacter PlayerCharacter;
        }
    }
}