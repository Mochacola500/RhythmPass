using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Dev
{
    public enum WorldCharacterBeatBehaviourEnum : int
    {
        None = 0,
        Bounce,
        Shake
    }
    public enum MoveReasonEnum : int
    {
        Normal,
        Knockback,
    }
    public struct MoveInfo
    {
        public TileObject TargetTileObject;
        public TileObject StartTileObject;
        public Tween MoveTween;
        public MoveReasonEnum MoveReason;
        public bool IsDirectionChange()
        {
            return MoveReason == MoveReasonEnum.Normal;
        }
        public float GetMoveDuration()
        {
            switch(MoveReason)
            {
                case MoveReasonEnum.Normal:
                    return WorldCharacter.MoveDuration;
                case MoveReasonEnum.Knockback:
                    return WorldCharacter.MoveDuration * 0.5f;
            }
            return WorldCharacter.MoveDuration;
        }
        public bool IsSteelMove()
        {
            return null != MoveTween && MoveTween.active;
        }
    }

    public abstract class WorldCharacter : WorldObject
    {
        public const float MoveDuration = 0.1f;

        [Header("Stat")]
        [SerializeField] protected int _fullHP;
        [SerializeField] protected int _hp;
        [SerializeField] protected int _damage;
        [Header("Behaviour")]
        [SerializeField] protected WorldCharacterBeatBehaviourEnum _beatBehaviour = WorldCharacterBeatBehaviourEnum.Bounce;
        [SerializeField] protected float _jumpPower = 1f;
        [SerializeField] protected bool _isRotateMove;
        [SerializeField] protected DirectionEnum _direction = DirectionEnum.None;
        protected int _lastMoveBeatIndex;   //마지막에 이동하 비트의 Index값
        protected float _startMoveTime;
        protected MoveInfo _lastMoveInfo;
        protected float _startLocalScale;
        protected Tween _teleportTween; //todo MoveTween이랑 같이 캐싱 해서 쓸 수 있도록 수정 필요. 굳이 2개 있을 필요가 없어서
        public int FullHP => _fullHP;
        public int HP => _hp;
        public int Damage => _damage;
        public DirectionEnum Direction => _direction;
        public int LastMoveBeatIndex => _lastMoveBeatIndex;
        public MoveInfo LastMoveInfo => _lastMoveInfo;
        public override void Init()
        {
            base.Init();
            if(DirectionEnum.None != _direction)
            {
                transform.eulerAngles = new Vector3(0f, CoordinateUtil.GetAngle(_direction), 0f);
            }

            _startLocalScale = transform.localScale.x;
        }
        public void InitHP(int fullHP)
        {
            _fullHP = _hp = fullHP;
        }
        public virtual bool ReceiveDamage(DamageInfo damageInfo)
        {
            if (IsTeleport())
                return false;
            if (IsDead())
                return false;

            _hp -= damageInfo.Damage;
            //전투는 당분간 봉인 합니다
            _hp = 0;

            if(_hp <= 0)
            {
                _hp = 0;
                OnDead();
            }

            return true;
        }
        public bool IsDead()
        {
            return 0 == _hp; 
        }
        public virtual bool IsGimmick()
        {
            return false;
        }
        public bool MoveTo(MoveInfo moveInfo)
        {
            if (null == moveInfo.TargetTileObject)
                return false;
            if (_lastMoveInfo.IsSteelMove())
            {
                if (_lastMoveInfo.MoveReason == MoveReasonEnum.Normal)
                    StopMoveTween();
                else //넉백 중에는 움직일 수 없음 
                    return false;
            }

            moveInfo.StartTileObject = Game.World.CurrentStage.GameField.GetTile(Index);

            DirectionEnum moveDirection = CoordinateUtil.GetDirection(Index, moveInfo.TargetTileObject.Index);

            if (moveInfo.IsDirectionChange())
                _direction = moveDirection;

            moveInfo.MoveTween = 
                transform.DOJump(moveInfo.TargetTileObject.GetTopPosition(), _jumpPower, 1, moveInfo.GetMoveDuration()).
                OnComplete(OnEndMove).
                OnUpdate(OnUpdateMoveTween);

            if (_isRotateMove && moveInfo.IsDirectionChange())
            {
                Vector3 rotate = new Vector3(0f, CoordinateUtil.GetAngle(_direction), 0f);
                transform.DORotate(rotate, moveInfo.GetMoveDuration());
            }

            _lastMoveInfo = moveInfo;
            _startMoveTime = Game.World.CurrentStage.StageTime;

            SetIndex(moveInfo.TargetTileObject.Index);
            OnStartMove();

            return true;
        }
        public bool MoveTo(Vector2Int index,MoveReasonEnum moveReason = MoveReasonEnum.Normal)
        {
            TileObject tileObject = Game.World.CurrentStage.GameField.GetTile(index);
            return MoveTo(new MoveInfo() 
            {
                TargetTileObject = tileObject,
                MoveReason = moveReason
            });
        }
        public bool MoveTo(DirectionEnum directionEnum, MoveReasonEnum moveReason = MoveReasonEnum.Normal)
        {
            Vector2Int newIndex = Index + directionEnum.DirectionToIndex();
            return MoveTo(newIndex, moveReason);
        }
        public override void SetIndex(Vector2Int index)
        {
            if (Game.World != null)
            {
                TileObject prevTile = Game.World.CurrentStage.GameField.GetTile(_index);
                if (null != prevTile)
                {
                    prevTile.LeaveTile(this);
                }
            }
            base.SetIndex(index);
            if (Game.World != null)
            {
                TileObject currentTile = Game.World.CurrentStage.GameField.GetTile(_index);
                if (null != currentTile)
                {
                    currentTile.OccupyTile(this);
                }
            }
        }
        protected override void OnBeatTime(long beatTime, int beatIndex)
        {
            base.OnBeatTime(beatTime, beatIndex);

            switch(_beatBehaviour)
            {
                case WorldCharacterBeatBehaviourEnum.Bounce:
                    Bounce();
                    break;
            }
        }
        public void StartTeleport(PortalObject startPortal, PortalObject arrivalPortal)
        {
            if (null != _patternController)
                _patternController.Pause();

            _teleportTween = transform.DOJump(transform.position - Vector3.down, _jumpPower, 1, 0.5f).OnComplete(() => 
            {
                TileObject arrivalTile = Game.World.CurrentStage.GameField.GetTile(arrivalPortal.Index);
                Vector3 arrivalPosition = new Vector3(arrivalTile.transform.position.x, -1f, arrivalTile.transform.position.z);
                transform.position = arrivalPosition;
                SetIndex(arrivalTile.Index);

                _teleportTween = transform.DOJump(new Vector3(arrivalPosition.x, 0f, arrivalPosition.z), _jumpPower, 1, 0.5f).OnComplete(() => 
                {
                    if(null != _patternController)
                        _patternController.Play();

                    _teleportTween = null;
                });
            });
        }
        public void SetLastMoveBeatIndex(int index)
        {
            _lastMoveBeatIndex = index;
        }
        protected virtual void OnEndMove()
        {
            _lastMoveInfo.MoveTween = null;

            Game.World.CurrentStage.GameField.TryInteractionWithStaticObject(this);
        }
        protected virtual void OnStartMove()
        {
        }
        protected virtual void OnUpdateMove(float ratio)
        {
           
        }
        protected void StopMoveTween()
        {
            if(_lastMoveInfo.IsSteelMove())
            {
                _lastMoveInfo.MoveTween.Kill();
                _lastMoveInfo.MoveTween = null;
            }
        }
        protected void StopTeleportTween()
        {
            if(IsTeleport())
            {
                _teleportTween.Kill();
                _teleportTween = null;
            }
        }
        protected bool IsTeleport()
        {
            return null != _teleportTween && _teleportTween.active;
        }
        protected void StopAllTween()
        {
            StopMoveTween();
            StopTeleportTween();
        }
        protected virtual void OnDead()
        {
            //todo 이후 수정 test
            if (null != _worldObjectResource)
                _worldObjectResource.ActiveRender(false);
            StopAllTween();
            //Game.EffectManager.TryPlayEffect("CharacterDead", transform.position);
        }
        protected virtual void DoKnockback()
        {
            MoveTo(_direction.GetReverseDirection(),MoveReasonEnum.Knockback);
        }
        protected void Bounce()
        {
            DOTween.Sequence().
                Append(transform.DOScale(_startLocalScale * 0.8f, 0.05f)).
                Append(transform.DOScale(_startLocalScale * 1.3f, 0.05f)).
                Append(transform.DOScale(_startLocalScale, 0.05f));
        }
        void OnUpdateMoveTween()
        {
            float currentPassedTime = Game.World.CurrentStage.StageTime - _startMoveTime;
            OnUpdateMove(currentPassedTime / MoveDuration);
        }
    }
}