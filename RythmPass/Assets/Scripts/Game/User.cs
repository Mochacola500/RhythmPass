using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;
    //todo save load || ¼­¹ö
    public class User 
    {
        readonly Dictionary<int, Currency> CurrencyContainer = new Dictionary<int, Currency>();
        public void Init()
        {
            foreach(var currencyData in DataManager.CurrencyTable.records.Values)
            {
                Currency currency = new Currency(currencyData.ID);
                CurrencyContainer.Add(currency.ID, currency);
            }
        }
        public void SetCurrency(int id, long value)
        {
            Currency currency = GetCurrency(id);
            SetCurrency(currency, value);
        }
        public void AddCurrency(int id, long value)
        {
            Currency currency = GetCurrency(id);
            if(null != currency)
            {
                SetCurrency(currency,currency.Value + value);
            }
        }
        public void SetCurrency(Currency currency, long value)
        {
            if (null != currency)
            {
                currency.SetValue(value);
                Game.Instance.SendGameMessage(GameMessageEnum.ChangedUserCurrency, new GameMessage.ChangedUserCurrency()
                {
                    Currency = currency
                });
            }
        }
        public Currency GetCurrency(int id)
        {
            CurrencyContainer.TryGetValue(id, out var result);
            return result;
        }
        public long GetCurrencyAmount(int id)
        {
            Currency currency = GetCurrency(id);
            if (null != currency)
                return currency.Value;
            return 0;
        }
    }
}