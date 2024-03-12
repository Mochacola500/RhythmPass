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
    using System.Collections.Generic;
    using UnityEngine;
    using Newtonsoft.Json;
    
    [Serializable()]
    public class BGMRecord
    {
        public int ID;
        public string Desc;
        public string ClipPath;
        public int BPM;
    }
    [Serializable()]
    public class BGMRecordList
    {
        public List<BGMRecord> rows;
        public static void LoadJson(String file, Action<BGMRecordList> callback = null)
        {
            AssetManager.LoadAsync<TextAsset>(file, (textAsset) =>
			{
				if (textAsset == null)
				{
					callback?.Invoke(new BGMRecordList());
					return;
				}
				BGMRecordList data = JsonConvert.DeserializeObject<BGMRecordList>(textAsset.text);
				callback?.Invoke(data);
			});;
        }
    }
    public class BGMTable
    {
        public Dictionary<System.Int32, BGMRecord> records = new Dictionary<System.Int32, BGMRecord>();
        public virtual void Init(BGMRecordList data)
        {
            if (null == data || null == data.rows)
				return;
			foreach (var row in data.rows)
			{
				if (!records.ContainsKey(row.ID))
					records.Add(row.ID, row);
			};
        }
        public virtual BGMRecord GetRecord(int key)
        {
            BGMRecord record;
			if (false == records.TryGetValue(key, out record))
				return null;
			return record;
        }
    }
}
