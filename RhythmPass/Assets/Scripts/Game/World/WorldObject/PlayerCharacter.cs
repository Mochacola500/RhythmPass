using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{

    public class PlayerCharacter : WorldCharacter , IAsyncInitializer
    {
        public int MoveCount { get; private set; }
        readonly List<TileObject> _pathQueue = new List<TileObject>();
        int _currentPathIndex;

        Vector2Int _startIndex;
        void OnValidate()
        {
            _worldObjectResource = GetComponent<WorldObjectResource>();
        }
        void Awake()
        {
            _startIndex = Index;
        }
        public override void Init()
        {
            base.Init();

            TileObject tileObject = Game.World.CurrentStage.GameField.GetTile(_startIndex);
            if (null != tileObject)
            {   
                transform.position = new Vector3(tileObject.transform.position.x, 0f, tileObject.transform.position.z);
                SetIndex(_startIndex);
            }
            transform.eulerAngles = new Vector3(0f, CoordinateUtil.GetAngle(_direction), 0f);

            InitHP(1);
            MoveCount = 0;
            ActiveRender(true);
        }
        public bool IsPlayerMoveToPath()
        {
            return _pathQueue.Count != 0 && _pathQueue.Count > _currentPathIndex;
        }
        public bool IsLoadComplete()
        {
            return null != _worldObjectResource;
        }
        public void StartMove()
        {
            _pathQueue.Clear();
            foreach (var tile in Game.World.CurrentStage.PathController.PathTileList)
            {
                _pathQueue.Add(tile);
            }
            _currentPathIndex = 0;
        }
        public override void SetIndex(Vector2Int index)
        {
            _index = index;
        }
        public void PlayWinAnimation()
        {
            if (null != _worldObjectResource)
                _worldObjectResource.Animator.SetTrigger("Win");
        }
        public override bool ReceiveDamage(DamageInfo damageInfo)
        {
            bool result = base.ReceiveDamage(damageInfo);
            if (result)
            {
                //if (false == IsDead())
                //{
                //    DoKnockback();
                //}
                Game.World.CurrentStage.GameField.WorldCamera.Shake();
                FXObject.PlayFX("FX_Impact_Small.prefab", transform.position);
            }
            return result;
        }
        protected override void OnStartMove()
        {
            base.OnStartMove();

            Game.Instance.SendGameMessage(GameMessageEnum.PlayerMoveStart, new GameMessage.PlayerMoveStart() { Player = this});
        }
        protected override void OnEndMove()
        {
            base.OnEndMove();

            if (_lastMoveInfo.MoveReason == MoveReasonEnum.Normal)
            {
                TileObject prevTile = GetPrevTileObject();
                if(null != prevTile)
                {
                    Game.World.CurrentStage.PathController.RemovePath(prevTile);
                }
                Game.Instance.SendGameMessage(GameMessageEnum.PlayerMoveEnd, new GameMessage.PlayerMoveEnd()
                {
                    Player = this,
                    PlayerIndex = Index
                });

                //TryCheckBattle();
            }

            if(0 != _pathQueue.Count && _pathQueue[_pathQueue.Count - 1] == _lastMoveInfo.TargetTileObject)
            {
                OnPathEnd();
            }

            ++MoveCount;
        }
        protected override void OnDead()
        {
            base.OnDead();
            ResetPath();

            Game.Instance.SendGameMessage(GameMessageEnum.PlayerDead, new GameMessage.PlayerDead() { PlayerCharacter = this });
        }
        protected override void DoKnockback()
        {
            _currentPathIndex--;
            base.DoKnockback();
        }
        protected override void OnBeatTime(long beatTime, int beatIndex)
        {
            base.OnBeatTime(beatTime, beatIndex);

            TryMoveToNextPath(beatIndex);
        }
        void OnPathEnd()
        {
            Game.World.CurrentStage.OnPlayerCharacterPathEnd();
        }
        void TryCheckBattle()
        {
            if (IsDead())
                return;
            TileObject tileObject = Game.World.CurrentStage.GameField.GetTile(Index);
            if (null == tileObject)
                return;
            foreach (var worldCharacter in tileObject.WorldCharacters)
            {
                if (worldCharacter.IsDead())
                    continue;
                if (worldCharacter.IsGimmick())
                {
                    //³ªÀÇ ÆÇÁ¤ ½Â
                    //if (worldCharacter.LastMoveBeatIndex < LastMoveBeatIndex)
                    {
                        Game.World.CurrentStage.TrySendDamage(new DamageInfo(this, worldCharacter, Damage));
                        DoKnockback();
                    }
                }
            }
        }
        TileObject GetCurrentMovedTileObject()
        {
            if (_currentPathIndex >= _pathQueue.Count)
                return null;
            return _pathQueue[_currentPathIndex];
        }
        TileObject GetPrevTileObject()
        {
            int index = _currentPathIndex - 1;
            if (index < 0 || index >= _pathQueue.Count)
                return null;
            return _pathQueue[index];
        }
        void TryMoveToNextPath(int beatIndex)
        {
            if(_currentPathIndex < _pathQueue.Count)
            {
                _currentPathIndex++;

                TileObject targetTile = GetCurrentMovedTileObject();
                if (null != targetTile)
                {
                    MoveTo(targetTile.Index, MoveReasonEnum.Normal);
                    SetLastMoveBeatIndex(beatIndex);
                }
            }
        }
        void ResetPath()
        {
            _pathQueue.Clear();
        }
    }
}