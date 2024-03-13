using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Animations;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Dev.UI
{
    public class StageEditorUI : MonoBehaviour
    {
        enum ScrollContentType
        {
            None,
            Tile,
            Stage,
            Static,
            Gimmick,
        }

        static readonly string TileDir = @"Assets\Deploy\World\Prefabs\TileObject";
        static readonly string StageDir = @"Assets\Deploy\World\Prefabs\GameField";
        static readonly string StaticDir = @"Assets\Deploy\World\Prefabs\StaticObject";
        static readonly string GimmickDir = @"Assets\Deploy\World\Prefabs\WorldCharacter";
        static readonly string CharacterDir = @"Assets\Deploy\World\Prefabs\Characters";
        const int MaxNameLength = 20;

        [SerializeField] StageEditor m_StageEditor;
        [SerializeField] GameObject m_GameFieldGo;
        [SerializeField] GameObject m_ScrollContent;
        [SerializeField] InputField m_StageNameInputField;
        [SerializeField] Button m_Button;
        [SerializeField] GameObject m_FoldGo;
        [SerializeField] StageEditorTileItem m_TileItem;
        [SerializeField] StageEditorStageItem m_StageItem;
        [SerializeField] StageEditorTypeItem m_TypeItem;
        [SerializeField] InputField m_XInput;
        [SerializeField] InputField m_YInput;
        [SerializeField] GameObject m_ContentPool;
        [SerializeField] GameObject m_StagePool;
        [SerializeField] GameObject m_TilePool;
        [SerializeField] GameObject m_StaticPool;
        [SerializeField] GameObject m_GimmickPool;

        ScrollContentType m_CurrentContentType;
        bool m_IsFolded = false;

#if UNITY_EDITOR
        void Awake()
        {
            InitContents();
            m_StageEditor.Init();
            m_StageEditor.SetSize(5, 7);
            SetContent(ScrollContentType.Tile);
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.up * 3f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * 3f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.down * 3f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * 3f * Time.deltaTime);
            }
        }

        void InitContents()
        {
            InitPrefabs<TileObject, StageEditorTileItem>(TileDir, m_TilePool, m_TileItem, (name, item, clone) => clone.Init(name, item, m_StageEditor));
            InitPrefabs<GameField, StageEditorStageItem>(StageDir, m_StagePool, m_StageItem, (name, item, clone) => clone.Init(name, item, this));
            InitPrefabs<GameObject, StageEditorTypeItem>(GimmickDir, m_GimmickPool, m_TypeItem, (name, item, clone) => clone.Init(name, item, m_StageEditor));
            InitPrefabs<GameObject, StageEditorTypeItem>(CharacterDir, m_GimmickPool, m_TypeItem, (name, item, clone) => clone.Init(name, item, m_StageEditor));
            InitPrefabs<GameObject, StageEditorTypeItem>(StaticDir, m_StaticPool, m_TypeItem, (name, item, clone) => clone.Init(name, item, m_StageEditor));
        }

        void InitPrefabs<TObject, TUIItem>(string dir, GameObject pool, TUIItem uiItem, Action<string, TObject, TUIItem> onLoad = null) 
            where TObject : UnityEngine.Object
            where TUIItem : MonoBehaviour
        {
#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:prefab", new string[] { dir });
            foreach (var guid in guids)
            {
                CreatePrefab(guid, pool, uiItem, onLoad);   
            }
#endif
        }

        void CreatePrefab<TObject, TUIItem>(string guid, GameObject pool, TUIItem uiItem, Action<string, TObject, TUIItem> onLoad = null)
            where TObject : UnityEngine.Object
            where TUIItem : MonoBehaviour
        {
#if UNITY_EDITOR
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetManager.Load<TObject>(path);
            var clone = Instantiate(uiItem);
            var name = Path.GetFileNameWithoutExtension(path);
            if (name.Length > MaxNameLength)
            {
                name = name.Remove(MaxNameLength - 1);
            }
            clone.gameObject.SetParent(pool);
#endif
            onLoad?.Invoke(name, item, clone);
        }

        void SetContent(ScrollContentType contentType)
        {
            if (m_CurrentContentType == contentType)
            {
                return;
            }
            switch (contentType)
            {
                case ScrollContentType.Tile:
                    MovePoolObjects(m_TilePool);
                    break;
                case ScrollContentType.Stage:
                    MovePoolObjects(m_StagePool);
                    break;
                case ScrollContentType.Static:
                    MovePoolObjects(m_StaticPool);
                    break;
                case ScrollContentType.Gimmick:
                    MovePoolObjects(m_GimmickPool);
                    break;
            }
            m_CurrentContentType = contentType;
        }

        void MovePoolObjects(GameObject from)
        {
            GameObject toGo = null;
            switch (m_CurrentContentType)
            {
                case ScrollContentType.Tile:
                    toGo = m_TilePool;
                    break;
                case ScrollContentType.Stage:
                    toGo = m_StagePool;
                    break;
                case ScrollContentType.Static:
                    toGo = m_StaticPool;
                    break;
                case ScrollContentType.Gimmick:
                    toGo = m_GimmickPool;
                    break;
            }
            var fromChilds = from.GetComponentsInChildren<ManagementUIBase>();
            var contentChilds = m_ScrollContent.GetComponentsInChildren<ManagementUIBase>();
            
            foreach (var child in fromChilds)
            {
                child.gameObject.SetParent(m_ScrollContent);
            }
            if (toGo != null)
            {
                foreach (var child in contentChilds)
                {
                    child.gameObject.SetParent(toGo);
                }
            }
        }

        public void OnClickSaveButton()
        {
#if UNITY_EDITOR
            if (null != m_StageNameInputField)
            {
                var text = m_StageNameInputField.text;
                if (text != null)
                {
                    var name = Path.Combine(StageDir, text);
                    name += ".prefab";
                    var lookAt = m_GameFieldGo.GetComponentInChildren<LookAtConstraint>();
                    lookAt.constraintActive = true;
                    PrefabUtility.SaveAsPrefabAsset(m_GameFieldGo, name);
                    AssetDatabase.Refresh();
                    var guid = AssetDatabase.AssetPathToGUID(name);
                    CreatePrefab<GameField, StageEditorStageItem>(guid, m_StagePool, m_StageItem, (name, item, clone) => clone.Init(name, item, this));
                }
            }
#endif
        }

        public void OnClickFoldButton()
        {
            m_FoldGo.SetActive(m_IsFolded);
            m_IsFolded = !m_IsFolded;
        }

        public void OnClickCreateButton()
        {
            if (int.TryParse(m_XInput.text, out int x) &&
                int.TryParse(m_YInput.text, out int y))
            {
                m_StageEditor.SetSize(x, y);
            }
        }

        public void OnClickTileButton()
        {
            SetContent(ScrollContentType.Tile);
        }

        public void OnClickStageButton()
        {
            SetContent(ScrollContentType.Stage);
        }

        public void OnClickGimmickButton()
        {
            SetContent(ScrollContentType.Gimmick);
        }

        public void OnClickStatickButton()
        {
            SetContent(ScrollContentType.Static);
        }

        public void LoadGameField(string id, GameField gameField)
        {
            m_StageNameInputField.text = id;
            m_StageEditor.LoadGameField(gameField);
            m_GameFieldGo = m_StageEditor.GameField.gameObject;
        }

#endif
    }
}