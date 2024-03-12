using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dev.Sound
{
    using Data;
    public class BeatConverter 
    {
        //지금은 BPM대로 노드 시간을 주지만 이후에는 툴을 활용해서 직접 노드를 찍을 수 있게 처리
        public static List<long> GetBeatList(int bpm,float length)
        {
            List<long> result = new List<long>();
            long bps = GetBPS(bpm);
            long totalTime = (long)(length * GameTime.SECOND);
            long temp = bps;

            while(temp < totalTime)
            {
                result.Add(temp);
                temp += bps;
            }

            return result;
        }

        static long GetBPS(int bpm)
        {
            double decimalValue =  60.0 / (double)bpm;
            long result = (long)(decimalValue * (double)GameTime.SECOND);
            return result;
        }
    }
}