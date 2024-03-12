using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;
    using UI;
    using Sound;
    /// <summary>
    /// 스테이지 처리 이곳에서 . . .이후 튜토리얼 스테이지 또는 별도의 룰이 필요한 스테이지가 있을 경우 상속해서 처리
    /// </summary>
    public enum StageStateEnum : int
    {
        None = 0,
        WaitStart,
        Playing,
        Pause,
        End
    }
    public class Stage : IGameMessageReceiver, IAsyncInitializer
    {
        public int ID { get; private set; } = int.MinValue;
        public StageInfo StageInfo { get; private set; }
        public StageStateEnum StageState { get; private set; }
        public BeatController BeatController { get; private set; }
        public PathController PathController { get; private set; }
        public GameField GameField { get; private set; }
        public bool IsDragging { get; private set; }
        public float StageTime { get; private set; }
        public bool IsPlaying => StageState == StageStateEnum.Playing;
        // {{ Scrore Check
        protected int _tryCount;
        protected long _startTime;
        protected long _clearTime;
        protected int _moveCount;
        protected bool _isGetScoreItem; //todo 이후 수정
        // }} 
        protected readonly List<Action> m_OnInit = new List<Action>();
        public int TryCount => _tryCount;
        public virtual bool IsTutorialStage => false;
        public virtual void Init(int stageID)
        {
            m_OnInit.Add(StartStageDirection);
            StartInit(stageID);
        }
        protected void StartInit(int stageID)
        {
            ID = stageID;
            StageInfo = Game.StageManager.GetStageInfo(stageID);
            ChangeState(StageStateEnum.WaitStart);

            BeatController = new BeatController();
            BeatController.Init(StageInfo.Record.BGMID);

            _tryCount = 0;

            AssetManager.LoadAsync<GameObject>(StageInfo.Record.PrefabPath, (prefab) =>
            {
                if (null != GameField)
                {
                    GameObject.DestroyImmediate(GameField.gameObject);
                }
                GameField = GameObject.Instantiate<GameField>(prefab.GetComponent<GameField>());
                GameField.Init();

                GameField.WorldCamera.CloseUp(FadeUI.DefaultFadeTime + 1f, 1f, () =>
                {
                    //todo localizeText
                    Game.UIManager.SystemMessageMiddle("Game Start !".GetColorText(Color.white), 1f, () =>
                    {
                        OnstartStage();
                    });
                });
            });

            Game.Instance.StartCoroutine(CoroutineWaitLoadStage());
        }
        IEnumerator CoroutineWaitLoadStage()
        {
            yield return new WaitUntil(() => { return IsLoadComplete(); });

            PathController = new PathController();
            PathController.Init();

            foreach (var action in m_OnInit)
            {
                action?.Invoke();
            }
            m_OnInit.Clear();
        }
        void StartStageDirection()
        {
            Game.SoundManager.SetGameBGMVolume(1f);
            Game.UIManager.FadeIn(FadeUI.DefaultFadeTime, null);
            //GameField.WorldCamera.LerpSize(FadeUI.DefaultFadeTime + 1f, 8f, () =>
            //{
            //    //todo localizeText
            //    Game.UIManager.SystemMessageMiddle("Game Start !".GetColorText(Color.white), 1f, () =>
            //    {
            //        OnstartStage();
            //    });
            //});
        }
        protected virtual void OnstartStage()
        {
            Play();
            _startTime = Game.GameTime.GetClientLocalTime();
        }
        public virtual bool IsLoadComplete()
        {
            return null != GameField &&
                GameField.IsLoadComplete() &&
                null != BeatController &&
                BeatController.IsLoadComplete();
        }
        public virtual void Release()
        {
            GameObject.Destroy(GameField.gameObject);
            GameField = null;
            Game.SoundManager.StopBGM();
        }
        public virtual void Update()
        {
            InputUpdate();
            if (null != BeatController)
                BeatController.Update();
            if (IsPlaying)
                StageTime += Game.GameTime.GetDeltaTime();
        }
        public virtual void Pause()
        {
            if (false == IsPlaying)
                return;
            if (null != BeatController)
                BeatController.Pause();
            ChangeState(StageStateEnum.Pause);
        }
        public virtual void Play()
        {
            if (IsPlaying)
                return;
            if (null != BeatController)
                BeatController.Play();
            ChangeState(StageStateEnum.Playing);
        }
        public bool TrySendDamage(DamageInfo damageInfo)
        {
            if (false == damageInfo.IsValid())
                return false;
            return damageInfo.Victim.ReceiveDamage(damageInfo);
        }
        public virtual void OnPlayerCharacterDead(PlayerCharacter playerCharacter)
        {
            if (IsDragging)
                EndDrag();
            PathController.ResetPath();

            if (CheckCanRespawn())
            {
                _tryCount++;
                GameField.InitPlayer(playerCharacter);
            }
            else
            {
                OnStageEnd(false);
            }

            Game.Instance.SendGameMessage(GameMessageEnum.UpdateTryCount, null);
        }
        public void OnPlayerCharacterPathEnd()
        {
            PathController.ResetPath();
        }
        public virtual void OnPlayerCharacterArriveGoal()
        {
            OnStageEnd(true);
        }
        public void OnPlayerGetScoreItem()
        {
            _isGetScoreItem = true;
        }
        void OnStageEnd(bool isClear)
        {
            IRewardInfo rewardInfo = null;

            if (isClear)
            {
                _clearTime = Game.GameTime.GetClientLocalTime();
                if (StageInfo.CheckCanGetReward())
                    rewardInfo = StageInfo.Reward;

                StageInfo.SetClear();

                foreach (var score in StageInfo.Scores)
                {
                    bool isSucceeded = IsSucceededGetScore(score);
                    score.SetSucceeded(isSucceeded);
                }

                GameField.ForeachPlayer((player) => 
                {
                    player.PlayWinAnimation();
                });
            }

            Game.SoundManager.FadeGameBGMVolume(0f, 1f, () =>
            {
                Game.SoundManager.StopBGM();
            });
            //todo 해상도 대응 후 이 코드 수정 해주세요
            GameField.WorldCamera.FarAway(1.5f, 1f, () =>
            {
                GameMessage.StageEnd message = new GameMessage.StageEnd();
                message.IsClear = isClear;
                message.StageInfo = StageInfo;
                message.RewardInfo = rewardInfo;

                Game.Instance.SendGameMessage(GameMessageEnum.StageEnd, message);
            });
        }
        bool IsSucceededGetScore(StageScore score)
        {
            if (null == score)
                return false;
            switch (score.ScoreType)
            {
                case StageScoreTypeEnum.Clear:
                    return true;
                case StageScoreTypeEnum.TryCount:
                    return score.Value >= _tryCount;
                case StageScoreTypeEnum.Time:
                    return (_clearTime - _startTime) / GameTime.SECOND <= score.Value;
                case StageScoreTypeEnum.GetScoreItem:
                    return _isGetScoreItem;
                case StageScoreTypeEnum.PathCount:
                    {
                        int moveCount = 0;
                        GameField.ForeachPlayer((player) => moveCount += player.MoveCount);
                        return moveCount <= score.Value;
                    }
            }

            return false;
        }
        bool CheckCanRespawn()
        {
            //0이면 무한 도전 가능
            if (0 == StageInfo.Record.MaxTryCount)
                return true;
            if (StageInfo.Record.MaxTryCount > _tryCount)
                return true;
            return false;
        }
        protected void ChangeState(StageStateEnum state)
        {
            if (StageState == state)
                return;
            StageStateEnum prevstate = StageState;
            StageState = state;

            Game.Instance.SendGameMessage(GameMessageEnum.ChangedStageState, new GameMessage.ChangedStageState()
            {
                PrevState = prevstate,
                CurrentState = StageState
            });
        }
        protected virtual void InputUpdate()
        {
            if (IsInputActive())
            {
                if (false == IsDragging)
                {
                    if (Input.GetMouseButtonDown(0) && false == Game.UIManager.IsTouchUIObject())
                    {
                        if (false == TryStartDrag())
                        {
                            TryDetectFieldObject();
                        }
                    }
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        EndDrag();
                    }
                    else
                    {
                        UpdateDrag();
                    }
                }
            }

            Debug.DrawRay(_ray.origin, _ray.direction * 1000f, Color.red);
        }

        private Ray _ray;
        private void TryDetectFieldObject()
        {
            var camera = GameField?.WorldCamera?.Camera;
            if (null == camera)
                return;


            var ray = camera.ScreenPointToRay(Input.mousePosition);
            _ray = ray;

            var layer = LayerMask.GetMask("FieldObject");
            if (Physics.Raycast(ray, out var hit, float.MaxValue, layer))
            {
                Debug.Log(hit.transform.gameObject.name);
                hit.collider.GetComponent<WorldObject>()?.OnTouchObject();
            }

        }
        protected bool TryStartDrag()
        {
            RayResult result = GameField.GetInputIndex(Input.mousePosition);
            if (null == result.HitTile)
                return false;
            if (false == GameField.IsPlayerInTile(result.HitTile.Index))
                return false;
            if (GameField.IsPlayerMoveToPath())
                return false;
            if (PathController.DrawPath(result.HitTile))
            {
                IsDragging = true;
                OnStartDrag();
                return true;
            }
            return false;
        }
        protected void UpdateDrag()
        {
            RayResult result = Game.World.CurrentStage.GameField.GetInputIndex(Input.mousePosition);
            if (null == result.HitTile)
                return;

            if (PathController.DrawPath(result.HitTile))
            {
                //todo fail
            }
        }
        protected void EndDrag()
        {
            IsDragging = false;
            OnEndDrag();
            //Game.SoundManager.FadeGameBGMVolume(1f, 0.2f, null);
        }
        protected virtual bool IsInputActive()
        {
            return IsPlaying;
        }
        protected virtual void OnStartDrag()
        {
            GameField.OnStartDrag();
        }
        protected virtual void OnEndDrag()
        {
            if (PathController.IsValidPath())
            {
                Vector2Int startIndex = PathController.PathTileList[0].Index;
                GameField.GetPlayer(startIndex)?.StartMove();
            }
            else
            {
                PathController.ResetPath();
            }
            GameField.OnEndDrag();
        }
        public virtual void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch (messageName)
            {
                case GameMessageEnum.PlayerDead:
                    OnPlayerCharacterDead(message.Cast<GameMessage.PlayerDead>().PlayerCharacter);
                    break;
            }

            if (null != GameField)
                GameField.ProcessGameMessage(messageName, message);

        }

    }
}