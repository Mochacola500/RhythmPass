using System;
namespace Dev.UI
{
    public partial class UIManager
    {
        public static void LoadAsyncStageSelectUI(int stageGroupID, Action callbackCreate = null)
        {
            string path = "Assets/Deploy/UI/Prefab/Lobby/StageSelectUI.prefab";
            Game.UIManager.CreateUI<StageSelectUI>(path, (ui) =>
            {
                ui.Init(stageGroupID);
                callbackCreate?.Invoke();
            });
        }
        public static void LoadAsyncFadeUI(Action<FadeUI> callbackCreate)
        {
            string path = "Assets/Deploy/UI/Prefab/Common/FadeUI.prefab";
            Game.UIManager.CreateUI<FadeUI>(path, callbackCreate);
        }
        public static void LoadAsyncBeatUI()
        {
            string path = "Assets/Deploy/UI/Prefab/World/BeatUI.prefab";
            Game.UIManager.CreateUI<BeatUI>(path, (ui) =>
            {
                ui.Init();
            });
        }
        public static void LoadAsyncMainHUDUI()
        {
            string path = "Assets/Deploy/UI/Prefab/World/MainHUD.prefab";
            Game.UIManager.CreateUI<MainHUDUI>(path, (ui) =>
            {
                ui.Init();
            });
        }
        public static void LoadAsyncOptionUI()
        {
            string path = "Assets/Deploy/UI/Prefab/Common/OptionUI.prefab";
            Game.UIManager.CreateUI<OptionUI>(path, (ui) =>
            {
                ui.Init();
            });
        }
        public static void LoadAsyncStageClearUI(GameMessage.StageEnd result)
        {
            string path = "Assets/Deploy/UI/Prefab/World/StageClearUI.prefab";
            Game.UIManager.CreateUI<StageClearUI>(path, (ui) =>
            {
                ui.Init(result);
            });
        }
        public static void LoadAsyncSystemMessageUI(Action<SystemMessageUI> callbackCreate)
        {
            string path = "Assets/Deploy/UI/Prefab/Common/SystemMessageUI.prefab";
            Game.UIManager.CreateUI<SystemMessageUI>(path, callbackCreate);
        }
        public static void LoadAsyncLobbyUI(Action<LobbyUI> callbackCreate = null)
        {
            string path = "Assets/Deploy/UI/Prefab/Lobby/LobbyUI.prefab";
            Game.UIManager.CreateUI<LobbyUI>(path, (ui) =>
            {
                ui.Init();
                callbackCreate?.Invoke(ui);
            });
        }
        public static void LoadAsyncMessagePopupUI(string title, string message, MessagePopupUI.ButtonTypeEnum buttonType,
            Action callbackConfirm, Action callbackCancel)
        {
            string path = "";
            Game.UIManager.CreateUI<MessagePopupUI>(path, (ui) =>
            {
                ui.Init(title, message, buttonType, callbackConfirm, callbackCancel);
            });
        }
        public static void LoadAsyncStageEnterPopup(StageInfo stageInfo)
        {
            string path = "Assets/Deploy/UI/Prefab/Lobby/StageEnterUI.prefab";
            Game.UIManager.CreateUI<StageEnterUI>(path, (ui) =>
            {
                ui.Init(stageInfo);
            });
        }
        public static void LoadAsyncRewardUI(IRewardInfo rewardInfo, Action callbackClose)
        {
            string path = "Assets/Deploy/UI/Prefab/World/RewardUI.prefab";
            Game.UIManager.CreateUI<RewardUI>(path, (ui) =>
            {
                ui.Init(rewardInfo, callbackClose);
            });
        }
        public static void LoadAsyncNextStageEnterUI(StageInfo stageInfo)
        {
            string path = "Assets/Deploy/UI/Prefab/World/NextStageEnterUI.prefab";
            Game.UIManager.CreateUI<StageEnterUI>(path, (ui) =>
            {
                ui.Init(stageInfo);
            });
        }
        public static void LoadAsyncStageFailUI(StageInfo stageInfo)
        {
            string path = "Assets/Deploy/UI/Prefab/World/StageFailUI.prefab";
            Game.UIManager.CreateUI<StageFailUI>(path, (ui) =>
            {
                ui.Init(stageInfo);
            });
        }
        public static void LoadAsyncTutorialUI()
        {
            string path = "Assets/Deploy/UI/Prefab/World/TutorialUI.prefab";
            Game.UIManager.CreateUI<TutorialUI>(path, (ui) =>
            {
                ui.Init();
            });
        }
    }
}