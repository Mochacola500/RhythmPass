using System.Collections.Generic;

namespace Dev.Data
{
    public enum LanguageIDEnum
    {
        LanguageIdEnglish = 0,
        LanguageIdKorean = 1,
    }

    public class GameTexts
    {
        readonly Dictionary<int, string> _textDict = new();
        readonly KoreanJosa m_Josa = new();
        int m_LanguageCode;
        DescRecordList _descRecordList;

        public void Init(DescRecordList descList, int languageCode)
        {
            _descRecordList = descList;
            m_LanguageCode = languageCode;
            _textDict.Clear();
            if (null != descList.rows)
            {
                foreach (var record in descList.rows)
                {
                    switch (languageCode)
                    {
                        case (int)LanguageIDEnum.LanguageIdKorean:
                            _textDict.Add(record.ID, record.Kor);
                            break;
                        case (int)LanguageIDEnum.LanguageIdEnglish:
                            _textDict.Add(record.ID, record.Eng);
                            break;
                        //case (int)LanguageIDEnum.LanguageIdTaiwanese:
                        //    _textDict.Add(record.ID, record.Cht);
                        //    break;
                        default:
                            _textDict.Add(record.ID, string.Empty);
                            break;
                    }
                }
            }
        }
        public void OnChangeLanguage()
        {
            Init(_descRecordList, (int)Game.LocalData.LanguageID);
        }
        public string GetText(int textId)
        {
            if (false == _textDict.TryGetValue(textId, out var text))
                return string.Empty;
            text = TextFormat.ApplyTextFormat(text);
            if ((int)LanguageIDEnum.LanguageIdKorean == m_LanguageCode)
            {
                text = m_Josa.Replace(text);
            }
            return text;
        }

        public string FormatText(int textId, params object[] args)
        {
            if (false == _textDict.TryGetValue(textId, out var text))
                return string.Empty;
            text = TextFormat.ApplyTextFormat(text, args);
            if ((int)LanguageIDEnum.LanguageIdKorean == m_LanguageCode)
            {
                text = m_Josa.Replace(text);
            }
            return text;
        }
    }
}