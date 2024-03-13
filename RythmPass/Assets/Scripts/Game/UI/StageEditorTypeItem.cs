using System;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class StageEditorTypeItem : ManagementUIBase
    {
        [SerializeField] Button m_Button;
        [SerializeField] Text m_Text;
        StageEditor m_Editor;
        GameObject m_Go;

        public void Init(string id, GameObject go, StageEditor editor)
        {
            m_Text.text = id;
            m_Go = go;
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
            // 나중에 바꿔야함.
            var gimmick = m_Go.GetComponent<Gimmick>();
            if (gimmick != null)
            {
                m_Editor.SetSelectWorldCharacter(gimmick);
            }
            var goal = m_Go.GetComponent<GoalObject>();
            if (goal != null)
            {
                m_Editor.SetSelectStaticObject(goal);
            }
            var trap = m_Go.GetComponent<StaticTrap>();
            if (trap != null)
            {
                m_Editor.SetSelectStaticObject(trap);
            }
            var portal = m_Go.GetComponent<PortalObject>();
            if (portal != null)
            {
                m_Editor.SetSelectStaticObject(portal);
            }
            var character = m_Go.GetComponent<PlayerCharacter>();
            if (character != null)
            {
                m_Editor.SetSelectWorldCharacter(character);
            }
        }
    }
}