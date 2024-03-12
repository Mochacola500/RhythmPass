using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;

    public class StageScore
    {
        public int ID { get; private set; }
        public int Value { get; private set; }
        public  bool IsSucceeded { get; private set; }
        public StageScoreRecord Record { get; private set; }
        public StageScoreTypeEnum ScoreType => (StageScoreTypeEnum)ID;
        public StageScore(int scoreID, int value, bool isSucceeded)
        {
            ID = scoreID;
            Value = value;
            Record = DataManager.StageScoreTable.GetRecord(scoreID);
            IsSucceeded = isSucceeded;
        }
        public void SetSucceeded(bool isSucceeded)
        {
            IsSucceeded = isSucceeded;
        }
        public string GetScoreText()
        {
            switch((StageScoreTypeEnum)ScoreType)
            {
                case StageScoreTypeEnum.Clear:
                case StageScoreTypeEnum.GetScoreItem:
                    return DataManager.Texts.GetText(Record.Desc);
                case StageScoreTypeEnum.TryCount:
                case StageScoreTypeEnum.Time:
                case StageScoreTypeEnum.PathCount:
                    return DataManager.Texts.FormatText(Record.Desc, Value);
            }
            return string.Empty;
        }
    }
}