using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class WorldObjectResource : MonoBehaviour
    {
        public event Action EventAnimShootProjectile;

        [SerializeField] Animator _animator;
        [SerializeField] Renderer[] _renderers;
        [SerializeField] Transform _projectileMuzzle;
        public Animator Animator { get { return _animator; } }
        public Renderer[] Renderers { get { return _renderers; } }
        public Transform ProjectileMuzzle { get { return _projectileMuzzle; } }
#if UNITY_EDITOR
        private void OnValidate()
        {
            _animator = GetComponentInChildren<Animator>();
            _renderers = GetComponentsInChildren<Renderer>();
        }
#endif
        public void ActiveRender(bool isActive)
        {
            foreach (var renderer in _renderers)
            {
                renderer.gameObject.SetActive(isActive);
            }
        }
        public void OnAnimShootProjectile()
        {
            EventAnimShootProjectile?.Invoke();
        }
    }
}