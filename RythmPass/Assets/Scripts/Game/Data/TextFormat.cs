using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dev.Data
{
    public static class TextFormat
    {
        public static string ApplyTextFormat(string text, params object[] args)
        {
            string outText = "";
            string tagText = "";
            bool isTag = false;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '<')
                {
                    if (isTag)
                    {
                        outText += tagText;
                        tagText = "";
                    }
                    isTag = true;
                    tagText += c;
                }
                else if (c == '>')
                {
                    if (isTag)
                    {
                        tagText += c;
                        outText += ConvertTagText(tagText, args);
                        isTag = false;
                        tagText = "";
                    }
                    else
                    {
                        outText += c;
                    }
                }
                else
                {
                    if (isTag)
                        tagText += c;
                    else
                        outText += c;
                }
            }

            if (0 < tagText.Length)
                outText += tagText;

            return outText;
        }

        public static string ConvertTagText(string tagText, params object[] args)
        {
            var spitText = tagText.Substring(1, tagText.Length - 2).Split('=');
            if (1 == spitText.Length)
            {
                if (spitText[0].ToLower() == "br")
                {
                    return "\n";
                }
            }
            if (2 == spitText.Length)
            {
#if UNITY_EDITOR
                if (false == UnityEditor.EditorApplication.isPlaying)
                    return tagText;
#endif
                if (spitText[0].Substring(0, 3).ToLower() == "arg")
                {
                    if (3 < spitText[0].Length)
                    {
                        int argNum = Convert.ToInt32(spitText[0].Substring(3));
                        if (0 < argNum && argNum <= args.Length)
                            return ConvertArgTag(spitText[1], args[argNum - 1]);
                    }
                    else
                    {
                        return ConvertArgTag(spitText[1]);
                    }
                }
            }
            return tagText;
        }

        public static string ConvertArgTag(string tagText, object arg = null)
        {
            var spitText = tagText.Split('.');
            if (1 == spitText.Length)
            {
                switch (spitText[0].ToLower())
                {
                    case "number":
                    case "string":
                        {
                            if (null != arg)
                                return string.Format("{0}", arg);
                        }
                        break;
                }
            }
            else if (2 == spitText.Length)
            {
                switch (spitText[0].ToLower())
                {
                    case "time":
                        if(null != arg)
                            return GetTimeText(spitText[1], Convert.ToInt64(arg));
                        break;
                    case "timespan":
                        if (null != arg)
                            return GetTimeSpanText(spitText[1], Convert.ToInt64(arg));
                        break;
                    case "item":
                        if (null != arg)
                            return GetItemText(spitText[1], Convert.ToInt32(arg));
                        break;
                }
            }
            return tagText;
        }

        public static string GetTimeText(string formatText, long time)
        {
            GameTexts texts = DataManager.Texts;
            if (null == texts)
                return string.Empty;

            DateTime dateTime = UtilTime.TimeStampToDateTime(time);
            switch (formatText.ToLower())
            {
                case "ymd":
                        return texts.FormatText(2282, dateTime.Year, dateTime.Month, dateTime.Day);
                case "hms":
                    return texts.FormatText(2283, dateTime.Hour, dateTime.Minute, dateTime.Second);
                case "hm":
                    return texts.FormatText(2284, dateTime.Hour, dateTime.Minute);
                case "hm-1":
                    return string.Format("{0:D2}:{1:D2}", dateTime.Hour, dateTime.Minute);
                case "ymdhm":
                    return texts.FormatText(23636,dateTime.Year,dateTime.Month,dateTime.Day,dateTime.Hour,dateTime.Minute);
                case "ymdhm-1":
                    return texts.FormatText(25739, dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);
            }
            return string.Empty;
        }

        public static string GetTimeSpanText(string formatText, long time)
        {
            GameTexts texts = DataManager.Texts;
            if (null == texts)
                return string.Empty;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds((double)time);
            switch (formatText.ToLower())
            {
                case "dhmsf":
                    {
                        string stringTime = string.Empty;
                        if (0 < timeSpan.Days)
                            stringTime = string.Format("{0}", texts.FormatText(2268, timeSpan.Days));
                        if (0 < timeSpan.Hours)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2269, timeSpan.Hours));
                        if (0 < timeSpan.Minutes)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2270, timeSpan.Minutes));
                        if (0 < timeSpan.Seconds)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, timeSpan.Seconds));
                        if (0 < timeSpan.Milliseconds)
                        {
                            if(0 < stringTime.Length)
                                stringTime = string.Format("{0} {1}", stringTime, timeSpan.Milliseconds / 100);
                            else
                                stringTime = texts.FormatText(2272, timeSpan.Milliseconds / 100);
                        }
                        if (0 == stringTime.Length)
                            stringTime = texts.FormatText(2272, 0);
                        return stringTime;
                    }
                case "dhmsf-1":
                    {
                        if (0 < timeSpan.Days)
                            return texts.FormatText(2268, timeSpan.Days);
                        else if (0 < timeSpan.Hours)
                            return texts.FormatText(2269, timeSpan.Hours);
                        else if (0 < timeSpan.Minutes)
                            return texts.FormatText(2270, timeSpan.Minutes);
                        else if (0 < timeSpan.Seconds)
                            return texts.FormatText(2271, timeSpan.Seconds);
                        else
                            return texts.FormatText(2272, timeSpan.Milliseconds / 100);
                    }
                case "dhmsf-2":
                    {
                        int unitCount = 0;
                        string stringTime = string.Empty;
                        if (0 < timeSpan.Days && 2 > unitCount)
                        {
                            stringTime = string.Format("{0}", texts.FormatText(2268, timeSpan.Days));
                            unitCount++;
                        }
                        if (0 < timeSpan.Hours && 2 > unitCount)
                        {
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2269, timeSpan.Hours));
                            unitCount++;
                        }
                        if (0 < timeSpan.Minutes && 2 > unitCount)
                        {
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2270, timeSpan.Minutes));
                            unitCount++;
                        }
                        if (0 < timeSpan.Seconds && 2 > unitCount)
                        {
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, timeSpan.Seconds));
                            unitCount++;
                        }
                        if (0 < timeSpan.Milliseconds && 2 > unitCount)
                        {
                            if (0 < stringTime.Length)
                                stringTime = string.Format("{0} {1}", stringTime, timeSpan.Milliseconds / 100);
                            else
                                stringTime = texts.FormatText(2272, timeSpan.Milliseconds / 100);
                            unitCount++;
                        }
                        if (0 == stringTime.Length)
                            stringTime = texts.FormatText(2272, 0);
                        return stringTime;
                    }
                case "dhms":
                    {
                        string stringTime = string.Empty;
                        if (0 < timeSpan.Days)
                            stringTime = string.Format("{0}", texts.FormatText(2268, timeSpan.Days));
                        if (0 < timeSpan.Hours)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2269, timeSpan.Hours));
                        if (0 < timeSpan.Minutes)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2270, timeSpan.Minutes));
                        if (0 < timeSpan.Seconds)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, timeSpan.Seconds));
                        if (0 == stringTime.Length)
                            stringTime = texts.FormatText(2271, 0);
                        return stringTime;
                    }
                case "dhmsdn1":
                    {
                        string stringTime = string.Empty;
                        if (0 < timeSpan.Days)
                            stringTime = string.Format("{0}", texts.FormatText(2268, timeSpan.Days));
                        if (0 < timeSpan.Hours)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2269, timeSpan.Hours));
                        if (0 < timeSpan.Minutes)
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2270, timeSpan.Minutes));
                        if (0 < timeSpan.Seconds || 0 < timeSpan.Milliseconds)
                        {
                            if(timeSpan.Seconds == 0)
                                stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, string.Format("{0:N1}", timeSpan.Milliseconds / 1000f)));
                            else if(timeSpan.Milliseconds > 0 && timeSpan.Seconds > 0)
                                stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, string.Format("{0:N1}", timeSpan.Seconds + (timeSpan.Milliseconds / 1000f))));
                            else
                                stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, string.Format("{0}", timeSpan.Seconds)));
                        }
                        if (0 == stringTime.Length)
                            stringTime = texts.FormatText(2271, 0);
                        return stringTime;
                    }
                case "dhms-1":
                    {
                        if (0 < timeSpan.Days)
                            return texts.FormatText(2268, timeSpan.Days);
                        else if (0 < timeSpan.Hours)
                            return texts.FormatText(2269, timeSpan.Hours);
                        else if (0 < timeSpan.Minutes)
                            return texts.FormatText(2270, timeSpan.Minutes);
                        else
                            return texts.FormatText(2271, timeSpan.Seconds);
                    }
                case "dhms-2":
                    {
                        int unitCount = 0;
                        string stringTime = string.Empty;
                        if (0 < timeSpan.Days && 2 > unitCount)
                        {
                            stringTime = string.Format("{0}", texts.FormatText(2268, timeSpan.Days));
                            unitCount++;
                        }
                        if (0 < timeSpan.Hours && 2 > unitCount)
                        { 
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2269, timeSpan.Hours));
                            unitCount++;
                        }
                        if (0 < timeSpan.Minutes && 2 > unitCount)
                        {
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2270, timeSpan.Minutes));
                            unitCount++;
                        }
                        if (0 < timeSpan.Seconds && 2 > unitCount)
                        {
                            stringTime = string.Format("{0} {1}", stringTime, texts.FormatText(2271, timeSpan.Seconds));
                            unitCount++;
                        }
                        if (0 == stringTime.Length)
                            stringTime = texts.FormatText(2271, 0);
                        return stringTime;
                    }
                case "h:m:s":
                    {
                        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Days * 24 + timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds );
                    }
                case "h:m:s-2":
                    {
                        if(0 < timeSpan.Days || 0 < timeSpan.Hours)
                            return string.Format("{0:D2}:{1:D2}", timeSpan.Days * 24 + timeSpan.Hours, timeSpan.Minutes);
                        else
                            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                    }
            }
            return string.Empty;
        }

        public static string GetItemText(string formatText, int itemId)
        {
            //if(null != DataTableManager.Instance)
            //{
            //    var itemRecord = DataTableManager.Instance.ItemsTable.GetRecord(itemId);
            //    if(null != itemRecord)
            //    {
            //        switch (formatText.ToLower())
            //        {
            //            case "name":
            //                return itemRecord.Name; // todo : apply grade color and etc ...
            //        }
            //    }    
            //}
            return string.Empty;
        }

    }
}