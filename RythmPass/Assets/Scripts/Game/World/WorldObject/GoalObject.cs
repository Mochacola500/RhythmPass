using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class GoalObject : StaticObject
    {
        public override bool IsStation()
        {
            return true;
        }
        public override bool IsGoal()
        {
            return true;
        }
        public override void TryInteraction(WorldCharacter worldObject)
        {
            if (null == worldObject)
                return;
            //todo °³¼±
            if (false == worldObject is PlayerCharacter )
                return;

            Game.World.CurrentStage.OnPlayerCharacterArriveGoal();
        }
    }
}