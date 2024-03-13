using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    [RequireComponent(typeof(Text)), DisallowMultipleComponent]
    public class LocalizeText : MonoBehaviour
    {        
        [SerializeField] private int tableKey = -1;
        [SerializeField] private bool activate = true;
        [SerializeField] private Text textUi;

        public int TableKey { get { return tableKey; } }

        private void OnValidate()
        {
            textUi = GetTextComponent();
        }
        private void Awake()
        {
            if(false != activate)
            {
                SetText();
                GetTextComponent();
            }            
        }
        public void SetText()
        {
            if (0 >= tableKey)
                return;
            if(textUi == null)
                textUi = GetTextComponent();
            if (null == textUi)
                return;
            textUi.text = GetLocalizedText();
        }
        public void SetText(string text)
        {
            if(textUi == null)
                textUi = GetTextComponent();
            textUi.text = text;
        }
        public Text GetTextComponent()
        {
            return this.gameObject.GetComponent<Text>();
        }
        public void SetArgsText(params object[] args)
        {
            if(null != textUi)
            {
                textUi.text = Data.DataManager.Texts.FormatText(TableKey, args);
            }
        }
        string GetLocalizedText()
        {
            string localizedText = string.Empty;
            if (0 < tableKey)
            {
                if(null != Data.DataManager.Texts)
                    localizedText = Data.DataManager.Texts.GetText(tableKey);
            }
            return localizedText;
        }
        
    }
}
