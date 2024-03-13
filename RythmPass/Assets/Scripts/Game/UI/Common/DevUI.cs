using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class DevUI : MonoBehaviour
    {
        [SerializeField] Text _isMaterButtonText;
        private void Awake()
        {
            if (null != _isMaterButtonText)
                _isMaterButtonText.text = Game.Instance.IsMasterMode ? "MasterMode on" : "MasterMode off";
        }
        public void OnClickAllStageOpen()
        {
#if MASTER
            Game.Instance.IsMasterMode = !Game.Instance.IsMasterMode;
            if (null != _isMaterButtonText)
                _isMaterButtonText.text = Game.Instance.IsMasterMode ? "MasterMode on" : "MasterMode off";
#endif
        }
        public void OnClickDeleteLocalData()
        {
            Game.LocalData.DeleteLocalData();
        }
    }
}