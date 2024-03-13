using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
namespace Dev.Module.Admob
{
    /// <summary>
    /// �ֵ�� ȣ�� ����� ���� �������̽�
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
    /// ����� ������ ���� �۾��� �Ǿ� �ֽ��ϴ�. ���� �߰��� �ٸ� ���� �۾��� �ʿ��ϸ� ����
    /// </summary>
    public class AdmobModule
    {
        //���� �׽�Ʈ�� ���� UnitID �Դϴ�(����� ���¿��� �׽�Ʈ�� ���� ���� �׽�Ʈ�� �� ��� ��å ���� ������ ���� �Ʒ� ���� ����ؾ� �մϴ�)
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
            //���� �������� �׻� ���� ���� �̸� �ε�
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
