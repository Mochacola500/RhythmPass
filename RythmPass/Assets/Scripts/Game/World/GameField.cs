using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Dev
{
    using Data;
    using System;
    public class GameField : MonoBehaviour, IGameMessageReceiver, IAsyncInitializer
    {
        public readonly static Plane ZeroPlane = new Plane(Vector3.up, 0f);
        [SerializeField] private WorldCamera _worldCamera;
        [SerializeField] private Light _light;
        [SerializeField] private Vector2Int _cellSize;
        [SerializeField] private List<FieldObject> _environmentObjects;
        [SerializeField] private TileGroup _tileGroup;
        [SerializeField] private PathRenderer _pathRenderer;

        readonly List<WorldCharacter> _worldCharacterList = new List<WorldCharacter>();
        readonly List<StaticObject> _staticObjectList = new List<StaticObject>();
        readonly List<ItemObject> _itemObjectList = new List<ItemObject>();

        public readonly List<PlayerCharacter> PlayerCharacterList = new List<PlayerCharacter>();
        public Light Light => _light;
        public Vector2Int CellSize { get => _cellSize; set { _cellSize = value; } }
        public List<FieldObject> EnvironmentObjects => _environmentObjects;
        public List<WorldCharacter> WorldCharacters => _worldCharacterList;
        public WorldCamera WorldCamera => _worldCamera;
        public PathRenderer PathRenderer => _pathRenderer;
        public void Init()
        {
            // {{ todo 이후 수정
            WorldCharacter[] worldCharacters = GetComponentsInChildren<WorldCharacter>();
            foreach (var worldCharacter in worldCharacters)
            {
                //todo 임시 예외처리
                if (worldCharacter is PlayerCharacter)
                    continue;
                worldCharacter.Init();
                _worldCharacterList.Add(worldCharacter);
            }
            StaticObject[] staticObjects = GetComponentsInChildren<StaticObject>();
            foreach (var staticObject in staticObjects)
            {
                staticObject.Init();
                _staticObjectList.Add(staticObject);
            }
            ItemObject[] itemObjects = GetComponentsInChildren<ItemObject>();
            foreach (var itemObject in itemObjects)
            {
                itemObject.Init();
                _itemObjectList.Add(itemObject);
            }
            // }}
            InitPlayer();

        }
        public bool IsLoadComplete()
        {
            foreach(var player in PlayerCharacterList)
            {
                if (false == player.IsLoadComplete())
                    return false;
            }
            return true;
        }
        public void InitPlayer()
        {
            PlayerCharacterList.Clear();
            PlayerCharacter[] playerCharacters = GetComponentsInChildren<PlayerCharacter>();
            foreach(var playerCharacter in playerCharacters)
            {
                InitPlayer(playerCharacter);
                PlayerCharacterList.Add(playerCharacter);
            }
        }
        public void InitPlayer(PlayerCharacter playerCharacter)
        {
            playerCharacter.Init();
        }
        public void Validate()
        {
            if (0 >= CellSize.x * CellSize.y)
            {
                _cellSize = Vector2Int.one;
            }

            _tileGroup?.OnValidateCalledGameField(CellSize);
        }
        private void OnValidate()
        {
            Validate();
        }
        public TileObject GetTile(Vector2Int index)
        {
            return _tileGroup?.GetTile(CellSize, index);
        }
        public void OnStartDrag()
        {
            Game.Instance.DoDarkness();
        }
        public void OnEndDrag()
        {
            Game.Instance.DoBrightness();
        }
        public GoalObject GetGoalObject()
        {
            //todo 캐싱
            foreach (var staticObject in _staticObjectList)
            {
                if (staticObject.IsGoal())
                    return staticObject as GoalObject;
            }
            return null;
        }
        public void ForeachPlayer(Action<PlayerCharacter> query)
        {
            foreach(var player in PlayerCharacterList)
            {
                query?.Invoke(player);
            }
        }
        public bool IsPlayerInTile(Vector2Int index)
        {
            return GetPlayer(index) != null;
        }
        public PlayerCharacter GetPlayer(Vector2Int index)
        {
            foreach(var player in PlayerCharacterList)
            {
                if (player.Index == index)
                    return player;
            }
            return null;
        }
        public PlayerCharacter GetPlayer()
        {
            if (PlayerCharacterList.Count > 0)
                return PlayerCharacterList[0];
            return null;
        }
        public bool IsPlayerMoveToPath()
        {
            foreach (var player in PlayerCharacterList)
            {
                if (player.IsPlayerMoveToPath())
                    return true;
            }
            return false;
        }
        public void TryInteractionWithStaticObject(WorldCharacter worldCharacter)
        {
            TileObject tileObject = GetTile(worldCharacter.Index);
            if (null == tileObject)
                return;
            if (null == tileObject.StaticObject)
                return;

            tileObject.StaticObject.TryInteraction(worldCharacter);
        }
        public void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch (messageName)
            {
                case GameMessageEnum.BeatTime:
                case GameMessageEnum.PlayerMoveEnd:
                    {
                        if (null != _tileGroup)
                            _tileGroup.ProcessGameMessage(messageName, message);
                        foreach (var worldCharacter in _worldCharacterList)
                            if(worldCharacter.gameObject.activeSelf)
                                worldCharacter.ProcessGameMessage(messageName, message);
                        foreach (var staticObject in _staticObjectList)
                            if (staticObject.gameObject.activeSelf)
                                staticObject.ProcessGameMessage(messageName, message);
                        foreach (var itemObject in _itemObjectList)
                            if (itemObject.gameObject.activeSelf)
                                itemObject.ProcessGameMessage(messageName, message);
                        foreach(var playerCharacter in PlayerCharacterList)
                            playerCharacter.ProcessGameMessage(messageName, message);
                    }
                    break;
            }
        }
    }

    public static class GameFieldExtension
    {
        public static TileObject GetTile(this GameField gameField, Vector2Int index)
        {
            return gameField?.GetTile(index);
        }
        public static RayResult GetInputIndex(this GameField gameField, Vector3 screenPoint)
        {
            TileObject resultTile = null;
            var hitIndex = -Vector2Int.one;
            var hitPosition = -Vector3.one;
            WorldCamera worldCamera = null;
            try
            {
                worldCamera = gameField?.WorldCamera;
                if (null == worldCamera)
                    throw new Exception("WorldCamera is not connected");

                var ray = gameField.WorldCamera.GetRay(screenPoint);
                if (GameField.ZeroPlane.Raycast(ray, out var factor))
                {
                    hitPosition = ray.GetPoint(factor);
                    hitIndex = new Vector2Int(Mathf.RoundToInt(hitPosition.x), Mathf.RoundToInt(hitPosition.z));
                    resultTile = gameField.GetTile(hitIndex);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return new RayResult(worldCamera, resultTile, hitPosition, hitIndex);
        }
    }

}