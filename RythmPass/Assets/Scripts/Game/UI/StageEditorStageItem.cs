using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class StageEditorStageItem : ManagementUIBase
    {
        [SerializeField] Button m_Button;
        [SerializeField] Text m_Text;

        StageEditorUI m_EditorUI;
        GameField m_GameField;
#if UNITY_EDITOR
        public void Init(string id, GameField gameField, StageEditorUI editorUI)
        {
            m_Text.text = id;
            m_GameField = gameField;
            m_EditorUI = editorUI;
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
            m_EditorUI.LoadGameField(m_Text.text, m_GameField);
        }
#endif
    }
}