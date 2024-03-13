using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.EditorCode
{
    [CreateAssetMenu(fileName = "DataLoadAsset.asset", menuName = "Dev/Data/DataLoadAsset", order = 2)]
    public class DataLoadAsset : ScriptableObject
    {
        [Header("Folder")]
        public UnityEngine.Object ExcelFolder;
        public UnityEngine.Object CodeFolder;
        public UnityEngine.Object JsonFolder;
    }
}