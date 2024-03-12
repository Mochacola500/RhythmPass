using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class MessagePopupUI : ManagementUIBase
    {
        public enum ButtonTypeEnum : int
        {
            ConfirmAndCancel,
            Confirm,
            Cancel,
        }

        [SerializeField] Text _titleText;
        [SerializeField] Text _messageText;
        [SerializeField] StateButton _confitmButton;
        [SerializeField] StateButton _cancelButton;

        Action _callbackConfirm;
        Action _callbackCancenl;

        public void Init(string titleText, string messageText,ButtonTypeEnum buttonType,Action callbackConfirm, Action callbackCancel)
        {
            if (null != _titleText)
                _titleText.text = titleText;
            if (null != _messageText)
                _messageText.text = messageText;
            if (null != _confitmButton)
            {
                _confitmButton.gameObject.SetActive(buttonType == ButtonTypeEnum.Confirm || buttonType == ButtonTypeEnum.ConfirmAndCancel);
                _confitmButton.Button.onClick.AddListener(OnClickConfirm);
            }
            if (null != _cancelButton)
            {
                _cancelButton.gameObject.SetActive(buttonType == ButtonTypeEnum.Cancel || buttonType == ButtonTypeEnum.ConfirmAndCancel);
                _cancelButton.Button.onClick.AddListener(OnClickCancel);
            }
            _callbackConfirm = callbackConfirm;
            _callbackCancenl = callbackCancel;
        }
        void OnClickConfirm()
        {
            _callbackConfirm?.Invoke();
            CloseUI();
        }
        void OnClickCancel()
        {
            _callbackCancenl?.Invoke();
            CloseUI();
        }
    }
}