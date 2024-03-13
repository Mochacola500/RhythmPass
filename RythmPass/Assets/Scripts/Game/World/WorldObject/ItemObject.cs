using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public enum WorldItemTypeEnum : int
    {
        ScoreItem,
    }
    public class ItemObject : WorldObject
    {
        [SerializeField] WorldItemTypeEnum WorldItemType;

        void OnPlayerGetItem()
        {
            switch(WorldItemType)
            {
                case WorldItemTypeEnum.ScoreItem:
                    Game.World.CurrentStage.OnPlayerGetScoreItem();
                    break;
            }
            //todo 이후 수정
            gameObject.SetActive(false);
        }
        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch(messageName)
            {
                case GameMessageEnum.PlayerMoveEnd:
                    {

                        if(null != Game.World.CurrentStage.GameField.GetPlayer(Index))
                        {
                            OnPlayerGetItem();
                        }
                    }
                    break;
            }
            base.ProcessGameMessage(messageName, message);
        }
    }
}