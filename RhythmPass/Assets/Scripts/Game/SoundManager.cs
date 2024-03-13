using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
namespace Dev.Sound
{
    using Data;
    public enum SoundTypeEnum : int
    {
        BGM,
        SFX,
        UI
    }
    public struct AudioClipInfo
    {
        public int ID;
        public AudioClip AudioClip;
    }
    public class AudioClipPool
    {
        public readonly SoundTypeEnum SoundType;
        public readonly Dictionary<int, AudioClipInfo> Container = new Dictionary<int, AudioClipInfo>();
        public AudioClipPool(SoundTypeEnum soundType)
        {
            SoundType = soundType;
        }
        public bool IsContain(int id)
        {
            return Container.ContainsKey(id);
        }
        public void GetClip(int id,Action<AudioClip> callback)
        {
            if(Container.TryGetValue(id, out AudioClipInfo clip))
            {
                callback?.Invoke(clip.AudioClip);
            }
            else
            {
                LoadClip(id, callback);
            }
        }
        public void Release()
        {
            //todo addressable 해제
            Container.Clear();
        }
        public void LoadClip(int id,Action<AudioClip> callback)
        {
            if (Container.TryGetValue(id, out var clipInfo))
            {
                callback?.Invoke(clipInfo.AudioClip);
                return;
            }

            string audioClipPath = string.Empty;
            switch(SoundType)
            {
                case SoundTypeEnum.BGM:
                    {
                        BGMRecord record = DataManager.BGMTable.GetRecord(id);
                        if (null != record)
                            audioClipPath = record.ClipPath;
                    }
                    break;
                case SoundTypeEnum.SFX:
                    {
                        SFXRecord record = DataManager.SFXTable.GetRecord(id);
                        if (null != record)
                            audioClipPath = record.Path;
                    }
                    break;
                case SoundTypeEnum.UI:
                    {
                        UISoundRecord record = DataManager.UISoundTable.GetRecord(id);
                        if (null != record)
                            audioClipPath = record.Path;
                    }
                    break;
            }

            if (audioClipPath.IsNullOrEmpty())
            {
                callback?.Invoke(null);
                return;
            }
            AssetManager.LoadAsync<AudioClip>(audioClipPath, (clip) => 
            {
                if (null != clip)
                {
                    Container.Add(id, new AudioClipInfo()
                    {
                        ID = id,
                        AudioClip = clip
                    });
                }
                callback?.Invoke(clip);
            });
        }
    }

    public class SoundManager
    {
        public const float MaxDB = 1f;
        public const float MinDB = 0f;
        //public event Action EventBGMEnd;
        public AudioSource BGMSource { get; private set; }
        public AudioSource FXSource { get; private set; }
        public AudioSource UISoundSource { get; private set; }
        //개발자가 컨트롤 하는 volume 값
        public float GameBGMVolume { get; private set; }
        public float GameSFXVolume { get; private set; }
        //유저가 컨트롤 하는 volume 값
        public float UserBGMVolume { get; private set; }
        public float UserSFXVolume { get; private set; }
        public float UserUIVolume { get; private set; }
        public AudioClipPool BgmAudioClipPool { get; private set; }
        public AudioClipPool SfxAudioClipPool { get; private set; }
        public AudioClipPool UIAudioClipPool { get; private set; }

        Action _callbackEndBGM;
        bool _isBGMLoop;

        Coroutine _coroutineFadeBGM;
        Coroutine _coroutineFadeSFX;
        public void Init(Initializer initializer)
        {
            BgmAudioClipPool = new AudioClipPool(SoundTypeEnum.BGM);
            SfxAudioClipPool = new AudioClipPool(SoundTypeEnum.SFX);
            UIAudioClipPool = new AudioClipPool(SoundTypeEnum.UI);

            BGMSource = initializer.BGMSource;
            FXSource = initializer.FXSource;
            UISoundSource = initializer.UISoundSource;

            SetGameBGMVolume(1f);
            SetGameSFXVolume(1f);
            //todo 이후 유저 옵션 저장
            SetUserBGMVolume(0.8f);
            SetUserSFXVolume(0.8f);
            SetUserUISoundVolume(1f);
        }
        public void Update()
        {
            if (null == BGMSource.clip)
                return;
            if(false == BGMSource.isPlaying)
            {
                if(_isBGMLoop)
                {
                    BGMSource.Play();
                }
                _callbackEndBGM?.Invoke();
            }
        }
        public void PlayBGM(int id, bool isLoop = false,Action callbackEndBGM = null)
        {
            BgmAudioClipPool.GetClip(id, (clip) => 
            {
                PlayBGM(clip, isLoop, callbackEndBGM);
            });
        }
        public void PlaySFX(int id)
        {
            SfxAudioClipPool.GetClip(id, (clip) => 
            {
                PlaySFX(clip);
            });
        }
        public void PlayUISound(int id)
        {
            UIAudioClipPool.GetClip(id, (clip) => 
            {
                PlayUISound(clip);
            });
        }
        public void PlayBGM()
        {
            BGMSource.Play();
        }
        public void PauseBGM()
        {
            BGMSource.Pause();
        }
        public void StopBGM()
        {
            BGMSource.Stop();
            BGMSource.clip = null;
        }
        public void SetGameBGMVolume(float volume)
        {
            GameBGMVolume = volume;
            UpdateBGMVolume();
        }
        public void SetGameSFXVolume(float volume)
        {
            GameSFXVolume = volume;
            UpdateSFXvolume();
        }
        public void SetUserBGMVolume(float volume)
        {
            UserBGMVolume = volume;
            UpdateBGMVolume();
        }
        public void SetUserSFXVolume(float volume)
        {
            UserSFXVolume = volume;
            UpdateSFXvolume();
        }
        public void SetUserUISoundVolume(float volume)
        {
            UserUIVolume = volume;
            UpdateUISoundVolume();
        }
        public void FadeGameBGMVolume(float volume, float fadeTime, Action callbackEnd)
        {
            if (null != _coroutineFadeBGM)
            {
                Game.Instance.StopCoroutine(_coroutineFadeBGM);
            }
            _coroutineFadeBGM = Game.Instance.StartCoroutine(CoroutineFadeVolume(true, volume, fadeTime, callbackEnd));
        }
        public void FadeGameSFXVolume(float volume, float fadeTime, Action callbackEnd)
        {
            if (null != _coroutineFadeSFX)
            {
                Game.Instance.StopCoroutine(_coroutineFadeSFX);
            }
            _coroutineFadeSFX = Game.Instance.StartCoroutine(CoroutineFadeVolume(false, volume, fadeTime, callbackEnd));
        }
        void UpdateBGMVolume()
        {
            BGMSource.volume = GameBGMVolume * UserBGMVolume;
        }
        void UpdateSFXvolume()
        {
            FXSource.volume = GameSFXVolume * UserSFXVolume;
        }
        void UpdateUISoundVolume()
        {
            UISoundSource.volume = UserUIVolume;
        }
        void PlayBGM(AudioClip clip, bool isLoop = false,Action callbackEndBGM = null)
        {
            _callbackEndBGM = callbackEndBGM;
            _isBGMLoop = isLoop;

            if (BGMSource.clip != clip)
                BGMSource.clip = clip;
            BGMSource.time = 0f;
            //BGMSource.loop = isLoop;
            PlayBGM();
        }
        void PlaySFX(AudioClip clip)
        {
            FXSource.PlayOneShot(clip, FXSource.volume);
        }
        void PlayUISound(AudioClip clip)
        {
            UISoundSource.PlayOneShot(clip, UISoundSource.volume);
        }
        IEnumerator CoroutineFadeVolume(bool isBGM,float toVolume,float fadeTime,Action callbackEndFade)
        {
            float fromVolume;
            if (isBGM)
                fromVolume = GameBGMVolume;
            else
                fromVolume = GameSFXVolume;

            float time = 0f;
            while(time <= fadeTime)
            {
                float currentVolume = Mathf.Lerp(fromVolume, toVolume, time / fadeTime);
                if (isBGM)
                    SetGameBGMVolume(currentVolume);
                else
                    SetGameSFXVolume(currentVolume);
                time += Game.GameTime.GetDeltaTime();
                yield return null;
            }
            if (isBGM)
            {
                SetGameBGMVolume(toVolume);
                _coroutineFadeBGM = null;
            }
            else
            {
                SetGameSFXVolume(toVolume);
                _coroutineFadeSFX = null;
            }

            callbackEndFade?.Invoke();
        }
    }
}
