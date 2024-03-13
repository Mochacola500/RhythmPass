using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
namespace Dev.UI
{
    public enum UINameEnum : int
    {
        None = 0,

        MainTitleUI,
        StageSelectUI,
        FadeUI,
        BeatUI,
        MainHUDUI,
        OptionUI,
        StageClearUI,
        SystemMessageUI,
        LobbyUI,
        EditorUI,
        MessagePopupUI,
        StageEnterUI,
        RewardUI,
        NextStageEnterUI,
        StageFailUI,
        TutorialUI,
    }
    public partial class UIManager : IGameMessageReceiver
    {
        public Canvas MainCanvas { get; private set; }
        public GraphicRaycaster GraphicRaycaster { get; private set; }
        readonly List<ManagementUIBase> _uiList = new List<ManagementUIBase>();
        GameObject _touchEffect;
        public void Init(Initializer initializer)
        {
            MainCanvas = initializer.MainCanvas;
            GraphicRaycaster = initializer.GraphicRaycaster;
            _touchEffect = initializer.TouchEffect;

            InitCanvasScaler();
        }
        public void InitCanvasScaler(bool resize = false)
        {
            var scaler = MainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (null != scaler)
            {
                scaler.matchWidthOrHeight = ((float)Screen.width / (float)Screen.height) < 0.5625 ? 0 : 1;

                if (resize)
                {
                    scaler.enabled = false;
                    scaler.enabled = true;
                }
            }
        }
        public void Update()
        {
            if (null != _touchEffect)
            {
                if (Input.GetMouseButtonDown(0) /*&& IsTouchUIObject()*/)
                {
                    _touchEffect.gameObject.SetActive(false);
                    _touchEffect.transform.position = Input.mousePosition;
                    _touchEffect.gameObject.SetActive(true);
                }
            }
        }

        public void CreateUI<T>(string prefabPath, Action<T> callback = null) where T : ManagementUIBase
        {
            AssetManager.LoadAsync<GameObject>(prefabPath, (prefab) => 
            {
                if (null == prefab)
                    return;
                GameObject newObject = GameObject.Instantiate(prefab,MainCanvas.transform);
                T component = newObject.GetComponent<T>();

                AddUI(component);

                callback?.Invoke(component);
            });
        }
        public void RemoveUI(ManagementUIBase ui)
        {
            ui.OnCloseUI();
            _uiList.Remove(ui);
            GameObject.Destroy(ui.gameObject);
        }
        public void RemoveAllUI(bool isIgnoreStaticUI = true)
        {
            for(int i =0; i < _uiList.Count; ++i)
            {
                if(isIgnoreStaticUI && _uiList[i].IsStatic)
                {
                    continue;
                }

                _uiList[i].OnCloseUI();
                GameObject.Destroy(_uiList[i].gameObject);
                _uiList.RemoveAt(i);
                --i;
            }

        }
        public void AddUI(ManagementUIBase ui)
        {
            ui.ApplySafeArea();

            _uiList.Add(ui);

#if MASTER
            if (null != Game.Instance.DevUI)
                Game.Instance.DevUI.transform.parent.SetSiblingIndex(Game.Instance.DevUI.transform.parent.parent.childCount - 1);
#endif

            if (null != _touchEffect)
                _touchEffect.transform.SetSiblingIndex(_touchEffect.transform.parent.childCount - 1);
        }
        public T GetUI<T>(UINameEnum name) where T : ManagementUIBase
        {
            foreach(var ui in _uiList)
            {
                if (ui.UIName == name)
                    return ui as T;
            }
            return null;
        }
        public void OnLobbyInit()
        {
            MainTitleUI mainTitleUI = MonoBehaviour.FindObjectOfType<MainTitleUI>();
            if(null != mainTitleUI)
            {
                AddUI(mainTitleUI);
            }   
            else
            {
                UIManager.LoadAsyncLobbyUI();
            }
            SystemMessageUI systemMessageUI = MonoBehaviour.FindObjectOfType<SystemMessageUI>();
            if(null != systemMessageUI)
            {
                AddUI(systemMessageUI);
            }
            else
            {
                LoadAsyncSystemMessageUI(null);
            }
        }
        public void SystemMessageMiddle(string text, float time = 1f,Action callbackEnd = null)
        {
            GetUI<SystemMessageUI>(UINameEnum.SystemMessageUI).PlayMiddleText(text, time, callbackEnd);
        }
        public void FadeOut(float time,Action callbackComplete)
        {
            FadeUI ui = GetUI<FadeUI>(UINameEnum.FadeUI);

            if (null != ui)
                ui.FadeOut(time, callbackComplete);
            else
                LoadAsyncFadeUI((fadeUI) => { fadeUI.FadeOut(time, callbackComplete); });

            SetUIInteraction(false);
        }
        public void FadeIn(float time,Action callbackComplete)
        {
            FadeUI ui = GetUI<FadeUI>(UINameEnum.FadeUI);

            Action callback = () =>
            {
                SetUIInteraction(true);
                callbackComplete?.Invoke();
            };

            if (null != ui)
                ui.FadeIn(time, callback);
            else
                LoadAsyncFadeUI((fadeUI) => { fadeUI.FadeIn(time, callback); });
        }
        public void SetUIInteraction(bool isActive)
        {
            if (null != GraphicRaycaster)
                GraphicRaycaster.enabled = isActive;
        }
        public FadeTypeEnum GetFadeState()
        {
            FadeUI ui = GetUI<FadeUI>(UINameEnum.FadeUI);
            if (null == ui)
                return FadeTypeEnum.FadeIn;
            return ui.GetFadeState();
        }
        public void OnChangedScene(SceneTypeEnum prevSceneType, SceneTypeEnum afterSceneType)
        {
            RemoveAllUI();

            if(SceneTypeEnum.LobbyScene == afterSceneType)
            {
                LoadAsyncLobbyUI();
                LoadAsyncStageSelectUI(Game.StageManager.LastEnterStageGroupID, () => 
                {
                    FadeIn(FadeUI.DefaultFadeTime, null);
                });
            }
        }
        public bool IsTouchUIObject()
        {
            if (null == EventSystem.current)
                return false;
            return EventSystem.current.IsPointerOverGameObject();
        }
        public void BounceUI(Transform transform)
        {
            //DOTween.Sequence().Append(transform.DOScale(1.3f, 0.05f)).
            //    Append(transform.DOScale(1.0f, 0.05f));
            DOTween.Sequence().
                Append(transform.DOScale(0.8f, 0.05f)).
                Append(transform.DOScale(1.3f, 0.05f)).
                Append(transform.DOScale(1f, 0.05f));
        }
        public void InitWorldSceneUI()
        {
            RemoveAllUI();
            LoadAsyncMainHUDUI();
        }
        public void OnChangeLanguage()
        {
            foreach(var ui in _uiList)
            {
                ui.OnChangeLanguage();
            }
        }
        public void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message)
        {
            switch(messageName)
            {
                case GameMessageEnum.ChangedStage:
                    InitWorldSceneUI();
                    break;
                case GameMessageEnum.StageEnd:
                    GameMessage.StageEnd stageEndMessage = message.Cast<GameMessage.StageEnd>();
                    if(stageEndMessage.IsClear)
                    {
                        LoadAsyncStageClearUI(stageEndMessage);
                    }
                    else
                    {
                        LoadAsyncStageFailUI(stageEndMessage.StageInfo);
                    }
                    break;
            }
            //todo °³¼±
            List<ManagementUIBase> copyList = new List<ManagementUIBase>(_uiList.Count);
            copyList.AddRange(_uiList);

            foreach(var ui in copyList)
            {
                ui.ProcessGameMessage(messageName, message);
            }
        }
    }
}
