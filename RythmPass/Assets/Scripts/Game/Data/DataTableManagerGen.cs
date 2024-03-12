//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dev.Data
{
    using System;
    
    public partial class DataManager
    {
        public static void LoadDataJson(String path, Action<SerializeData> callback)
        {
            SerializeData data = new SerializeData();
			CurrencyRecordList.LoadJson(path + "/Currency.json", (sd) => { data.currencyData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			DescRecordList.LoadJson(path + "/Desc.json", (sd) => { data.descData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			RewardRecordList.LoadJson(path + "/Reward.json", (sd) => { data.rewardData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			BGMRecordList.LoadJson(path + "/BGM.json", (sd) => { data.bGMData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			SFXRecordList.LoadJson(path + "/SFX.json", (sd) => { data.sFXData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			UISoundRecordList.LoadJson(path + "/UISound.json", (sd) => { data.uISoundData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			StageRecordList.LoadJson(path + "/Stage.json", (sd) => { data.stageData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			StageGroupRecordList.LoadJson(path + "/StageGroup.json", (sd) => { data.stageGroupData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			StageScoreRecordList.LoadJson(path + "/StageScore.json", (sd) => { data.stageScoreData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });
			StageScoreGroupRecordList.LoadJson(path + "/StageScoreGroup.json", (sd) => { data.stageScoreGroupData = sd; if (CheckLoadComplete(data)) callback.Invoke(data); });;
        }
        public static Boolean CheckLoadComplete(SerializeData data)
        {
            
			if (null == data.currencyData) return false;
			if (null == data.descData) return false;
			if (null == data.rewardData) return false;
			if (null == data.bGMData) return false;
			if (null == data.sFXData) return false;
			if (null == data.uISoundData) return false;
			if (null == data.stageData) return false;
			if (null == data.stageGroupData) return false;
			if (null == data.stageScoreData) return false;
			if (null == data.stageScoreGroupData) return false;
			return true;
        }
        [Serializable]
        public class SerializeData
        {
            public CurrencyRecordList currencyData;
            public DescRecordList descData;
            public RewardRecordList rewardData;
            public BGMRecordList bGMData;
            public SFXRecordList sFXData;
            public UISoundRecordList uISoundData;
            public StageRecordList stageData;
            public StageGroupRecordList stageGroupData;
            public StageScoreRecordList stageScoreData;
            public StageScoreGroupRecordList stageScoreGroupData;
        }
    }
}