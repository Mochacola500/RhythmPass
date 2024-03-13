using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class GizmoRenderer : MonoBehaviour
    {
        public enum GizmoTypeEnum : int
        {
            Sphere = 0,
        }
        [SerializeField] GizmoTypeEnum _type;
        [SerializeField] Color _color;
        [SerializeField] float _scale;
        private void OnDrawGizmos()
        {
            Gizmos.color = _color;

            switch(_type)
            {
                case GizmoTypeEnum.Sphere:
                    Gizmos.DrawWireSphere(transform.position, _scale);
                    break;
            }
        }
    }
}