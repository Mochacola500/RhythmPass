using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;
    public class LocalData
    {
        public LanguageIDEnum LanguageID { get; private set; }
        public bool IsTutorialClear { get; private set; }
        public void Init()
        {
            LanguageID = (LanguageIDEnum)PlayerPrefs.GetInt("LanguageID", 0);
            IsTutorialClear = PlayerPrefs.GetInt("IsTutorialClear", 0) == 0 ? false : true;
        }
        public void SetLanguageID(LanguageIDEnum languageID)
        {
            if (LanguageID == languageID)
                return;
            LanguageID = languageID;
            PlayerPrefs.SetInt("LanguageID", (int)languageID);
        }
        public void SetTutorialClear()
        {
            if (IsTutorialClear)
                return;

            IsTutorialClear = true;
            PlayerPrefs.SetInt("IsTutorialClear", IsTutorialClear ? 1 : 0);
        }
        public void Save()
        {
            PlayerPrefs.Save();
        }
        public void DeleteLocalData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}