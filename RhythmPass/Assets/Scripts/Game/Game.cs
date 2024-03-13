using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    using Sound;
    using UI;
    using Data;
    using Module.Admob;
    using GoogleMobileAds.Api;

    public class Game : MonoBehaviourSingleton<Game>, IGameMessageReceiver, IAdmobCallbackReceiver
    {
        public static GameTime GameTime { get; private set; }
        public static SoundManager SoundManager { get; private set; }
        public static UIManager UIManager { get; private set; }
        public static Lobby Lobby { get; private set; }
        public static World World { get; private set; }
        public static StageManager StageManager { get; private set; }
        public static User User { get; private set; }
        public static TimerManager TimerManager { get; private set; }
        public static DataManager DataManager { get; private set; }
        public static GameObjectPool ObjectPool { get; private set; }
        public static LocalData LocalData { get; private set; }
        public static bool IsInitDone { get; private set; }
        [SerializeField] Initializer _initilizer;
        [SerializeField] private Rendering.DarknessRenderFeature _darknessRenderFeature;

        GameMessageHandler _gameMessageHandler;
        SceneTransition _sceneTransition;
        AdmobModule _admobModule;
        Action _callbackAdmobWatch; 
#if MASTER
        public DevUI DevUI { get; private set; }
        public bool IsMasterMode { get; set; }
#endif
        public SceneTypeEnum CurrentScene => _sceneTransition.CurrentSceneType;
        void Awake()
        {
            if (_instance != null)
            {
                Destroy(_initilizer.MainCanvas.gameObject);
                Destroy(_initilizer.EventSystem.gameObject);
                Destroy(gameObject);
            }
        }
        void Start()
        {
            AssetManager.Init(string.Empty);
            AssetManager.OnComplete += Init;
            //Init();
        }
        void Update()
        {
            if (null != World)
                World.Update();
            if (null != SoundManager)
                SoundManager.Update();
            if (null != UIManager)
                UIManager.Update();
        }
        void LateUpdate()
        {
            if (null != _gameMessageHandler)
                _gameMessageHandler.ProcessMessage();
        }
        void Init()
        {
            if (null == LocalData)
                LocalData = new LocalData();
            LocalData.Init();
            if (null == _gameMessageHandler)
                _gameMessageHandler = new GameMessageHandler(this);
            if (null == GameTime)
                GameTime = new GameTime();
            if (null == SoundManager)
                SoundManager = new SoundManager();
            SoundManager.Init(_initilizer);
            if (null == UIManager)
                UIManager = new UIManager();
            UIManager.Init(_initilizer);
            if (null == Lobby)
                Lobby = new Lobby();
            if (null == _sceneTransition)
                _sceneTransition = new SceneTransition();
            if (null == User)
                User = new User();
            if (null == StageManager)
                StageManager = new StageManager();
            if (null == TimerManager)
                TimerManager = _initilizer.TimerManager;
            if (null == ObjectPool)
            {
                var pool = new GameObject("ObjectPool");
                pool.SetParent(gameObject);
                ObjectPool = new GameObjectPool(pool.transform);
            }
            if (null == _admobModule)
                _admobModule = new AdmobModule();

            DontDestroyOnLoad(_initilizer.MainCanvas.gameObject);
            DontDestroyOnLoad(gameObject);

            StartCoroutine(CoroutineLoadGameData());
        }

        IEnumerator CoroutineLoadGameData()
        {
            if (null == DataManager)
                DataManager = new DataManager();
            DataManager.LoadData();
            yield return new WaitUntil(() => DataManager.IsLoadComplete);
            OnLoadEndGameData();
        }
        void OnLoadEndGameData()
        {
            User.Init();
            StageManager.Init();
            Lobby.Init();
            UIManager.OnLobbyInit();

            //todo 우선은 테스트 ID로 이후 수정
            AdmobModuleInitializer initializer = new AdmobModuleInitializer();
            initializer.Init(AdmobModule.TestUnitID, this, true);
            _admobModule.Init(initializer);

#if MASTER
            AssetManager.LoadAsync<GameObject>("Assets/Deploy/UI/Prefab/Dev/DevCanvas.prefab", (prefab) =>
            {
                GameObject newObject = Instantiate(prefab,_initilizer.MainCanvas.transform);
                DevUI = newObject.GetComponentInChildren<DevUI>(true);
                newObject.transform.Find("DevButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => 
                {
                    DevUI.gameObject.SetActive(true);
                });
            });
#endif

            IsInitDone = true;
        }
        public void SendGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            if (null != _gameMessageHandler)
                _gameMessageHandler.EnqueueMessage(messageName, message);
        }
        public void LoadStage(int stageID,StageEnterTypeEnum enterType)
        {
            if (false == CheckCanStageEnter(stageID))
                return;

            if(StageEnterTypeEnum.Currency == enterType)
            {
                User.AddCurrency((int)CurrencyEnum.Ticket, -1);
                StartLoadStage(stageID);
            }
            else
            {
#if UNITY_EDITOR
                StartLoadStage(stageID);
#else
                ShowAd(() => 
                {
                    StartLoadStage(stageID);
                });
#endif
            }
        }
        void StartLoadStage(int stageID)
        {
            Game.StageManager.SetLastSelectStageID(stageID);

            Game.SoundManager.FadeGameBGMVolume(0f, FadeUI.DefaultFadeTime, () =>
            {
                Game.SoundManager.StopBGM();
            });

            UIManager.FadeOut(FadeUI.DefaultFadeTime, () =>
            {
                if (SceneTypeEnum.WorldScene == _sceneTransition.CurrentSceneType)
                {
                    World.LoadStage(StageManager.LastEnterStageID);
                }
                else if (SceneTypeEnum.LobbyScene == _sceneTransition.CurrentSceneType)
                {
                    _sceneTransition.ChangeScene(SceneTypeEnum.WorldScene);
                }
            });
        }
        public void LoadLobby()
        {
            float fadeTime = FadeUI.DefaultFadeTime;
            Game.SoundManager.FadeGameBGMVolume(0f, fadeTime, null);
            Game.UIManager.FadeOut(fadeTime, () =>
            {
                Game.SoundManager.StopBGM();
                _sceneTransition.ChangeScene(SceneTypeEnum.LobbyScene);
            });
        }
        public void OnDisabledScene(SceneTypeEnum disableScene)
        {
            if(disableScene == SceneTypeEnum.LobbyScene)
            {
                if (null != Lobby)
                {
                    Lobby.Release();
                    Lobby = null;
                }
            }
            else if(disableScene == SceneTypeEnum.WorldScene)
            {
                if (null != World)
                {
                    World = null;
                }
            }
        }
        public void OnChangedScene(SceneTypeEnum prevSceneType, SceneTypeEnum afterSceneType)
        {
            if(SceneTypeEnum.LobbyScene == afterSceneType)
            {
                if(null == Lobby)
                {
                    Lobby = new Lobby();
                    Lobby.Init();
                }
            }
            else if(SceneTypeEnum.WorldScene == afterSceneType)
            {
                if(null == World)
                {
                    World = new World();
                    World.Init(StageManager.LastEnterStageID);
                }
            }

            UIManager.OnChangedScene(prevSceneType, afterSceneType);
        }
        public bool CheckCanStageEnter(int stageID)
        {
            StageInfo stageInfo = StageManager.GetStageInfo(stageID);
            if (null == stageInfo)
                return false;

#if MASTER
            // {{ 마스터 모드
            if (IsMasterMode)
                return true;
            // }}
#endif

            if (stageInfo.IsLock())
                return false;
            return true;
        }
        public void DoDarkness()
        {
            _darknessRenderFeature.DoDarkness();
        }
        public void DoBrightness()
        {
            _darknessRenderFeature.DoBrightness();
        }
        public void ChangeLanguage(LanguageIDEnum languageID)
        {
            if (LocalData.LanguageID == languageID)
                return;

            LocalData.SetLanguageID(languageID);

            DataManager.Texts.OnChangeLanguage();
            UIManager.OnChangeLanguage();
        }
        //======================================= Admob Start ==========================================================
        public void ShowAd(Action callbackSucceededWatchAd)
        {
            _callbackAdmobWatch = callbackSucceededWatchAd;
            _admobModule.ShowAd();
        }
        public void OnAdLoad(bool isSucceeded, AdFailedToLoadEventArgs failedArgs)
        {
        }
        public void OnAdShow(bool isSucceeded)
        {
            if(false == isSucceeded)
            {
                UI.UIManager.LoadAsyncMessagePopupUI("Failed load admob", "plz check Admob", MessagePopupUI.ButtonTypeEnum.Confirm,
                    null,null);
            }
        }
        public void OnAdWatchSucceeded()
        {
            _callbackAdmobWatch?.Invoke();
            _callbackAdmobWatch = null;
        }
        public void OnAdClose()
        {
        }

        //======================================= Admob End ==========================================================
        void IGameMessageReceiver.ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            if (null != World)
                World.ProcessGameMessage(messageName, message);
            if (null != UIManager)
                UIManager.ProcessGameMessage(messageName, message);
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}