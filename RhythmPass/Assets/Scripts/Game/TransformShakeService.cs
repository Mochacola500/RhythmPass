using System;
using UnityEngine;

namespace Dev
{
    public class TransformShakeService
    {
        public bool IsShaking => m_IsShaking;

        Transform m_TargetTm;
        bool m_IsShaking;
        Vector3 m_OriginPos;
        Quaternion m_OriginRotation;
        CoroutineHandler m_RoutineHandler;

        public TransformShakeService(Transform targetTm)
        {
            m_TargetTm = targetTm;
            m_IsShaking = false;
            m_RoutineHandler = new();
        }

        public void Stop()
        {
            m_IsShaking = false;
            m_TargetTm.position = m_OriginPos;
            m_TargetTm.rotation = m_OriginRotation;

            m_RoutineHandler.Stop();
        }

        public void Shake(MonoBehaviour monoBehaviour, float powerRate, float vibrateRate, float duration, Action callbackEnd = null)
        {
            if (m_TargetTm == null || monoBehaviour == null || duration == 0f)
            {
                return;
            }
            m_IsShaking = true;
            m_OriginPos = m_TargetTm.position;
            m_OriginRotation = m_TargetTm.rotation;

            float offset = UnityEngine.Random.Range(0, 11);
            var up = m_TargetTm.up;
            var right = m_TargetTm.right;

            m_RoutineHandler.Lerp(monoBehaviour, 0, 1, duration, (f) =>
            {
                float progressTime = f * duration;
                float dist = powerRate * Mathf.Pow(1 - f, 2f);
                float vdt = (progressTime + offset) * vibrateRate;
                float dx = Mathf.Cos(vdt * 3f + 19f) + Mathf.Sin(vdt * -7f + 5f) * 0.5f;
                float dy = Mathf.Cos(vdt * 3f + 8f) + Mathf.Sin(vdt * -7f - 5f) * 0.5f;
                var v = new Vector2(dx, dy * dist);
                m_TargetTm.position = m_OriginPos + up * v.y + right * v.x;
            },
            () =>
            {
                Stop();
                callbackEnd?.Invoke();
            });
        }
    }
}