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
        private static bool s_Initialized = false;      //�ʱ�ȭ �Ϸ�
        private static bool s_CatalogLoaded = false;    //īŻ�α� �ε� �Ϸ�
        private static bool s_AssetsReady = false;      //�ٿ�ε� �� ��� �۾� �Ϸ�
        
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
            //Remote ����Ҷ� �߰� ����
            //CatalogPath = string.IsNullOrEmpty(url) ? CatalogPath /*���� Ÿ�ٺ� ��η� �����ؾ���*/ : url;

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
            //�׻� catalog�� �ֽ�ȭ�� �õ��Ѵ�                
            _checkForCatalogUpdatesHandle = Addressables.CheckForCatalogUpdates(false);
            _checkForCatalogUpdatesHandle.Completed += (catalogs) =>
            {
                if (AsyncOperationStatus.Succeeded != catalogs.Status)
                {
                    Debug.LogError("Addressables Fatal Error: Fetch failed"); // ����

                    //Remote�� ����� ���..
                    //���ͳ� ������ �ȵǾ Remote catalog �б� ����
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
                    var popupStr = $"���ҽ� ���� �ٿ�ε� ���� Ȥ�� ��Ʈ��ũ ������ ��Ȱ���� �ʽ��ϴ�. �ٽ� �õ����ּ���.\nError code: {(int)errCode}";

                    //1. catalog�� link���ϵ� �ʼ����� ������ cached ������ streamingassets�� �ִ��� Ȯ�� (�����Ҷ� ���ԵǴ��� Ȯ��)
                    //2. URLȮ��
                    //3. ������ ������ �� ������ ��� Ȯ��
                    //4. �����Ҷ� �÷��� Ȯ�� (CRC mismatch)
                    //5. ������ ������ �ִ��� Ȯ�� (�ʹ� �����ɸ��� ��ȭ���̳� ��Ʈ ������ ��� ����)
                    //6. ĳ�������� ����� �ٽ� �õ�(���ݿ��� �ٿ�ε��� catalog�� �켱������ �б� ������ ó������ �߸��� catalog�� ������ ������ ���� �õ����� ������ ���� �� ����)

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
                    //play mode fast �� ��� editor only
                    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                    var setting = settings.GetDataBuilder(settings.ActivePlayModeDataBuilderIndex);

                    if (setting.GetType() == typeof(UnityEditor.AddressableAssets.Build.DataBuilders.BuildScriptFastMode))
                    {
                        //�����Ϳ��� ���Ѱ��̹Ƿ�.. �ٷ� Pass
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
                            //catalog�� ���� �Ѵ� ������Ʈ
                            //Remote�� ��� �ٿ�ε带 ��ġ�� �ʱ�ȭ ������ ���ľ���
                            Game.Lobby.TryPatchDownload(locatorsToUpdate.Result);

                            OnReadyAssetManager();
                        };
                    }
                    else
                    {
                        //�ֽ� ������ catalog�ε� ������ ���� ���,
                        //������ ó�� ������ ��
                        //Remote�� ��� �ٿ�ε带 ��ġ�� �ʱ�ȭ ������ ���ľ���
                        Game.Lobby.TryPatchDownload(initLocator);

                        OnReadyAssetManager();
                    }
                }
            };
        }

        private static void PopupRetryUpdateCatalog(string popupContents)
        {
            //todo catalog�� update�� ����������� �ٽ� �õ��� �� �ֵ��� �ȳ� �޼����� ���� �۾�
            //������ ������ �׳� ����..

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