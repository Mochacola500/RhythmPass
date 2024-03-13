using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening;

namespace Dev
{
    public enum TileTypeEnum : int
    {
        Normal = 0,
        Empty = 1,
    }
    public class TileObject : WorldObject
    {
        [SerializeField] private TileTypeEnum _tileType;
        [SerializeField] private float _height;
        [SerializeField] private List<WorldCharacter> _worldCharacters;
        [SerializeField] private StaticObject _staticObject;
        public float Height => _height; 
        public List<WorldCharacter> WorldCharacters => _worldCharacters;
        public StaticObject StaticObject => _staticObject;
        public TileTypeEnum TileType => _tileType;
        private TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _tween;
        private Material _mat;
        private Tween _boundTween;
        public override void SetIndex(Vector2Int index)
        {
            _index = index;
            transform.position = new Vector3(_index.x, _height, _index.y);
        }
        public void OccupyTile(WorldCharacter worldCharacter)
        {
            if (null == worldCharacter)
                throw new System.Exception();

            _worldCharacters.Add(worldCharacter);
        }
        public void LeaveTile(WorldCharacter worldCharacter)
        {
            if (null == worldCharacter)
                throw new System.Exception();

            var removed = false;
            for (int i = 0; i < _worldCharacters.Count; i++)
            {
                if(_worldCharacters[i] == worldCharacter)
                {
                    _worldCharacters.RemoveAt(i);
                    removed = true;
                }
            }

            if(false == removed)
            {
                //throw new System.Exception();
            }
        }
        public WorldCharacter GetUpperWorldCharacter()
        {
            if (0 == _worldCharacters.Count)
                return null;
            return _worldCharacters[0];
        }
        public void BindStaticObject(StaticObject staticObject)
        {
            _staticObject = staticObject;
        }
        public Vector3 GetTopPosition()
        {
            Vector3 result = transform.position;
            result.y = 0f;
            return result;
        }
        public void Bounce()
        {
            if (null != _boundTween && _boundTween.active)
                return;
            _boundTween = transform.DOMove(new Vector3(transform.position.x, -1f, transform.position.z), 0.1f).SetLoops(2, LoopType.Yoyo);
        }
        public bool CheckCanPath()
        {
            switch(_tileType)
            {
                case TileTypeEnum.Empty:
                    return false;
            }

            return true;
        }
    }
}