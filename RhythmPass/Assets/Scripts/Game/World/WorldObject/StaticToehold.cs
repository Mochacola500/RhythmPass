using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Dev
{
    public class StaticToehold : StaticObject
    {
        [SerializeField] UnityEvent _eventOnPlayer;
        [SerializeField] UnityEvent _eventLeavPlayer;
        public bool IsOnPlayer { get; private set; }
        public override void Init()
        {
            base.Init();
            SetToehold(false);
        }
        public override bool IsStation()
        {
            return true;
        }
        public override void TryInteraction(WorldCharacter worldCharacter)
        {
            if (false == worldCharacter is PlayerCharacter)
                return;
            SetToehold(true);
        }
        void SetToehold(bool isPush)
        {
            if (IsOnPlayer == isPush)
                return;

            IsOnPlayer = isPush;

            if (IsOnPlayer)
                _eventOnPlayer.Invoke();
            else
                _eventLeavPlayer.Invoke();
        }
        void OnPlayerMoveStart()
        {
            bool isLeave = true;
            Game.World.CurrentStage.GameField.ForeachPlayer((player) => 
            {
                if (player.Index == Index)
                    isLeave = false;
            });

            if(isLeave)
            {
                SetToehold(false);
            }
        }
        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            base.ProcessGameMessage(messageName, message);

            switch(messageName)
            {
                case GameMessageEnum.PlayerMoveStart:
                    OnPlayerMoveStart();
                    break;
            }
        }
    }
}