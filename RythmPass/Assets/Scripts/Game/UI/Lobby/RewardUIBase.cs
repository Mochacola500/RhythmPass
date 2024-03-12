using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class RewardUIBase : MonoBehaviour
    {
        [SerializeField] Image _rewardImage;
        [SerializeField] Text _amountText;
        [SerializeField] Image _receiveCheckImage;
        public void Init(IRewardInfo rewardInfo)
        {
            if(null != _rewardImage)
            {
                string path = Reward.GetRewardIconPath(rewardInfo.ID);
                AssetManager.LoadAsync<Sprite>(path, (sprite) => 
                {
                    _rewardImage.sprite = sprite;
                });
            }
            if (null != _amountText)
                _amountText.text = string.Format("x{0}", rewardInfo.Record.Value);
            if(null != _receiveCheckImage)
            {
                Reward reward = rewardInfo as Reward;
                if (null != reward)
                    _receiveCheckImage.gameObject.SetActive(reward.IsRecieve);
                else
                    _receiveCheckImage.gameObject.SetActive(false);
            }
        }
    }
}
