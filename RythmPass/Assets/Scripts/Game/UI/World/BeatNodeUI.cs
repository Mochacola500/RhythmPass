using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    using Sound;
    public class BeatNodeUI : MonoBehaviour
    {
        [SerializeField] RectTransform _rectTransform;
        long _startTime;
        long _endTime;
        Vector2 _startPoint;
        Vector2 _endPoint;
#if UNITY_EDITOR
        private void OnValidate()
        {
            _rectTransform = transform as RectTransform;
        }
#endif
        public void Init(long startTime, long endTime, Vector2 startPoint, Vector2 endPoint)
        {
            _startTime = startTime;
            _endTime = endTime;
            _startPoint = startPoint;
            _endPoint = endPoint;
            _rectTransform.anchoredPosition = startPoint;
            gameObject.SetActive(true);
        }
        public void UpdateNode()
        {
            if(Game.World.CurrentStage.BeatController.GetCurrentTime() >= _endTime)
            {
                gameObject.SetActive(false);
                return;
            }

            _rectTransform.anchoredPosition = GetPosition();
        }
        Vector2 GetPosition()
        {
            long totalTime = _endTime - _startTime;
            long progressTime = Game.World.CurrentStage.BeatController.GetCurrentTime() - _startTime;
            double ratio = (double)(((decimal)progressTime) / ((decimal)totalTime));
            return Vector2.Lerp(_startPoint, _endPoint, (float)ratio);
        }
    }
}
