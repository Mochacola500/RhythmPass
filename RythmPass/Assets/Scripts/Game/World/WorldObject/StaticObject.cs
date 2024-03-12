using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class StaticObject : WorldObject
    {
        public override void SetIndex(Vector2Int index)
        {
            base.SetIndex(index);
            if (Game.World != null)
            {
                TileObject tileObject = Game.World.CurrentStage.GameField.GetTile(index);
                if (null != tileObject)
                {
                    tileObject.BindStaticObject(this);
                }
            }
        }
        public virtual bool IsStation()
        {
            return false;
        }
        public virtual bool IsGoal()
        {
            return false;
        }
        public virtual void TryInteraction(WorldCharacter worldCharacter)
        {

        }
    }
}