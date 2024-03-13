using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class PortalObject : StaticObject
    {
        [SerializeField] PortalObject _otherPortalObject;

        public override bool IsStation()
        {
            return true;
        }
        public override void TryInteraction(WorldCharacter worldCharacter)
        {
            if (false == CheckCanTeleport(worldCharacter))
                return;

            worldCharacter.StartTeleport(this, _otherPortalObject);
        }
        bool CheckCanTeleport(WorldCharacter worldCharacter)
        {
            if (null == _otherPortalObject)
                return false;
            if (null == worldCharacter)
                return false;
            if (worldCharacter.IsDead())
                return false;
            if (worldCharacter.Index != Index)
                return false;
            return true;
        }
    }
}