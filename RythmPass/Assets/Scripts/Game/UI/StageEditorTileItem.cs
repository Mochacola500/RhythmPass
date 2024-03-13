using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class StageEditorTileItem : ManagementUIBase
    {
        [SerializeField] Button m_Button;
        [SerializeField] Text m_Text;

        StageEditor m_Editor;
        TileObject m_TileObject;

        public void Init(string id, TileObject tile, StageEditor editor)
        {
            m_Text.text = id;
            m_TileObject = tile;
            m_Editor = editor;
        }

        private void Awake()
        {
            m_Button.onClick.AddListener(OnClick);
        }

        public override void OnCloseUI()
        {
            m_Button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            m_Editor.SetSelectTileObject(m_TileObject);
        }
    }
}