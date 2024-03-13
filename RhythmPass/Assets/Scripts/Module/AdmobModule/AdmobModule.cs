using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
namespace Dev.Module.Admob
{
    /// <summary>
    /// 애드몹 호출 결과를 받을 인터페이스
    /// </summary>
    public interface IAdmobCallbackReceiver
    {
        void OnAdLoad(bool isSucceeded, AdFailedToLoadEventArgs failedArgs);
        void OnAdShow(bool isSucceeded);
        void OnAdWatchSucceeded();
        void OnAdClose();
    }
    public class AdmobModuleInitializer
    {
        public string UnitID;
        public IAdmobCallbackReceiver CallbackReceiver;
        public bool ActiveLog;
        //public readonly List<string> TestDevices = new List<string>(); 
        public void Init(string unitID, IAdmobCallbackReceiver callbackReceiver,bool activeLog)
        {
            UnitID = unitID;
            CallbackReceiver = callbackReceiver;
            ActiveLog = activeLog;
        }
        //public void AddTestDevice(string deviceID)
        //{
        //    TestDevices.Add(deviceID);
        //}
    }
    /// <summary>
    /// 현재는 보상형 광고만 작업이 되어 있습니다. 이후 추가로 다른 광고 작업이 필요하면 수정
    /// </summary>
    public class AdmobModule
    {
        //광고 테스트를 위한 UnitID 입니다(미출시 상태에서 테스트를 위해 광고 테스트를 할 경우 정책 위반 방지를 위해 아래 값을 사용해야 합니다)
        public const string TestUnitID = "ca-app-pub-3940256099942544/5224354917";
        private RewardedAd _rewardedAdmob;
        private AdmobModuleInitializer _initializer;
        public void Init(AdmobModuleInitializer initializer)
        {
            _initializer = initializer;
            MobileAds.Initialize((status) => 
            {
                RequestRewardAdmobLoad();
            });
        }
        public void Release()
        {
            _rewardedAdmob.OnAdLoaded -= OnAdLoadSucceeded;
            _rewardedAdmob.OnAdFailedToLoad -= OnAdLoadFailed;
            _rewardedAdmob.OnAdOpening -= OnAdShowSucceeded;
            _rewardedAdmob.OnAdFailedToShow -= OnAdShowFailed;
            _rewardedAdmob.OnUserEarnedReward -= OnAdSucceededWatch;
            _rewardedAdmob.OnAdClosed -= OnAdSkipOrClose;

            _rewardedAdmob = null;
        }
        public bool IsLoaded()
        {
            return null != _rewardedAdmob &&  _rewardedAdmob.IsLoaded();
        }
        public void ShowAd()
        {
            if (false == IsLoaded())
            {
                Log("AdmobModule::ShowAd is not admob loaded");
                return;
            }
            _rewardedAdmob.Show();
        }
        void OnAdLoadSucceeded(object sender,EventArgs args)
        {
            Log("AdmobModule::OnAdLoadSucceeded");
            if (null != _initializer.CallbackReceiver)
                _initializer.CallbackReceiver.OnAdLoad(true, null);
        }
        void OnAdLoadFailed(object sender,AdFailedToLoadEventArgs failedArgs)
        {
            Log("AdmobModule::OnAdLoadFailed");
            if (null != _initializer.CallbackReceiver)
                _initializer.CallbackReceiver.OnAdLoad(false, failedArgs);
        }
        void OnAdShowSucceeded(object sender,EventArgs args)
        {
            Log("AdmobModule::OnAdShowSucceeded");
            if (null != _initializer.CallbackReceiver)
                _initializer.CallbackReceiver.OnAdShow(true);
        }
        void OnAdShowFailed(object sender,EventArgs args)
        {
            Log("AdmobModule::OnAdShowFailed");
            if (null != _initializer.CallbackReceiver)
                _initializer.CallbackReceiver.OnAdShow(false);
        }
        void OnAdSucceededWatch(object sender, EventArgs args)
        {
            Log("AdmobModule::OnAdSucceededWatch");
            if (null != _initializer.CallbackReceiver)
                _initializer.CallbackReceiver.OnAdWatchSucceeded();
        }
        void OnAdSkipOrClose(object sender, EventArgs args)
        {
            Log("AdmobModule::OnAdSkipOrClose");
            if (null != _initializer.CallbackReceiver)
                _initializer.CallbackReceiver.OnAdClose();
            //광고 닫혔으면 항상 다음 광고 미리 로드
            RequestRewardAdmobLoad();
        }
        void RequestRewardAdmobLoad()
        {
            if(null != _rewardedAdmob)
            {
                _rewardedAdmob.Destroy();
            }

            _rewardedAdmob = new RewardedAd(_initializer.UnitID);

            _rewardedAdmob.OnAdLoaded += OnAdLoadSucceeded;
            _rewardedAdmob.OnAdFailedToLoad += OnAdLoadFailed;
            _rewardedAdmob.OnAdOpening += OnAdShowSucceeded;
            _rewardedAdmob.OnAdFailedToShow += OnAdShowFailed;
            _rewardedAdmob.OnUserEarnedReward += OnAdSucceededWatch;
            _rewardedAdmob.OnAdClosed += OnAdSkipOrClose;

            _rewardedAdmob.LoadAd(new AdRequest.Builder().Build());

             Log("OnAdmobInit Succeeded and try load admob");
        }
        void Log(string log)
        {
            if (null != _initializer && _initializer.ActiveLog)
                Debug.Log(log);
        }
    }
}
