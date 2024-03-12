using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class PathRenderer : MonoBehaviour
    {
        [SerializeField] float _pathHeightOffset= 0.3f;
        [SerializeField] LineRenderer _lineRenderer;

        public void ResetPath()
        {
            _lineRenderer.positionCount = 0;
        }
        public void Refresh()
        {
            List<TileObject> pathList = Game.World.CurrentStage.PathController.PathTileList;
            _lineRenderer.positionCount = pathList.Count;
            for(int i =0; i < pathList.Count; ++i)
            {
                _lineRenderer.SetPosition(i, new Vector3(pathList[i].transform.position.x, _pathHeightOffset, pathList[i].transform.position.z));
            }
        }
    }
}