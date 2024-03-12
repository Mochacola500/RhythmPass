using System;

namespace Dev
{
    public static class CommonUtils
    {

    }

    public class DisposableUtils
    {
        public static IDisposable Create(Action onDisposeAction)
        {
            return new Disposable(onDisposeAction);
        }

        public static IDisposable Create<T>(Action<T> onDisposeAction, T param01)
        {
            return new Disposable<T>(onDisposeAction, param01);
        }
    }

    public struct Disposable : IDisposable
    {
        readonly Action m_OnDisposeAction;

        public Disposable(Action onDisposeAction)
        {
            m_OnDisposeAction = onDisposeAction;
        }

        void IDisposable.Dispose()
        {
            m_OnDisposeAction?.Invoke();
        }
    }

    public struct Disposable<T> : IDisposable
    {
        readonly Action<T> m_OnDisposeAction;
        readonly T m_Param01;

        public Disposable(Action<T> onDisposeAction, T param01)
        {
            m_OnDisposeAction = onDisposeAction;
            m_Param01 = param01;
        }

        void IDisposable.Dispose()
        {
            m_OnDisposeAction?.Invoke(m_Param01);
        }
    }
}