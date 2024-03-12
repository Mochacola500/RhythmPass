using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public static class CoordinateUtil
    {
        static public float NormalizeAngle(float angle)
        {
            float a = angle % 360f;
            if(0 > a)
                a += 360f;
            return a;
        }
        static public float GetAngle(DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.Up:
                    return 0; 
                case DirectionEnum.Right:
                    return 90f;
                case DirectionEnum.Down:
                    return 180f;
                case DirectionEnum.Left:
                    return 270f;
            }
            return 0f;
        }
        static public DirectionEnum GetDirection(Vector2Int fromIndex, Vector2Int toIndex)
        {
            if(fromIndex.x == toIndex.x)
            {
                if (fromIndex.y > toIndex.y)
                    return DirectionEnum.Down;
                else
                    return DirectionEnum.Up;
            }
            else
            {
                if (fromIndex.x > toIndex.x)
                    return DirectionEnum.Left;
                else
                    return DirectionEnum.Right;
            }
        }
        static public float RotateAngle(float angle, float rotate)
        {
            return NormalizeAngle(angle + rotate);
        }
        static public float GetRotateSpeedEulerY(float startAngle, float targetAngle,float rotateTime)
        {
            // {{ calc rotate speed 
            float rotateAngle = Mathf.Abs(targetAngle - startAngle);
            if (rotateAngle > 180f)
                rotateAngle = 360f - rotateAngle;
            float speed = rotateAngle / rotateTime;
            // }} 

            // {{ calc rotate direction
            Vector3 forward = Quaternion.Euler(0f, startAngle, 0f) * Vector3.forward;
            Vector3 direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (Vector3.Cross(forward, direction).y < 0f)
                speed *= -1f;
            // }} 
            return speed;
        }
    }
}