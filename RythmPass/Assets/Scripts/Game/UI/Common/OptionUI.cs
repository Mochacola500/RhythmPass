using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class OptionUI : ManagementUIBase
    {
        [SerializeField] Slider _bgmVolumeSlider;
        [SerializeField] Slider _sfxVolumeSlider;
        [SerializeField] Slider _uiVolumeSlider;
        [SerializeField] Toggle[] _languageToggles;
        [SerializeField] GameObject _goToLobbyButtonGroup;
        public void Init()
        {
            if(null != _bgmVolumeSlider)
                _bgmVolumeSlider.value = Game.SoundManager.UserBGMVolume;
            if(null != _sfxVolumeSlider)
                _sfxVolumeSlider.value = Game.SoundManager.UserSFXVolume;
            if(null != _uiVolumeSlider)
                _uiVolumeSlider.value = Game.SoundManager.UserUIVolume;
            if (null != _goToLobbyButtonGroup)
                _goToLobbyButtonGroup.gameObject.SetActive(Game.Instance.CurrentScene == SceneTypeEnum.WorldScene);
            if (null != _languageToggles)
                _languageToggles[(int)Game.LocalData.LanguageID].isOn = true;
        }
        public void OnChangeBGMVolume(float volume)
        {
            Game.SoundManager.SetUserBGMVolume(volume);
        }
        public void OnChangeSFXVolume(float volume)
        {
            Game.SoundManager.SetUserSFXVolume(volume);
        }
        public void OnChangeUISoundVolume(float volume)
        {
            Game.SoundManager.SetUserUISoundVolume(volume);
        }
        public void OnChangeLanguageToggleValue(bool isOn)
        {
            if (isOn)
            {
                for (int i = 0; i < _languageToggles.Length; ++i)
                {
                    if (_languageToggles[i].isOn)
                    {
                        Game.Instance.ChangeLanguage((Data.LanguageIDEnum)i);
                        break;
                    }
                }
            }
        }
        public void OnClickGoToLobby()
        {
            Game.Instance.LoadLobby();
        }
    }
}