using System;
using UnityEngine;

namespace Dev
{
	public class UtilTime
	{
		public static DateTime TimeStampToDateTime(long timeStampMs)
		{
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			dtDateTime = dtDateTime.AddMilliseconds(timeStampMs);
			return dtDateTime;
		}

		public static long DateTimeToTimeStamp(DateTime dateTime)
		{
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (long)dateTime.Subtract(dtDateTime).TotalMilliseconds;
		}

		public static long StringToTimeStamp(string timeStr)
		{   // YYYY/MM/DD/hh/mm
			if (12 != timeStr.Length)
				return 0;

			DateTime dateTime = new DateTime(
				Convert.ToInt32(timeStr.Substring(0, 4)),
				Convert.ToInt32(timeStr.Substring(4, 2)),
				Convert.ToInt32(timeStr.Substring(6, 2)),
				Convert.ToInt32(timeStr.Substring(8, 2)),
				Convert.ToInt32(timeStr.Substring(10, 2)),
				0, 0);

			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (long)dateTime.Subtract(dtDateTime).TotalMilliseconds;
		}

		public static long ToTimeStamp(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			DateTime dateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return (long)dateTime.Subtract(dtDateTime).TotalMilliseconds;
		}
	}
}

