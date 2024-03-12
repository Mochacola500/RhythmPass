using System;
using UnityEngine;

namespace Dev
{
    using Data;

    public interface ICurrency
    {
        int ID { get; }
        long Value { get; }
        CurrencyRecord Record { get; }
    }
    public struct CurrencyInfo : ICurrency
    {
        public int ID { get; private set; }
        public long Value { get; private set; }
        public CurrencyRecord Record { get; private set; }
        public CurrencyInfo(int id, long value = 0)
        {
            ID = id;
            Value = value;
            Record = DataManager.CurrencyTable.GetRecord(id);
        }
    }
    public class Currency : ICurrency
    {
        public int ID { get; private set; }
        public long Value { get; private set; }
        public CurrencyRecord Record { get; private set; }
        public Currency(int id, long value = 0)
        {
            ID = id;
            Record = DataManager.CurrencyTable.GetRecord(id);
            SetValue(value);
        }
        public Currency(CurrencyRecord record, long value)
        {
            ID = record.ID;
            Record = record;
            SetValue(value);
        }
        public void SetValue(long value)
        {
            Value = value;
            if (Value < 0)
                Value = 0;
            else if (Value > Record.MaxCount)
                Value = Record.MaxCount;
        }
    }
}
