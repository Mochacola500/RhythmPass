using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    using Data;

    [System.Serializable]
    public struct CurrencyUIGroup
    {
        public CurrencyEnum CurrencyEnum;
        public CurrencyUI CurrencyUI;
    }
    public class CurrencyUI : MonoBehaviour
    {
        [SerializeField] Text _text;
        [SerializeField] Image _icon;
        public void Init(ICurrency currency)
        {
            if (null == currency)
                return;
            Init(currency.Record, currency.Value);
        }
        public void Init(int currencyID, long amount = 0)
        {
            Init(DataManager.CurrencyTable.GetRecord(currencyID),amount);
        }
        public void Init(CurrencyRecord record, long amount = 0)
        {
            if (null == record)
                return;

            if (null != _text)
                _text.text = amount.ToString("N0");
            if(null != _icon)
            {
                AssetManager.LoadAsync<Sprite>(record.IconPath, (sprite) => 
                {
                    _icon.sprite = sprite;
                });
            }
        }
    }
}