using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class StaticTrap : StaticObject
    {
        static readonly int AttackAnimationKey = Animator.StringToHash("Attack");

        [SerializeField] int _damage;
        bool _isAttack;
        public void Attack()
        {
            _isAttack = true;
            SetAnimation();
        }
        public void AttackCancel()
        {
            _isAttack = false;
            SetAnimation();
        }
        public override void TryInteraction(WorldCharacter worldCharacter)
        {
            if (null == worldCharacter)
                return;

            Game.World.CurrentStage.TrySendDamage(new DamageInfo(this, worldCharacter, _damage));
        }
        void SetAnimation()
        {
            if (null != _worldObjectResource && null != _worldObjectResource.Animator)
                _worldObjectResource.Animator.SetBool(AttackAnimationKey, _isAttack);
        }
    }
}