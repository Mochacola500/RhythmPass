using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace Dev
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FXObject : MonoBehaviour
    {
        [SerializeField]
        ParticleSystem m_RootPs;

        public static FXObject PlayFX(string name, Vector3 position)
        {
            var fullPath = Path.Combine(GameConstant.FX_PrefabDir, name);
            var fx = Game.ObjectPool.RequestGameObject<FXObject>(fullPath);
            fx.transform.position = position;
            fx.Play();
            return fx;
        }

        public static FXObject PlayFX(string name, Transform parentTm)
        {
            var fx = PlayFX(name, Vector3.zero);
            fx.transform.SetParent(parentTm);
            return fx;
        }

        void Awake()
        {
            m_RootPs = gameObject.GetOrAddComponent<ParticleSystem>();
        }

        void LateUpdate()
        {
            if (m_RootPs.IsAlive(withChildren: true) == false)
            {
                Game.ObjectPool.Release(gameObject);
            }
        }

        public void Play()
        {
            m_RootPs.Simulate(0f, true, true);
            m_RootPs.Play(true);
        }

        public void Pause()
        {
            m_RootPs.Simulate(m_RootPs.time, withChildren: false, restart: false);
            m_RootPs.Stop(withChildren: true);
        }

        public void Stop()
        {
            m_RootPs.Stop(withChildren: true);
        }

        public void Release()
        {
            Game.ObjectPool.Release(gameObject);
        }
    }
}