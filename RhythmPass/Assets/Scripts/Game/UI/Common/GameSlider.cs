using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class GameSlider : MonoBehaviour
    {
        public enum SliderTypeEnum : int
        {
            Time,
            Count
        }
        [SerializeField] Slider _slider;
        [SerializeField] Text _text;

        //SliderTypeEnum _sliderType;

        public void InitCountType(int maxValue, int currentValue)
        {
            //_sliderType = SliderTypeEnum.Count;
            if (null != _slider)
            {
                _slider.minValue = 0;
                _slider.maxValue = maxValue;
                _slider.value = maxValue - currentValue;
            }
            if(null != _text)
            {
                _text.text = string.Format("{0} / {1}", maxValue - currentValue, maxValue);
            }
        }
        public void InitTimeType(float maxTime, float currentTime)
        {
            //_sliderType = SliderTypeEnum.Time;
            if (null != _slider)
            {
                _slider.minValue = 0f;
                _slider.maxValue = maxTime;
                _slider.value = maxTime - currentTime;
            }
            if(null != _text)
            {
                _text.text = (maxTime - currentTime).ToString("F0");
            }
        }
    }
}