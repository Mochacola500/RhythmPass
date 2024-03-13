using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.UI
{
    public class MainTitleUI : ManagementUIBase
    {
        public void OnClick()
        {
            if (false == Game.IsInitDone)
                return;

            UIManager.LoadAsyncLobbyUI();
        }
    }
}