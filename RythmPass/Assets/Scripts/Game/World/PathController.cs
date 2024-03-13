using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;
    public class PathController
    {
        public readonly List<TileObject> PathTileList = new List<TileObject>();
        PathRenderer _pathRenderer;

        public void Init()
        {
            _pathRenderer = Game.World.CurrentStage.GameField.PathRenderer;
        }
        public bool DrawPath(TileObject tileObject)
        {
            if (CheckCanDeletePath(tileObject))
            {
                PathTileList.Remove(GetLastPathTile());
            }
            else if (CheckCanAddPath(tileObject))
            {
                PathTileList.Add(tileObject);
            }
            else
            {
                return false;
            }
            if(null != _pathRenderer)
            {
                _pathRenderer.Refresh();
            }
            //PlayNextSound();
            tileObject.Bounce();
            return true;
        }
        public void RemovePath(TileObject tileObject)
        {
            PathTileList.Remove(tileObject);

            if (null != _pathRenderer)
                _pathRenderer.Refresh();
        }
        public bool IsValidPath()
        {
            if (PathTileList.Count <= 2)
                return false;
            if (null == GetLastPathTile().StaticObject)
                return false;
            if (false == GetLastPathTile().StaticObject.IsStation())
                return false;
            return true;
        }
        public void ResetPath()
        {
            PathTileList.Clear();
            _pathRenderer.ResetPath();
        }
        TileObject GetLastPathTile()
        {
            if (PathTileList.Count == 0)
                return null;
            return PathTileList[PathTileList.Count - 1];
        }
        bool CheckCanAddPath(TileObject tileObject)
        {
            if (null == tileObject)
                return false;
            if (false == tileObject.CheckCanPath())
                return false;
            if(null != GetLastPathTile())
            {
                if (PathTileList.Count >= 2)
                {
                    if (null != GetLastPathTile().StaticObject && GetLastPathTile().StaticObject.IsStation())
                        return false;
                }
                if (GetLastPathTile() == tileObject)
                    return false;
                //대각선 금지
                if (GetLastPathTile().Index.x != tileObject.Index.x &&
                    GetLastPathTile().Index.y != tileObject.Index.y)
                    return false;
                //두 칸 이상 한번에 금지
                if (Mathf.Abs(GetLastPathTile().Index.x - tileObject.Index.x) > 1)
                    return false;
                if (Mathf.Abs(GetLastPathTile().Index.y - tileObject.Index.y) > 1)
                    return false;
            }
            if (PathTileList.Contains(tileObject))
                return false;

            return true;
        }

        bool CheckCanDeletePath(TileObject tileObject)
        {
            if (null == tileObject)
                return false;
            if (false == tileObject.CheckCanPath())
                return false;
            if (null == GetLastPathTile())
            {
                return false;
            }
            if (PathTileList.Count > 1)
            {
                return PathTileList[PathTileList.Count - 2] == tileObject;
            }
            return false;
        }
    }
}