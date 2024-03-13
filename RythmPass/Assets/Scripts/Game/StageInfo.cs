using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Data;
    public class StageInfo
    {
        public int ID { get; private set; }
        public int Index { get; private set; }
        public bool IsClear { get; private set; }
        public StageRecord Record { get; private set; }
        public StageScore[] Scores { get; private set; }
        public StageGroupInfo StageGroupInfo { get; private set; }
        public Reward Reward { get; private set; }
        public StageInfo(StageRecord record, StageGroupInfo stageGroupInfo)
        {
            ID = record.ID;
            Record = record;
            StageGroupInfo = stageGroupInfo;

            if(0 != record.RewardID)
                Reward = new Reward(record.RewardID);

            Scores = StageManager.GetStageScoreListByGroupID(Record.ScoreGroupID).ToArray();
        }
        public void SetIndex(int index)
        {
            Index = index;
        }
        public void SetClear()
        {
            if (IsClear)
                return;
            IsClear = true;
            if(CheckCanGetReward())
            {
                Reward.SetReceive();
            }
        }
        public bool IsLock()
        {
            return StageGroupInfo.IsLock(this);
        }
        public int GetScorePoint()
        {
            int result = 0;
            if (false == IsClear)
                return result;

            foreach(var score in Scores)
            {
                if (score.IsSucceeded)
                    result++;
            }

            return result;
        }
        public IRewardInfo GetRewardInfo()
        {
            return Reward;
        }
        public bool CheckCanGetReward()
        {
            return null != Reward && false == Reward.IsRecieve;
        }
        public StageInfo GetNextStage()
        {
            return StageGroupInfo.GetNextStage(ID);
        }
        public StageScore GetScore(StageScoreTypeEnum scoreType)
        {
            foreach(var score in Scores)
            {
                if (score.ID == (int)scoreType)
                    return score;
            }
            return null;
        }
        public bool IsTutorialStage()
        {
            return 0 == Index && 0 == StageGroupInfo.Index;
        }
    }
    public class StageGroupInfo
    {
        public int ID { get; private set; }
        public int Index { get; private set; }
        public StageGroupRecord Record { get; private set; }
        public List<StageInfo> StageInfoList { get; private set; }
        public StageGroupInfo(StageGroupRecord record)
        {
            ID = record.ID;
            Record = record;
            StageInfoList = new List<StageInfo>();
            foreach(var stageData in DataManager.StageTable.records.Values)
            {
                if(stageData.GroupID == ID)
                    StageInfoList.Add(new StageInfo(stageData,this));
            }
            StageInfoList.Sort((left, right) => left.Record.Order.CompareTo(right.Record.Order));
            for (int i = 0; i < StageInfoList.Count; ++i)
                StageInfoList[i].SetIndex(i);
        }
        public void SetIndex(int index)
        {
            Index = index;
        }
        public bool IsClear()
        {
            return GetClearStageCount() >= StageInfoList.Count;
        }
        public int GetClearStageCount()
        {
            int result = 0;
            foreach(var stageInfo in StageInfoList)
            {
                if (stageInfo.IsClear)
                    result++;
            }
            return result;
        }
        public bool IsLock(StageInfo stageInfo)
        {
            if (null == stageInfo)
                return false;
            StageInfo prevStage = GetPrevStage(stageInfo.ID);
            if (null == prevStage)
                return false;
            if (prevStage.IsClear)
                return false;
            return true;
        }
        public bool IsLock()
        {
            return Game.StageManager.IsLock(this);
        }
        public StageInfo GetStageInfo(int id)
        {
            foreach(var info in StageInfoList)
            {
                if (info.ID == id)
                    return info;
            }
            return null;
        }
        public StageInfo GetPrevStage(int id)
        {
            for(int i =0; i < StageInfoList.Count; ++i)
            {
                if(StageInfoList[i].ID == id)
                {
                    if (i == 0)
                        return null;
                    else
                        return StageInfoList[i - 1];
                }    
            }
            return null;
        }
        public StageInfo GetNextStage(int id)
        {
            for (int i = 0; i < StageInfoList.Count; ++i)
            {
                if (StageInfoList[i].ID == id)
                {
                    if (i == StageInfoList.Count - 1)
                        return null;
                    else
                        return StageInfoList[i + 1];
                }
            }
            return null;
        }

    }
    public class StageManager
    {
        public int LastEnterStageID { get; private set; }
        public int LastEnterStageGroupID { get; private set; }
        public readonly List<StageGroupInfo> StageGroupList = new List<StageGroupInfo>();
        public void Init()
        {
            StageGroupList.Clear();
            foreach (var stageGroupData in DataManager.StageGroupTable.records.Values)
            {
                StageGroupList.Add(new StageGroupInfo(stageGroupData));
            }
            StageGroupList.Sort((left, right) => left.Record.Order.CompareTo(right.Record.Order));
            for (int i = 0; i < StageGroupList.Count; ++i)
                StageGroupList[i].SetIndex(i);

            Game.SoundManager.FadeGameBGMVolume(1f, 1f, null);
        }
        public StageGroupInfo GetStageGroupInfo(int id)
        {
            foreach (var stageGroup in StageGroupList)
            {
                if (stageGroup.ID == id)
                    return stageGroup;
            }
            return null;
        }
        public StageInfo GetStageInfo(int stageID)
        {
            StageRecord record = DataManager.StageTable.GetRecord(stageID);
            if (null == record)
                return null;
            StageGroupInfo groupInfo = GetStageGroupInfo(record.GroupID);
            if (null == groupInfo)
                return null;
            return groupInfo.GetStageInfo(stageID);
        }
        public bool IsLock(StageGroupInfo stageGroupInfo)
        {
            if (null == stageGroupInfo)
                return false;
            if (0 == stageGroupInfo.Index)
                return false;
            if (StageGroupList[stageGroupInfo.Index - 1].IsClear())
                return false;
            return true;
        }
        public void SetLastSelectStageID(int id)
        {
            LastEnterStageID = id;
            StageRecord record = DataManager.StageTable.GetRecord(id);
            if(null != record)
                LastEnterStageGroupID = record.GroupID;
        }

        //================================================================================================================
        public static List<StageScore> GetStageScoreListByGroupID(int scoreGroupID)
        {
            List<StageScore> result = new List<StageScore>();
            foreach(var stageGroup in DataManager.StageScoreGroupTable.records.Values)
            {
                if(stageGroup.ID == scoreGroupID)
                {
                    result.Add(new StageScore(stageGroup.ScoreID1, stageGroup.ScoreValue1, false));
                    result.Add(new StageScore(stageGroup.ScoreID2, stageGroup.ScoreValue2, false));
                    result.Add(new StageScore(stageGroup.ScoreID3, stageGroup.ScoreValue3, false));
                }
            }
            return result;
        }
    }
}