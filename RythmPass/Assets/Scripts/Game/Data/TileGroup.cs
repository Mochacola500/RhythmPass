using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class TileGroup : MonoBehaviour ,IGameMessageReceiver
    {
        public List<TileObject> Tiles => _tiles;
        [SerializeField] private List<TileObject> _tiles;
        public void OnValidateCalledGameField(Vector2Int cellSize)
        {
            _tiles = GetComponentsInChildren<TileObject>(true).ToList();

            for (int i = 0; i < _tiles.Count; i++)
            {
                var indexX = i % cellSize.x;
                var indexY = i / cellSize.x;
                _tiles[i].SetIndex(new Vector2Int(indexX, indexY));
            }
        }

        public void Order()
        {

        }

        public TileObject GetTile(Vector2Int cellSize, Vector2Int index)
        {
            var targetIndex = cellSize.x * index.y + index.x;
            var tiles = Tiles;

            if (0 > targetIndex || tiles.Count <= targetIndex)
                return null;

            return tiles[targetIndex];
        }

        public void SetTile(TileObject tile, Vector2Int cellSize, Vector2Int index)
        {
            var targetIndex = cellSize.x * index.y + index.x;
            var tiles = Tiles;

            if (0 > targetIndex || tiles.Count <= targetIndex)
                return;

            tiles[targetIndex] = tile;
        }

        public void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch(messageName)
            {
                case GameMessageEnum.BeatTime:
                    foreach(var tile in _tiles)
                    {
                        tile.ProcessGameMessage(messageName, message);
                    }
                    break;
            }
        }
    }
}