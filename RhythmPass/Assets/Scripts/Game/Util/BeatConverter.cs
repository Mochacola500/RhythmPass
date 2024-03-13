using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dev.Sound
{
    using Data;
    public class BeatConverter 
    {
        //������ BPM��� ��� �ð��� ������ ���Ŀ��� ���� Ȱ���ؼ� ���� ��带 ���� �� �ְ� ó��
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