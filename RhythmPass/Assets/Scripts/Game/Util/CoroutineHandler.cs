using System;
using System.Collections;
using UnityEngine;

namespace Dev
{
    public static class CoroutineUtils
    {
        public static CoroutineHandler Start(MonoBehaviour monoBehaviour, IEnumerator coroutine)
        {
            var handler = new CoroutineHandler();
            handler.Start(monoBehaviour, coroutine);
            return handler;
        }

        public static CoroutineHandler DelayCall(MonoBehaviour monoBehaviour, float time, Action action)
        {
            var handler = new CoroutineHandler();
            handler.DelayCall(monoBehaviour, time, action);
            return handler;
        }

        public static CoroutineHandler DelayCallRealTime(MonoBehaviour monoBehaviour, float time, Action action)
        {
            var handler = new CoroutineHandler();
            handler.DelayCallRealTime(monoBehaviour, time, action);
            return handler;
        }

        public static CoroutineHandler Lerp(MonoBehaviour monoBehaviour, float start, float end, float duration, Action<float> action, Action onFinish = null)
        {
            var handler = new CoroutineHandler();
            handler.Lerp(monoBehaviour, start, end, duration, action, onFinish);
            return handler;
        }
    }

    public struct CoroutineHandler
    {
        MonoBehaviour m_MonoBehaviour;
        Coroutine m_Coroutine;

        public bool IsActive => m_Coroutine != null;

        public void Start(MonoBehaviour monoBehaviour, IEnumerator coroutine)
        {
            m_MonoBehaviour = monoBehaviour;
            m_Coroutine = m_MonoBehaviour.StartCoroutine(coroutine);
        }

        public void Stop()
        {
            if (m_Coroutine != null)
            {
                m_MonoBehaviour?.StopCoroutine(m_Coroutine);
                m_MonoBehaviour = null;
                m_Coroutine = null;
            }
        }

        public void DelayCall(MonoBehaviour monoBehaviour, float time, Action action)
        {
            if (time <= 0)
            {
                action?.Invoke();
                return;
            }
            Start(monoBehaviour, DelayCallImpl(time, action, false));
        }

        public void DelayCallRealTime(MonoBehaviour monoBehaviour, float time, Action action)
        {
            if (time <= 0)
            {
                action?.Invoke();
                return;
            }
            Start(monoBehaviour, DelayCallImpl(time, action, true));
        }

        public void Lerp(MonoBehaviour monoBehaviour, float start, float end, float duration, Action<float> action, Action onFinish)
        {
            Start(monoBehaviour, Lerp(start, end, duration, action, onFinish));
        }

        IEnumerator DelayCallImpl(float time, Action action, bool isRealTime)
        {
            if (isRealTime)
            {
                yield return new WaitForSecondsRealtime(time);
            }
            else
            {
                yield return new WaitForSeconds(time);
            }
            action?.Invoke();
        }

        IEnumerator Lerp(float start, float end, float duration, Action<float> action, Action onFinish)
        {
            if (action == null)
            {
                yield return new WaitForSeconds(duration);
            }
            else
            {
                float e = 0f;
                while (true)
                {
                    e += Time.deltaTime;
                    float t = duration > 0f ? Mathf.Clamp01(e / duration) : 1f;
                    float f = Mathf.Lerp(start, end, t);
                    action.Invoke(f);

                    if (t == 1.0f)
                    {
                        break;
                    }

                    yield return null;
                }
            }
            onFinish?.Invoke();
        }
    }
}