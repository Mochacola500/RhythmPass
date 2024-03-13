using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Dev
{
    public class Gimmick : WorldCharacter
    {
        static readonly int AttackAnimationKey = Animator.StringToHash("Attack");

        [SerializeField] protected bool _attackable = true;
        [SerializeField] protected Projectile _projectilePrefab;
        bool _isUpdateBattleCheck;

        public override void Init()
        {
            base.Init();

            if (null != _worldObjectResource)
                _worldObjectResource.EventAnimShootProjectile += OnAnimShootProjectile;
        }
        public void Attack()
        {
            _attackable = true;
            SetAttackAnimation();
        }
        public void AttackCancel()
        {
            _attackable = false;
            SetAttackAnimation();
        }
        public void ShootProjectile()
        {
            if (null != _worldObjectResource && null != _worldObjectResource.Animator)
                _worldObjectResource.Animator.Play("Attack");
        }
        public override bool IsGimmick()
        {
            return true;
        }
        public override bool ReceiveDamage(DamageInfo damageInfo)
        {
            bool result = base.ReceiveDamage(damageInfo);

            if(false == IsDead())
            {
                Bounce();
            }

            return result;
        }
        public void OnAnimShootProjectile()
        {
            if (null == _projectilePrefab)
                return;

            Projectile projectile = Instantiate(_projectilePrefab);
            projectile.Init(_worldObjectResource.ProjectileMuzzle.position, _direction,this);
        }
        void SetAttackAnimation()
        {
            if (null != _worldObjectResource && null != _worldObjectResource.Animator)
                _worldObjectResource.Animator.SetBool(AttackAnimationKey, _attackable);
        }
        void TryCheckBattle()
        {
            TileObject myTile = Game.World.CurrentStage.GameField.GetTile(Index);
            if (null == myTile)
                return;
            Game.World.CurrentStage.GameField.ForeachPlayer((player) => 
            {
                TryCheckBattle(player);
            });
        }
        void TryCheckBattle(WorldCharacter worldCharacter)
        {
            if (null == worldCharacter)
                return;
            if (this == worldCharacter)
                return;
            if (worldCharacter.Index != Index)
                return;
            if (worldCharacter.IsDead())
                return;
            Game.World.CurrentStage.TrySendDamage(new DamageInfo(this, worldCharacter, Damage));
        }
        protected override void OnStartMove()
        {
            base.OnStartMove();

            _isUpdateBattleCheck = true;
        }
        protected override void OnUpdateMove(float ratio)
        {
            base.OnUpdateMove(ratio);

            if (false == _isUpdateBattleCheck)
                return;
            if (ratio < 0.5f)
                return;
            _isUpdateBattleCheck = false;
            
            Game.World.CurrentStage.GameField.ForeachPlayer((player) => 
            {
                //마주보고 오는 상황
                if (player.LastMoveInfo.StartTileObject == LastMoveInfo.TargetTileObject &&
                    player.LastMoveInfo.TargetTileObject == LastMoveInfo.StartTileObject)
                {
                    Game.World.CurrentStage.TrySendDamage(new DamageInfo(this, player, Damage));
                    //Game.World.CurrentStage.TrySendDamage(new DamageInfo(playerCharacter, this, playerCharacter.Damage));
                    //DoKnockback();
                    //if (null != _patternController)
                    //    _patternController.DoRollback();
                }
            });
        }
        public override void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            base.ProcessGameMessage(messageName, message);

            switch (messageName)
            {
                //todo 현재 플레이어와 기믹이 서로의 방향으로 마주보고 동시에 이동할 때 버그 있어 추 후 수정
                case GameMessageEnum.PlayerMoveEnd:
                    {
                        if (_attackable)
                        {
                            TryCheckBattle();
                        }
                    }
                    break;
            }
        }
    }
}