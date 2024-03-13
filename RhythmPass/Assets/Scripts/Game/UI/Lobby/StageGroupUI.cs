using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    using Data;
    public class StageGroupUI : MonoBehaviour
    {
        [SerializeField] Image _image;
        [SerializeField] Image _shadowImage;
        [SerializeField] Text _nameText;
        [SerializeField] Text _descriptionText;
        [SerializeField] StateButton _selectButton;
        [SerializeField] Image _lockImage;
        StageGroupInfo _stageGroupInfo;
        public void Init(int stageGroupID)
        {
            Init(Game.StageManager.GetStageGroupInfo(stageGroupID));   
        }
        public void Init(StageGroupInfo groupInfo)
        {
            _stageGroupInfo = groupInfo;
            bool isLock = Game.StageManager.IsLock(_stageGroupInfo);
            if (null != _image || null != _shadowImage)
            {
                AssetManager.LoadAsync<Sprite>(_stageGroupInfo.Record.ImagePath, (sprite) =>
                {
                    if (null != _image)
                    {
                        _image.sprite = sprite;
                        _image.color = isLock ? Color.gray : Color.white;
                    }
                    if (null != _shadowImage)
                        _shadowImage.sprite = sprite;
                });
            }
            if (null != _nameText)
            {
                _nameText.text = DataManager.Texts.GetText(groupInfo.Record.Name);
                _nameText.color = isLock ? Color.gray : Color.white;
            }
            if (null != _descriptionText)
                _descriptionText.text = DataManager.Texts.GetText(groupInfo.Record.Description);
            if (null != _selectButton)
            {
                _selectButton.SetState(!isLock);
                if (null != _selectButton.Text)
                    _selectButton.Text.text = DataManager.Texts.FormatText(11, _stageGroupInfo.GetClearStageCount(), _stageGroupInfo.StageInfoList.Count);
            }
            if (null != _lockImage)
                _lockImage.gameObject.SetActive(isLock);
        }
        public void OnClickEnter()
        {
            if (null == _stageGroupInfo)
                return;
            if (Game.StageManager.IsLock(_stageGroupInfo))
                return;
            UIManager.LoadAsyncStageSelectUI(_stageGroupInfo.ID);
        }
    }
}