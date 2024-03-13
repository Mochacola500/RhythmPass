using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class SlotBase : MonoBehaviour
    {

        [SerializeField] Image _image;
        [SerializeField] Text _valueText;

        public void Init(IRewardInfo rewardInfo)
        {
            InitImage(Reward.GetRewardIconPath(rewardInfo.ID));
            InitValue(rewardInfo.Record.Value);
        }
        void InitImage(string path)
        {
            AssetManager.LoadAsync<Sprite>(path, (sprite) => 
            {
                if(null != sprite)
                    _image.sprite = sprite;
            });
        }
        void InitValue(long value)
        {
            if (null != _valueText)
            {
                _valueText.text = string.Format("x{0}", value.ToString("N0"));
            }
        }
    }
}