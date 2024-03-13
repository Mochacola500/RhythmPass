using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;
    public interface IRewardInfo
    {
        public int ID { get; }
        public RewardRecord Record { get; }
    }
    public struct RewardInfo : IRewardInfo
    {
        public int ID { get; private set; }
        public RewardRecord Record { get; private set; }
        public RewardInfo(int id)
        {
            ID = id;
            Record = DataManager.RewardTable.GetRecord(id);
        }
        public RewardInfo(RewardRecord record)
        {
            ID = record.ID;
            Record = record;
        }
    }
    public class Reward : IRewardInfo
    {
        public int ID { get; private set; }
        public RewardRecord Record { get; private set; }
        public bool IsRecieve { get; private set; }
        public Reward(int id)
        {
            ID = id;
            Record = DataManager.RewardTable.GetRecord(ID);
            IsRecieve = false;
        }
        public void SetReceive()
        {
            if (IsRecieve)
                return;

            IsRecieve = true;

            switch ((RewardTypeEnum)Record.Type)
            {
                case RewardTypeEnum.Currency:
                    {
                        Game.User.AddCurrency(Record.ConnectID,Record.Value);
                    }
                    break;
            }
        }

        public static string GetRewardIconPath(int rewardID)
        {
            RewardRecord record = DataManager.RewardTable.GetRecord(rewardID);
            if (null == record)
                return string.Empty;
            switch((RewardTypeEnum)record.Type)
            {
                case RewardTypeEnum.Currency:
                    {
                        CurrencyRecord currencyRecord = DataManager.CurrencyTable.GetRecord(record.ConnectID);
                        if (null == currencyRecord)
                            return null;
                        return currencyRecord.IconPath;
                    }
            }

            return string.Empty;
        }
    }
}