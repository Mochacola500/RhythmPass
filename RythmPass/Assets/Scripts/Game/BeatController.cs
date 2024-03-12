using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Sound
{
    using Data;
    public class BeatController : IAsyncInitializer
    {
        BGMRecord _bgmRecord;
        List<long> _beatList;

        int _adjacentIndex;
        public void Init(int bgmID)
        {
            Init(DataManager.BGMTable.GetRecord(bgmID));
        }
        public void Init(BGMRecord bgmRecord)
        {
            _bgmRecord = bgmRecord;
            Game.SoundManager.BgmAudioClipPool.LoadClip(_bgmRecord.ID,(clip)=> 
            {
                _beatList = BeatConverter.GetBeatList(_bgmRecord.BPM, clip.length);
            });
            _adjacentIndex = 0;
            Game.SoundManager.SetGameBGMVolume(1f);
        }
        public bool IsLoadComplete()
        {
            return Game.SoundManager.BgmAudioClipPool.IsContain(_bgmRecord.ID);
        }
        public void Update()
        {
            if (false == IsLoadComplete())
                return;
            if (false == Game.World.CurrentStage.IsPlaying)
                return;
            if (_adjacentIndex >= _beatList.Count)
                return;
            float currentPlayTime = Game.SoundManager.BGMSource.time;
            long millisecondTime = (long)(currentPlayTime * 1000f);

            while (true)
            {
                if (_adjacentIndex >= _beatList.Count)
                    break;
                long adjacentTime = _beatList[_adjacentIndex];

                if (adjacentTime <= millisecondTime)
                {
                    Game.Instance.SendGameMessage(GameMessageEnum.BeatTime, new GameMessage.BeatTime() 
                    {
                        NodeTime = adjacentTime,
                        Index = _adjacentIndex
                    });
                    _adjacentIndex++;
                }
                else
                {
                    break;
                }
            }
        }
        public void Play()
        {
            Game.SoundManager.PlayBGM(_bgmRecord.ID,true, OnBGMEnd);
        }
        public void Pause()
        {
            Game.SoundManager.PauseBGM();
        }
        public long GetCurrentTime()
        {
            return (long)(Game.SoundManager.BGMSource.time * 1000f);
        }
        public bool TryGetBeat(int index, out long beat)
        {
            if (index >= _beatList.Count)
            {
                beat = -1;
                return false;
            }

            beat = _beatList[index];

            return true;
        }
        public int GetBeatCount()
        {
            return _beatList.Count;
        }
        void OnBGMEnd()
        {
            _adjacentIndex = 0;
        }
    }
}