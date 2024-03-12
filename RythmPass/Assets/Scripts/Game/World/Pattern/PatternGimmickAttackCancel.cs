using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class PatternGimmickAttackCancel : PatternBase
    {
        public override void Execute()
        {
            if (false == _owner is Gimmick)
                return;

            Gimmick owner = _owner as Gimmick;
            owner.AttackCancel();
        }
    }
}