using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.UI
{
    public class ManagementUIBase : MonoBehaviour, IGameMessageReceiver
    {
        public UINameEnum UIName;
        public RectTransform SafeAreaTransform;
        [Tooltip("씬이동에도 UI 유지 여부")]public bool IsStatic;
        public void CloseUI()
        {
            Game.UIManager.RemoveUI(this);
        }
        public virtual void OnCloseUI() {  }
        public virtual void ProcessGameMessage(GameMessageEnum messageName, IGameMessage message) { }
        public virtual void Refresh() { }
        public virtual void OnChangeLanguage()
        {
            LocalizeText[] localizeTexts = transform.GetComponentsInChildren<LocalizeText>();
            foreach(var text in localizeTexts)
            {
                text.SetText();
            }

            Refresh();
        }
        public void ApplySafeArea()
        {
            if (null == SafeAreaTransform)
                return;

            Rect safeArea = Screen.safeArea;
            var minAnchor = safeArea.position;
            var maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            SafeAreaTransform.anchorMin = minAnchor;
            SafeAreaTransform.anchorMax = maxAnchor;
        }
    }
}