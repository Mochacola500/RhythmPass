using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Data
{
    public partial class DataManager 
    {
        public static BGMTable BGMTable { get; private set; }
        public static SFXTable SFXTable { get; private set; }
        public static UISoundTable UISoundTable { get; private set; }
        public static StageTable StageTable { get; private set; }
        public static StageGroupTable StageGroupTable { get; private set; }
        public static StageScoreTable StageScoreTable { get; private set; }
        public static StageScoreGroupTable StageScoreGroupTable { get; private set; }
        public static CurrencyTable CurrencyTable { get; private set; }
        public static RewardTable RewardTable { get; private set; }
        public static GameTexts Texts { get; private set; }
        public static bool IsLoadComplete { get; private set; }
        public void LoadData()
        {
            IsLoadComplete = false;
            InstantiateClass();
            LoadDataJson("Assets/Deploy/Data", (serializeData) => 
            {
                Init(serializeData);
                IsLoadComplete = true;
            });
        }
        void InstantiateClass()
        {
            StageTable = new StageTable();
            StageGroupTable = new StageGroupTable();
            StageScoreTable = new StageScoreTable();
            StageScoreGroupTable = new StageScoreGroupTable();
            BGMTable = new BGMTable();
            SFXTable = new SFXTable();
            UISoundTable = new UISoundTable();
            CurrencyTable = new CurrencyTable();
            RewardTable = new RewardTable();
            Texts = new GameTexts();
        }

        void Init(SerializeData data)
        {
            StageTable.Init(data.stageData);
            StageGroupTable.Init(data.stageGroupData);
            StageScoreTable.Init(data.stageScoreData);
            StageScoreGroupTable.Init(data.stageScoreGroupData);
            BGMTable.Init(data.bGMData);
            SFXTable.Init(data.sFXData);
            UISoundTable.Init(data.uISoundData);
            CurrencyTable.Init(data.currencyData);
            RewardTable.Init(data.rewardData);
            Texts.Init(data.descData, (int)Game.LocalData.LanguageID);
        }
    }
}