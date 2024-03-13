using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class GameTime
    {
        public const long SECOND = 1000;
        public const long MINUTE = 60 * SECOND;
        public const long HOUR = 60 * MINUTE;
        public const long DAY = 24 * HOUR;
        public const long WEEK = 7 * DAY;
        public long GetClientLocalTime()
        {   // Ŭ�� �ӽ��� ���� �ð�
            return UtilTime.DateTimeToTimeStamp(DateTime.Now);
        }
        public DateTime GetClientLocalDateTime()
        {   // Ŭ�� �ӽ��� ���� �ð� ����ü
            return DateTime.Now;
        }
        public float GetDeltaTime()
        {
            return Time.deltaTime;
        }
    }
}