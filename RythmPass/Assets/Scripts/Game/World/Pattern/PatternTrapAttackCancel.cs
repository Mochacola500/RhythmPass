using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class PatternTrapAttackCancel : PatternBase
    {
        public override void Execute()
        {
            if (false == _owner is StaticTrap)
                return;

            StaticTrap owner = _owner as StaticTrap;
            owner.AttackCancel();
        }
    }
}