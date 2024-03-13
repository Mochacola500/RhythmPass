using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    [RequireComponent(typeof(Button))]
    public class StateButton : MonoBehaviour
    {
        [SerializeField] Button _button;
        [SerializeField] Text _text;
        [Header("OnActive")]
        [SerializeField] Sprite _activeSprite;
        [SerializeField] Color _activeColor = Color.white;
        [Header("OnDisable")]
        [SerializeField] Sprite _disableSprite;
        [SerializeField] Color _disableColor = Color.white;
        public Text Text => _text;
        public Button Button => _button;
#if UNITY_EDITOR
        private void OnValidate()
        {
            _button = GetComponent<Button>();
            _text = GetComponentInChildren<Text>();
        }
#endif
        public void SetState(bool isActive)
        {
            if (null == _button || null == _button.image)
                return;

            _button.image.sprite = isActive ? _activeSprite : _disableSprite;
            _button.image.color = isActive ? _activeColor : _disableColor;
            _button.interactable = isActive;
        }
    }
}