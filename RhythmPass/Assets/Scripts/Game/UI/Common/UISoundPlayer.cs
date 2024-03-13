using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Dev.UI
{
    public class UISoundPlayer : MonoBehaviour
    {
        public int ID;
        public bool AutoActivePlay;
#if UNITY_EDITOR
        private void OnValidate()
        {
            Button button = GetComponent<Button>();
            if(null != button)
                button.onClick.AddListener(Play);
        }
#endif
        private void OnEnable()
        {
            if (AutoActivePlay)
                Play();
        }
        public void Play()
        {
            if (0 == ID)
                return;

            Game.SoundManager.PlayUISound(ID);
        }
    }
}