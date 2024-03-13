using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using DG.Tweening;
using UnityEngine.Animations;

namespace Dev
{
    public class WorldCamera : MonoBehaviour
    {
        [SerializeField] Camera _camera;
        public Camera Camera => _camera;

        TransformShakeService m_TmShaker;
        LookAtConstraint m_LookAtConstraint;

        private float _cameraSize;
        public float CameraSize => _cameraSize;
        
        public readonly static float StandarScreenRatio = 9f / 16f;
        public readonly static float StandarCameraSize = 8f;

        public void Awake()
        {

            AdaptResolution();
            
            m_TmShaker = m_TmShaker ?? new(transform);
            m_LookAtConstraint = gameObject.GetOrAddComponent<LookAtConstraint>();
        }
        
        private void AdaptResolution()
        {
            var size = StandarCameraSize;

            // 1:2 해상도까지만.. 플립의 9:22는 너무갔다..
            var screenRatio = (float)Screen.width / Screen.height;
            if (1.1f < screenRatio)
            {
                Debug.LogError("왜 가로모드임??????");
            }

            var factor = StandarScreenRatio / UnityEngine.Mathf.Clamp(screenRatio, 0.5f, 1f);
            _cameraSize = size * factor;            
            SetSize(_cameraSize);
        }
        private void SetSize(float size)
        {
            if (null != _camera)
                _camera.orthographicSize = size;
        }
        public Ray GetRay(Vector3 screenPoint)
        {
            if (null == _camera)
            {
                throw new Exception("Camera is null");
            }
            return _camera.ScreenPointToRay(screenPoint);
        }

        public void CloseUp(float time, float amountSize, Action callbackEnd)
        {
            var startSize = Mathf.Abs(amountSize) + _cameraSize;
            /*
            _camera.orthographicSize = startSize;
            _camera.DOOrthoSize(_cameraSize, time);
            */
            CoroutineUtils.Lerp(this, startSize, _cameraSize, time, (currentSize) =>
            {
                SetSize(currentSize);
            },
            () =>
            {
                callbackEnd?.Invoke();
            });
        }

        public void FarAway(float time, float amountSize, Action callbackEnd)
        {
            var endSize = Mathf.Abs(amountSize) + _cameraSize;
            CoroutineUtils.Lerp(this, _cameraSize, endSize, time, (currentSize) =>
            {
                SetSize(currentSize);
            },
            () =>
            {
                callbackEnd?.Invoke();
            });

        }

        public void Shake(float powerRate = 0.3f, float vibrateRate = 2f, float duration = 0.08f)
        {
            m_LookAtConstraint.constraintActive = false;
            m_TmShaker.Shake(this, powerRate, vibrateRate, duration, () => m_LookAtConstraint.constraintActive = true);
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            _camera = GetComponentInChildren<Camera>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.up * 1f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * 1f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.down * 1f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * 1f * Time.deltaTime);
            }
        }
#endif
    }
}
