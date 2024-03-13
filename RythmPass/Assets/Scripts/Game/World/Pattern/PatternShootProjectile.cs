using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class PatternShootProjectile : PatternBase
    {
        public override void Execute()
        {
            if (null == _owner)
                return;
            Gimmick gimmick = _owner as Gimmick;
            if (null == gimmick)
                return;
            gimmick.ShootProjectile();
        }
    }
}