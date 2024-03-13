using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Dev
{
    [System.Serializable]
    public class PatternMove : PatternBase
    {
        [SerializeField] DirectionEnum _direction;
        public override void Execute()
        {
            if (null == _owner)
                return;
            if (false == _owner is WorldCharacter)
                return;
            WorldCharacter worldCharacter = _owner as WorldCharacter;
            worldCharacter.MoveTo(_owner.Index + _direction.DirectionToIndex());
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _direction = (DirectionEnum)EditorGUILayout.EnumPopup(_direction);
        }
#endif
    }
}
