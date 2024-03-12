using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
#endif // UNITY_EDITOR

using Object = UnityEngine.Object;

namespace Dev
{
    public static class AssetManager 
    {
        public static string CatalogPath = "";
        public static bool Completed { get { return s_AssetsReady; } }
        private static bool s_Initialized = false;      //초기화 완료
        private static bool s_CatalogLoaded = false;    //카탈로그 로드 완료
        private static bool s_AssetsReady = false;      //다운로드 및 모든 작업 완료
        
        public static event Action OnComplete;
        public static bool HasInit { get; private set; } = false;
        private static AsyncOperationHandle<IResourceLocator> _initHandle;
        public static bool HasCheckForCatalogUpdatesHandle { get; private set; } = false;
        private static AsyncOperationHandle<List<string>> _checkForCatalogUpdatesHandle;
        public static bool HasCatalogLocatorHandle { get; private set; } = false;
        private static AsyncOperationHandle<List<IResourceLocator>> _catalogLocatorHandle;
        private static void OnReadyAssetManager()
        {
            s_AssetsReady = true;
            ClearHandles();
            OnComplete?.Invoke();
        }

        private static void ClearHandles()
        {
            if (_catalogLocatorHandle.IsValid())
                Addressables.Release(_catalogLocatorHandle);

            if (_checkForCatalogUpdatesHandle.IsValid())
                Addressables.Release(_checkForCatalogUpdatesHandle);

            if (_initHandle.IsValid())
                Addressables.Release(_initHandle);

            HasInit = false;
            HasCheckForCatalogUpdatesHandle = false;
            HasCatalogLocatorHandle = false;
        }
        public static void Init(string url)
        {
            //Remote 사용할때 추가 수정
            //CatalogPath = string.IsNullOrEmpty(url) ? CatalogPath /*빌드 타겟별 경로로 수정해야함*/ : url;

            _initHandle = Addressables.InitializeAsync(false);
            _initHandle.Completed += InitDone;
        }

        private static void InitDone(AsyncOperationHandle<IResourceLocator> initLocator)
        {
            Debug.Log($" Addressables.InitializeAsync done. status: {initLocator.Status}");

            var initExcep = initLocator.OperationException;
            if (initLocator.Status == AsyncOperationStatus.Succeeded)
            {
                s_Initialized = true;
                TryUpdateCatalogs(initLocator);
            }
            else
            {
                UI.UIManager.LoadAsyncMessagePopupUI("Error"
                    , "[Fatal Error]\nFail InitializeAsync"
                    , UI.MessagePopupUI.ButtonTypeEnum.Confirm
                    , Game.Quit
                    , null
                    );
            }
        }

        private static void TryUpdateCatalogs(AsyncOperationHandle<IResourceLocator> initLocator)
        {
            //항상 catalog는 최신화를 시도한다                
            _checkForCatalogUpdatesHandle = Addressables.CheckForCatalogUpdates(false);
            _checkForCatalogUpdatesHandle.Completed += (catalogs) =>
            {
                if (AsyncOperationStatus.Succeeded != catalogs.Status)
                {
                    Debug.LogError("Addressables Fatal Error: Fetch failed"); // 콘텐

                    //Remote를 사용할 경우..
                    //인터넷 연결이 안되어서 Remote catalog 읽기 실패
                    var errCode = AddressableError.UnknownError;
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        errCode = AddressableError.NotConnectedInternet;
                    }
                    else
                    {
                        errCode = AddressableError.ConnectedInternetButSomethingError;
                    }

                    var errorLog = _checkForCatalogUpdatesHandle.OperationException?.Message;
                    var popupStr = $"리소스 파일 다운로드 실패 혹은 네트워크 연결이 원활하지 않습니다. 다시 시도해주세요.\nError code: {(int)errCode}";

                    //1. catalog와 link파일등 필수적인 파일이 cached 폴더나 streamingassets에 있는지 확인 (빌드할때 포함되는지 확인)
                    //2. URL확인
                    //3. 번들이 서버에 잘 들어갔는지 경로 확인
                    //4. 빌드할때 플랫폼 확인 (CRC mismatch)
                    //5. 서버에 문제가 있는지 확인 (너무 오래걸리면 방화벽이나 포트 문제일 경우 있음)
                    //6. 캐시파일을 지우고 다시 시도(원격에서 다운로드한 catalog를 우선적으로 읽기 때문에 처음부터 잘못된 catalog를 가지고 있으면 다음 시도때도 문제가 있을 수 있음)

                    if (_checkForCatalogUpdatesHandle.IsValid() && HasCheckForCatalogUpdatesHandle)
                    {
                        Addressables.Release(_checkForCatalogUpdatesHandle);
                        HasCheckForCatalogUpdatesHandle = false;
                    }

                    PopupRetryUpdateCatalog(popupStr);
                }
                else
                {
#if UNITY_EDITOR
                    //play mode fast 일 경우 editor only
                    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                    var setting = settings.GetDataBuilder(settings.ActivePlayModeDataBuilderIndex);

                    if (setting.GetType() == typeof(UnityEditor.AddressableAssets.Build.DataBuilders.BuildScriptFastMode))
                    {
                        //에디터에서 실한것이므로.. 바로 Pass
                        OnReadyAssetManager();
                        return;
                    }
#endif
                    Debug.Log($"Addressable CheckForCatalogUpdates result count : {catalogs.Result.Count.ToString()}");
                    var list = catalogs.Result;
                    if (0 < list.Count)
                    {

                        _catalogLocatorHandle = Addressables.UpdateCatalogs(catalogs.Result, false);
                        HasCatalogLocatorHandle = true;

                        _catalogLocatorHandle.Completed += (locatorsToUpdate) =>
                        {
                            //catalog와 에셋 둘다 업데이트
                            //Remote의 경우 다운로드를 마치고 초기화 과정을 마쳐야함
                            Game.Lobby.TryPatchDownload(locatorsToUpdate.Result);

                            OnReadyAssetManager();
                        };
                    }
                    else
                    {
                        //최신 버전의 catalog인데 번들이 없을 경우,
                        //게임을 처음 실행할 때
                        //Remote의 경우 다운로드를 마치고 초기화 과정을 마쳐야함
                        Game.Lobby.TryPatchDownload(initLocator);

                        OnReadyAssetManager();
                    }
                }
            };
        }

        private static void PopupRetryUpdateCatalog(string popupContents)
        {
            //todo catalog를 update에 실패했을경우 다시 시도할 수 있도록 안내 메세지를 띄우는 작업
            //하지만 지금은 그냥 종료..

            //UI.UIManager.GetLoginMessagePopup(UI.EmessageButtonType.Enter_Cancel, popupContents, "", () =>
            //{
            //    if (_initHandle.IsValid())
            //    {
            //        TryUpdateCatalogs(_initHandle);
            //    }
            //    else
            //    {
            //        UI.UIManager.GetLoginMessagePopup(UI.EmessageButtonType.Cancel, GameData.DataTableManager.Texts.GetText(58898), "init error", null, () =>
            //        {
            //            Game.Quit();
            //        }, true,
            //        (ui) =>
            //        {
            //            ui.SetCancelText(GameData.DataTableManager.Texts.GetText(45543));
            //        });
            //        return;

            //    }
            //},
            //Game.Quit, true, (ui) =>
            //{
            //    ui.SetConfirmText(GameData.DataTableManager.Texts.GetText(141));
            //    ui.SetCancelText(GameData.DataTableManager.Texts.GetText(45543));
            //});  
        }

        public static void InstantiateComponent<T>(string assetPath, Action<T> callback, Transform parent = null, bool async = true) where T : UnityEngine.Component
        {
#if UNITY_EDITOR
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                GameObject clone = null;
                if (null != prefab)
                {
                    clone = Object.Instantiate(prefab, parent);
                }

                callback?.Invoke(clone?.GetComponent<T>());
                return;
            }
#else
            var handle = Addressables.InstantiateAsync(assetPath, parent);
            handle.Completed += (op) =>
            {                
                callback?.Invoke(op.Result?.GetComponent<T>());                
            };
#endif
        }

        public static void Instantiate<T>(string assetPath, Action<T> callback, Transform parent = null) where T : Object
        {
#if UNITY_EDITOR
            var loaded = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            var component = loaded as Component;
            var prefab = loaded as GameObject;

            GameObject obj = null;
            if (null != prefab)
            {
                obj = Object.Instantiate(prefab, parent);
            }
            else if (null != component)
            {
                obj = Object.Instantiate(component.gameObject, parent);
            }

            if (null != parent)
            {
                obj.transform.SetParent(parent);
            }

            callback?.Invoke(obj?.GetComponent<T>());
            return;
#endif
            Addressables.InstantiateAsync(assetPath, parent).Completed += (op) =>
            {
                callback?.Invoke(op.Result?.GetComponent<T>());
            };
        }
        public static void Instantiate(string assetPath, Action<GameObject> callback, Vector3 position, Quaternion rotation, Transform parent = null)
        {
#if UNITY_EDITOR
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject clone = null;
            if (null != prefab)
            {
                clone = Object.Instantiate(prefab, parent);
            }

            callback?.Invoke(clone);
            return;
#endif
            var handle = Addressables.InstantiateAsync(assetPath, position, rotation, parent);
            handle.Completed += (op) =>
            {
                callback?.Invoke(op.Result);
            };
        }

        public static void Instantiate(string assetPath, Action<GameObject> callback,Transform parent = null, bool instantiateInWorldSpace = true)
        {
#if UNITY_EDITOR
            {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (null != asset)
                {
                    GameObject prefab = null;
                    if (null != parent)
                        prefab = Object.Instantiate(asset, parent);
                    else
                        prefab = Object.Instantiate(asset);

                    callback?.Invoke(prefab);
                }
                else
                {
                    callback?.Invoke(null);
                }
                return;
            }            
            var handle = Addressables.InstantiateAsync(assetPath, parent, instantiateInWorldSpace);
            handle.Completed += (op) =>
            {
                callback?.Invoke(op.Result);
            };
#endif
        }

        public static void LoadAsset<T>(string assetPath, Action<T> callback, bool async) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            {//editor scope
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                callback?.Invoke(asset);
                return;
            }
#endif
            var handle = Addressables.LoadAssetAsync<T>(assetPath);
            if(false == async)
            {
                var asset = handle.WaitForCompletion();
                callback?.Invoke(asset);
            }
            else
            {
                handle.Completed += (asset) =>
                {
                    callback?.Invoke(asset.Result);
                };
            }
        }

        public static void LoadAsync<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object
        {
            LoadAsset<T>(assetPath, callback, true);
        }

        public static T Load<T>(string assetPath) where T : UnityEngine.Object
        {
            T asset = null;
            LoadAsset<T>(assetPath, x => { asset = x; }, false);
            return asset;
        }


        private static void ForcedDeleteAddressablesCacheDataFolderInternal()
        {
            var type = typeof(UnityEngine.AddressableAssets.Addressables);
            var field = type.GetField("m_AddressablesInstance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var targetField = field.FieldType;
            var value = targetField.GetField("kCacheDataFolder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)?.GetRawConstantValue();

            var id = null == value ? "{UnityEngine.Application.persistentDataPath}/com.unity.addressables/" : value as string;
            var path = UnityEngine.AddressableAssets.Initialization.AddressablesRuntimeProperties.EvaluateString(id);

            var dir = new System.IO.DirectoryInfo(path);
            var files = dir.Exists ? dir.GetFiles() : null;

            bool check = false;
            if (null != files)
            {
                foreach (var file in files)
                {
                    check |= file.Name.Contains(".hash") | file.Name.Contains(".json");
                }
            }

            if (false != check)
            {
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path, true);
                }
            }

        }
    }
}